using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript_1 : MonoBehaviour
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
            stateMachine.AddEvent(new SCInternalEvent("Event_1"));
        if (Input.GetKeyDown(KeyCode.Alpha2))
            stateMachine.AddEvent(new SCInternalEvent("Event_2"));
    }


    public void ActivityPrint(object sender, SCEventArgs args)
    {
        Debug.Log(sender + " sent event to " + this + " from " + args.Source + " with type " + args.Type);
    }
}
