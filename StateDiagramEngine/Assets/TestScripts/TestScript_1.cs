using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript_1 : MonoBehaviour
{
    public bool manualCondition = false;

    StatechartInstance stateMachine = null;


    private void Start()
    {
        stateMachine = GetComponent<StatechartInstance>();

        stateMachine.Subscribe(new Action("Root.B", Action.Type.ENTRY), new ActionDelegate(ActivityPrintA));
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            stateMachine.Step();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            stateMachine.AddEvent(new SCEvent("Event_1"));
        if (Input.GetKeyDown(KeyCode.Alpha2))
            stateMachine.AddEvent(new SCEvent("Event_2"));
    }


    public void ActivityPrintA()
    {
        Debug.Log("Activity A from " + name);
    }

    
    public void ActivityPrintB()
    {
        Debug.Log("Activity B from " + name);
    }
}
