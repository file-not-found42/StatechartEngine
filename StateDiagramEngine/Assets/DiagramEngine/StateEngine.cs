using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StatechartEngine : MonoBehaviour
{
    static StatechartEngine instance = null;

    readonly IList<StatechartInstance> updateInstances = new List<StatechartInstance>();
    readonly IList<StatechartInstance> lateInstances = new List<StatechartInstance>();
    readonly IList<StatechartInstance> fixedInstances = new List<StatechartInstance>();
    readonly IList<StatechartInstance> guiInstances = new List<StatechartInstance>();

#if SC_PROFILE_UPDATE
    float totalMS = 0;
    long numFrames = 0;
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
            case Statechart.Mode.Unity_On_Gui:
                GetInstance().guiInstances.Add(ins);
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
            case Statechart.Mode.Unity_On_Gui:
                GetInstance().guiInstances.Remove(ins);
                break;
        }
    }


    void Update()
    {
#if SC_PROFILE_UPDATE
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
#endif

        foreach (StatechartInstance i in updateInstances)
            i.Step();

#if SC_PROFILE_UPDATE
        stopwatch.Stop();
        totalMS += ProfilingUtility.PrintStopwatchMilliseconds(stopwatch);
        numFrames++;
        Debug.Log("Average " + totalMS / numFrames + " ms");
#endif
    }


    void LateUpdate()
    {
        foreach (StatechartInstance i in lateInstances)
            i.Step();
    }


    void FixedUpdate()
    {
        foreach (StatechartInstance i in fixedInstances)
            i.Step();
    }


    void OnGUI()
    {
        foreach (StatechartInstance i in guiInstances)
            i.Step();
    }
}
