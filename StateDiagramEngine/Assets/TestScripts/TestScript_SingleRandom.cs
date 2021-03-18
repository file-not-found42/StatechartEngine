using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript_SingleRandom : MonoBehaviour
{
    readonly string[] event_types = {"Event_1", "Event_2", "Event_3"};

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
            stateMachine.AddEvent(new SCInternalEvent(event_types[iEvent]));
        }
    }
}
