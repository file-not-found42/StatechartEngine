using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StatechartEngine : MonoBehaviour
{
    static StatechartEngine instance = null;

    readonly IList<StatechartInstance> updateInstances = new List<StatechartInstance>();
    readonly IList<StatechartInstance> lateInstances = new List<StatechartInstance>();
    readonly IList<StatechartInstance> fixedInstances = new List<StatechartInstance>();

#if SC_PROFILE_UPDATE
    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    AccumulatedTime accTime = new AccumulatedTime();
#endif


    [MethodImpl(MethodImplOptions.Synchronized)]
    static StatechartEngine GetInstance()
    {
        if (instance == null)
        {
            GameObject obj = new GameObject();
            DontDestroyOnLoad(obj);
            instance = obj.AddComponent<StatechartEngine>();
        }
        return instance;
    }

    
    public static void AddInstance(StatechartInstance ins)
    {
        switch(ins.GetMode())
        {
            case Statechart.Mode.Unity_Update:
                GetInstance().updateInstances.Add(ins);
                break;
            case Statechart.Mode.Unity_Late_Update:
                GetInstance().lateInstances.Add(ins);
                break;
            case Statechart.Mode.Unity_Fixed_Update:
                GetInstance().fixedInstances.Add(ins);
                break;
        }
    }


    public static void RemoveInstance(StatechartInstance ins)
    {
        switch (ins.GetMode())
        {
            case Statechart.Mode.Unity_Update:
                GetInstance().updateInstances.Remove(ins);
                break;
            case Statechart.Mode.Unity_Late_Update:
                GetInstance().lateInstances.Remove(ins);
                break;
            case Statechart.Mode.Unity_Fixed_Update:
                GetInstance().fixedInstances.Remove(ins);
                break;
        }
    }


    void Update()
    {
#if SC_PROFILE_UPDATE
        stopwatch.Start();
#endif

        foreach (StatechartInstance i in updateInstances)
            i.SuperStep();

#if SC_PROFILE_UPDATE
        stopwatch.Stop();
        accTime.Accumulate(stopwatch);
        stopwatch.Reset();
        if (accTime.SampleCount % 100 == 0)
            Debug.Log(accTime.GetStatistics(AccumulatedTime.TimeUnit.µs));
#endif
    }


    void LateUpdate()
    {
        foreach (StatechartInstance i in lateInstances)
            i.SuperStep();
    }


    void FixedUpdate()
    {
        foreach (StatechartInstance i in fixedInstances)
            i.SuperStep();
    }
}
