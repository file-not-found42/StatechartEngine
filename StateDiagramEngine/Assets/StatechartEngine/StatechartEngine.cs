using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Threading;

public class StatechartEngine : MonoBehaviour
{
    const int batchSize = 32;
    const int threadCount = 32;

    class InstanceList
    {
        public List<StatechartInstance> instances;
        volatile int position;

        public int Next()
        {
            lock (instances)
            {
                position += batchSize;
                return position;
            }
        }


        public void Reset()
        {
            position = 0;
        }
    }

    static StatechartEngine instance = null;
    static bool dead = false;

    readonly List<Thread> threads;
    readonly EventWaitHandle monitor_main;
    readonly EventWaitHandle monitor_workers;
    volatile int available = 0;
    volatile InstanceList currentTask;

    readonly InstanceList updateInstances;
    readonly InstanceList lateInstances;
    readonly InstanceList fixedInstances;

#if SC_PROFILE_UPDATE
    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    AccumulatedTime accTime = new AccumulatedTime();
#endif


    [MethodImpl(MethodImplOptions.Synchronized)]
    static StatechartEngine GetInstance()
    {
        if (dead)
            return null;

        if (instance == null)
        {
            GameObject obj = new GameObject();
            DontDestroyOnLoad(obj);
            instance = obj.AddComponent<StatechartEngine>();
        }
        return instance;
    }


    StatechartEngine()
    {
        monitor_workers = new EventWaitHandle(false, EventResetMode.ManualReset);
        monitor_main = new EventWaitHandle(true, EventResetMode.ManualReset);

        threads = new List<Thread>(threadCount);
        for (int i = 0; i < threadCount; i++)
        {
            threads.Add(new Thread(ExecuteInstances)
            { Name = "SCthread " + i,
              IsBackground = true });

            threads[threads.Count - 1].Start();
        }
        available = threads.Count;

        updateInstances = new InstanceList()
        { instances = new List<StatechartInstance>() };
        lateInstances = new InstanceList()
        { instances = new List<StatechartInstance>() };
        fixedInstances = new InstanceList()
        { instances = new List<StatechartInstance>() };
    }


    public static void AddInstance(StatechartInstance ins)
    {
        switch(ins.GetMode())
        {
            case Statechart.Mode.Unity_Update:
                GetInstance()?.updateInstances.instances.Add(ins);
                break;
            case Statechart.Mode.Unity_Late_Update:
                GetInstance()?.lateInstances.instances.Add(ins);
                break;
            case Statechart.Mode.Unity_Fixed_Update:
                GetInstance()?.fixedInstances.instances.Add(ins);
                break;
        }
    }


    public static void RemoveInstance(StatechartInstance ins)
    {
        switch (ins.GetMode())
        {
            case Statechart.Mode.Unity_Update:
                GetInstance()?.updateInstances.instances.Remove(ins);
                break;
            case Statechart.Mode.Unity_Late_Update:
                GetInstance()?.lateInstances.instances.Remove(ins);
                break;
            case Statechart.Mode.Unity_Fixed_Update:
                GetInstance()?.fixedInstances.instances.Remove(ins);
                break;
        }
    }


    void Update()
    {
#if SC_PROFILE_UPDATE
        stopwatch.Start();
#endif

        DoWork(updateInstances);

#if SC_PROFILE_UPDATE
        stopwatch.Stop();
        accTime.Accumulate(stopwatch);
        stopwatch.Reset();
        if (accTime.SampleCount % 100 == 0)
            Debug.Log("Executing " + updateInstances.instances.Count + " instances.\n" + accTime.GetStatistics(AccumulatedTime.TimeUnit.µs));
#endif
    }


    void LateUpdate()
    {
        DoWork(lateInstances);
    }


    void FixedUpdate()
    {
        DoWork(fixedInstances);
    }


    private void OnApplicationQuit()
    {
        dead = true;
    }


    void DoWork(InstanceList task)
    {
        if (task.instances.Count <= 0)
            return;

        if (threadCount <= 0)
        {
            foreach (var i in task.instances)
                i.SuperStep();
            return;
        }

        currentTask = task;
        monitor_workers.Set();
        // Wait until all threads have started
        if (!monitor_main.WaitOne(34))
            Debug.LogError("One or more threads did not start.");
        monitor_main.Reset();
        monitor_workers.Reset();
        // Wait until all threads have finished
        if (!monitor_main.WaitOne(1000))
            Debug.LogError("Deadlock or loop detected.");
        monitor_main.Reset();

        task.Reset();

        if (available != threadCount)
            Debug.Log(available);
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    void ThreadStarted()
    {
        available--;
        if (available <= 0)
            monitor_main.Set();
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    void ThreadFinished()
    {
        available++;
        if (available >= threads.Count)
            monitor_main.Set();
    }


    void ExecuteInstances()
    {
        while (!dead)
        {
            monitor_workers.WaitOne();
            GetInstance().ThreadStarted();

            // While work is available
            int batch = currentTask.Next();
            while (batch < currentTask.instances.Count)
            {
                // A single batch
                for (int i = batch; i < batch + batchSize && i < currentTask.instances.Count; i++)
                    currentTask.instances[i].SuperStep();
                batch = currentTask.Next();
            }

            GetInstance().ThreadFinished();
        }
    }
}
