using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript_1 : MonoBehaviour
{
    public bool manualCondition = false;

    StatechartInstance stateMachine = null;


    void Start()
    {
        stateMachine = GetComponent<StatechartInstance>();

        stateMachine.Subscribe("Root.B", Action.Type.ENTRY, ActivityPrintA);
    }


    void OnDestroy()
    {
        stateMachine.Unsubscribe("Root.B", Action.Type.ENTRY, ActivityPrintA);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            stateMachine.Step();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            stateMachine.AddEvent(new SCEvent("Event_1"));
        if (Input.GetKeyDown(KeyCode.Alpha2))
            stateMachine.AddEvent(new SCEvent("Event_2"));
    }


    public void ActivityPrintA(object sender, StateChartEventArgs args)
    {
        Debug.Log("Activity A from " + name);
    }

    
    public void ActivityPrintB(object sender, StateChartEventArgs args)
    {
        Debug.Log("Activity B from " + name);
    }
}
