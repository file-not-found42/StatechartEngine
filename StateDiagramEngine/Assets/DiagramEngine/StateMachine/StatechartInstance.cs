using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StatechartInstance : MonoBehaviour
{
    List<SCEvent> events = new List<SCEvent>();
    List<SCEvent> newEvents = new List<SCEvent>();
    
    readonly Dictionary<string, ActivityFunction> activities = new Dictionary<string, ActivityFunction>();
    readonly Dictionary<string, bool> conditions = new Dictionary<string, bool>();

    [SerializeField]
    Statechart machine;

    Configuration config;


    void Awake()
    {
        config = machine.Instantiate();
    }


    void OnEnable()
    {
        StatechartEngine.AddInstance(this);
    }


    void OnDisable()
    {
        StatechartEngine.RemoveInstance(this);
    }


    public void Step()
    {
        events = newEvents;
        newEvents = new List<SCEvent>();

        State source = config.atomicState;

        Transition trans = source.Step(this, events);

        if (trans == null)
        {
            source.Stay(this);
            return;
        }

        State dest = trans.destination;

        source.Exit(this);
        trans.Traverse(this);
        dest.Entry(this);

        config.atomicState = dest;

        Debug.Log(this + " is now in " + config.ToString());
    }


    public bool CheckCondition(Condition c)
    {
        return conditions[c.property];
    }


    public void DoActivity(Activity a)
    {
        activities.TryGetValue(a.text, out ActivityFunction act);
        act();
    }

    
    public void AddEvent(SCEvent e)
    {
        newEvents.Add(e);
    }


    public void Subscribe()
    {

    }


    public void Unsubscribe()
    {

    }


    public void SetProperty()
    {

    }


    public Statechart.Mode GetMode()
    {
        return machine.GetMode();
    }
}
