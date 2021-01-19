using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Machine", menuName = "StateMachine", order = 3)]
public class Statechart : ScriptableObject
{
    public enum Mode
    {
        MANUAL,
        UPDATE,
        LATE_UPDATE,
        FIXED_UPDATE,
        ON_GUI
    }

    [SerializeField]
    List<State> states = new List<State>();
    [SerializeField]
    List<Transition> transitions = new List<Transition>();
    [SerializeField]
    State root = null;
    [SerializeField]
    Mode mode = Mode.MANUAL;

    public Statechart()
    {
        // TEST CODE BEGIN
        State r = new State("R");
        root = r;
        states.Add(r);
        State a = new State("A")
        {
            stayAction = new Activity
            {
                text = "TestScript_1:ActivityPrintA"
            }
        };
        states.Add(a);
        State b = new State("B");
        states.Add(b);

        b.parent = r;
        a.parent = r;
        r.def = a;

        Transition ab = new Transition(b);
        ab.cond = new Condition
        {
            property = "TestScript_1:ConditionManual"
        };
        transitions.Add(ab);
        Transition ba = new Transition(a);
        transitions.Add(ba);

        a.outTransitions.Add(ab);
        b.outTransitions.Add(ba);
        // TEST CODE END
    }

    
    public Configuration Instantiate()
    {
        Configuration config = new Configuration
        {
            atomicState = root.def
        };
        return config;
    }


    public Mode GetMode()
    {
        return mode;
    }
}
