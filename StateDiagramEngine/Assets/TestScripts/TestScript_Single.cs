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

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
                stateMachine.SetProperty("Property_0", !stateMachine.GetProperty("Property_0"));
            if (Input.GetKeyDown(KeyCode.Alpha1))
                stateMachine.SetProperty("Property_1", !stateMachine.GetProperty("Property_1"));
            if (Input.GetKeyDown(KeyCode.Alpha2))
                stateMachine.SetProperty("Property_2", !stateMachine.GetProperty("Property_2"));
            if (Input.GetKeyDown(KeyCode.Alpha3))
                stateMachine.SetProperty("Property_3", !stateMachine.GetProperty("Property_3"));
            if (Input.GetKeyDown(KeyCode.Alpha4))
                stateMachine.SetProperty("Property_4", !stateMachine.GetProperty("Property_4"));
            if (Input.GetKeyDown(KeyCode.Alpha5))
                stateMachine.SetProperty("Property_5", !stateMachine.GetProperty("Property_5"));
            if (Input.GetKeyDown(KeyCode.Alpha6))
                stateMachine.SetProperty("Property_6", !stateMachine.GetProperty("Property_6"));
            if (Input.GetKeyDown(KeyCode.Alpha7))
                stateMachine.SetProperty("Property_7", !stateMachine.GetProperty("Property_7"));
            if (Input.GetKeyDown(KeyCode.Alpha8))
                stateMachine.SetProperty("Property_8", !stateMachine.GetProperty("Property_8"));
            if (Input.GetKeyDown(KeyCode.Alpha9))
                stateMachine.SetProperty("Property_9", !stateMachine.GetProperty("Property_9"));
        }
        else
        {
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
    }


    public void ActivityPrint(object sender, ActionArgs args)
    {
        Debug.Log(sender + " sent event to " + this + " from " + args.Source + " with type " + args.Type);
    }
}
