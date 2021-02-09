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
            case Statechart.Mode.UPDATE:
                GetInstance().updateInstances.Add(ins);
                break;
            case Statechart.Mode.LATE_UPDATE:
                GetInstance().lateInstances.Add(ins);
                break;
            case Statechart.Mode.FIXED_UPDATE:
                GetInstance().fixedInstances.Add(ins);
                break;
            case Statechart.Mode.ON_GUI:
                GetInstance().guiInstances.Add(ins);
                break;
        }
    }


    public static void RemoveInstance(StatechartInstance ins)
    {
        switch (ins.GetMode())
        {
            case Statechart.Mode.UPDATE:
                GetInstance().updateInstances.Remove(ins);
                break;
            case Statechart.Mode.LATE_UPDATE:
                GetInstance().lateInstances.Remove(ins);
                break;
            case Statechart.Mode.FIXED_UPDATE:
                GetInstance().fixedInstances.Remove(ins);
                break;
            case Statechart.Mode.ON_GUI:
                GetInstance().guiInstances.Remove(ins);
                break;
        }
    }


    void Update()
    {
        foreach (StatechartInstance i in updateInstances)
            i.Step();
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
