using UnityEngine;

public class TestScript_SingleRandom : MonoBehaviour
{
    readonly string[] event_types = {"Event_1", "Event_2", "Event_3"};
    readonly string[] properties = { "Property_1", "Property_2", "Property_3" };

    StatechartInstance stateMachine = null;


    void Start()
    {
        stateMachine = GetComponent<StatechartInstance>();
    }


    void Update()
    {
        int numEvents = Random.Range(0, event_types.Length * 2);
        for (int i = 0; i < numEvents; i++)
        {
            int iEvent = Random.Range(0, event_types.Length);
            stateMachine.AddEvent(new SCEvent(event_types[iEvent]));
        }

        int numProps = Random.Range(0, properties.Length * 2);
        for (int i = 0; i < numProps; i++)
        {
            int iProp = Random.Range(0, properties.Length);
            stateMachine.SetProperty(properties[iProp], !stateMachine.GetProperty(properties[iProp]));
        }
    }
}
