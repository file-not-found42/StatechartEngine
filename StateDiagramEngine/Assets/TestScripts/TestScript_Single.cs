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
            stateMachine.SuperStep();

        if (Input.GetKeyDown(KeyCode.Alpha0))
            stateMachine.AddEvent(new SCEvent("Event_0"));
        if (Input.GetKeyDown(KeyCode.Alpha1))
            stateMachine.AddEvent(new SCEvent("Event_1"));
        if (Input.GetKeyDown(KeyCode.Alpha2))
            stateMachine.AddEvent(new SCEvent("Event_2"));
        if (Input.GetKeyDown(KeyCode.Alpha3))
            stateMachine.AddEvent(new SCEvent("Event_3"));
        if (Input.GetKeyDown(KeyCode.Alpha4))
            stateMachine.AddEvent(new SCEvent("Event_4"));
        if (Input.GetKeyDown(KeyCode.Alpha5))
            stateMachine.AddEvent(new SCEvent("Event_5"));
        if (Input.GetKeyDown(KeyCode.Alpha6))
            stateMachine.AddEvent(new SCEvent("Event_6"));
        if (Input.GetKeyDown(KeyCode.Alpha7))
            stateMachine.AddEvent(new SCEvent("Event_7"));
        if (Input.GetKeyDown(KeyCode.Alpha8))
            stateMachine.AddEvent(new SCEvent("Event_8"));
        if (Input.GetKeyDown(KeyCode.Alpha9))
            stateMachine.AddEvent(new SCEvent("Event_9"));
    }


    public void ActivityPrint(object sender, ActionArgs args)
    {
        Debug.Log(sender + " sent event to " + this + " from " + args.Source + " with type " + args.Type);
    }
}
