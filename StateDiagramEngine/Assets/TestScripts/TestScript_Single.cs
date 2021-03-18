using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript_Single : MonoBehaviour
{
    StatechartInstance stateMachine = null;


    void Start()
    {
        stateMachine = GetComponent<StatechartInstance>();

        stateMachine.Subscribe("Root.B", Action.Type.ENTRY, ActivityPrint);
    }


    void OnDestroy()
    {
        stateMachine.Unsubscribe("Root.B", Action.Type.ENTRY, ActivityPrint);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            stateMachine.Step();

        if (Input.GetKeyDown(KeyCode.Alpha0))
            stateMachine.AddEvent(new SCInternalEvent("Event_0"));
        if (Input.GetKeyDown(KeyCode.Alpha1))
            stateMachine.AddEvent(new SCInternalEvent("Event_1"));
        if (Input.GetKeyDown(KeyCode.Alpha2))
            stateMachine.AddEvent(new SCInternalEvent("Event_2"));
        if (Input.GetKeyDown(KeyCode.Alpha3))
            stateMachine.AddEvent(new SCInternalEvent("Event_3"));
        if (Input.GetKeyDown(KeyCode.Alpha4))
            stateMachine.AddEvent(new SCInternalEvent("Event_4"));
        if (Input.GetKeyDown(KeyCode.Alpha5))
            stateMachine.AddEvent(new SCInternalEvent("Event_5"));
        if (Input.GetKeyDown(KeyCode.Alpha6))
            stateMachine.AddEvent(new SCInternalEvent("Event_6"));
        if (Input.GetKeyDown(KeyCode.Alpha7))
            stateMachine.AddEvent(new SCInternalEvent("Event_7"));
        if (Input.GetKeyDown(KeyCode.Alpha8))
            stateMachine.AddEvent(new SCInternalEvent("Event_8"));
        if (Input.GetKeyDown(KeyCode.Alpha9))
            stateMachine.AddEvent(new SCInternalEvent("Event_9"));
    }


    public void ActivityPrint(object sender, SCEventArgs args)
    {
        Debug.Log(sender + " sent event to " + this + " from " + args.Source + " with type " + args.Type);
    }
}
