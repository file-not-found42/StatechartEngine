using UnityEngine;

public class TestScript_SingleRandom : MonoBehaviour
{
    readonly static string[] event_types = {"Event_1", "Event_2", "Event_3", "Event_4", "Event_5", "Event_6"};
    readonly static string[] properties = { "is_1", "is_2", "is_3", "is_4", "is_5", "is_6", "is_7", "is_8", "isNot_1", "isNot_2", "isNot_4" };
    readonly static System.Random rand = new System.Random();

    [SerializeField]
    long event_count = 0;
    [SerializeField]
    long property_count = 12;

    StatechartInstance stateMachine = null;

    void Start()
    {
        stateMachine = GetComponent<StatechartInstance>();

        var args = System.Environment.GetCommandLineArgs();
        
        for (int i = 0; i < args.Length - 1; i++)
            if (args[i] == "--event_count")
                try
                {
                    event_count = long.Parse(args[i + 1]);
                }
                catch (System.Exception)
                {
                    Debug.LogError("Invalid number format for the argument '--event_count'");
                    return;
                }

        for (int i = 0; i < args.Length - 1; i++)
            if (args[i] == "--property_count")
                try
                {
                    property_count = long.Parse(args[i + 1]);
                }
                catch (System.Exception)
                {
                    Debug.LogError("Invalid number format for the argument '--property_count'");
                    return;
                }
    }


    void Update()
    {
        for (int i = 0; i < event_count; i++)
        {
            int iEvent = rand.Next(event_types.Length);
            stateMachine.AddEvent(new SCEvent(event_types[iEvent]));
        }

        for (int i = 0; i < property_count; i++)
        {
            int iProp = rand.Next(properties.Length);
            stateMachine.SetProperty(properties[iProp], !stateMachine.GetProperty(properties[iProp]));
        }
    }
}
