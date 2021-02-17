using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

//[CreateAssetMenu(fileName = "Machine", menuName = "StateMachine", order = 3)]
public class Statechart : ScriptableObject
{
    public enum Mode
    {
        Manual,
        On_Event,
        Unity_Update,
        Unity_Late_Update,
        Unity_Fixed_Update,
        Unity_On_Gui
    }

    [SerializeField]
    TextAsset scxml = null;
    [SerializeField]
    Mode mode = Mode.Manual;

    //[SerializeField]
    readonly List<Node> states = new List<Node>();
    //[SerializeField]
    readonly List<Transition> transitions = new List<Transition>();
    //[SerializeField]
    State root = null;

    
    public Configuration Instantiate()
    {
        if (root == null)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(scxml.text);

            ParseStates(doc.LastChild);
            ParseTransitions(doc.LastChild);
        }
        
        Configuration config = new Configuration();
        config.atomicState.UnionWith(root.Enter());
        return config;
    }


    void ParseStates(XmlNode node)
    {
        if (node.Name == "state")
        {
            // Test if the state is an atomic state
            bool isAtomic = true;
            if (node.HasChildNodes)
                foreach (XmlNode n in node.ChildNodes)
                    if (n.Name == "state" || n.Name == "parallel")
                        isAtomic = false;

            // Create the new state
            State state;
            if (isAtomic)
                state = new AtomicState(node.Attributes["id"].Value);
            else
                state = new CompoundState(node.Attributes["id"].Value);
            
            if (root == null)
                root = state;
            else
                state.parent = GetState(node.ParentNode.Attributes["id"].Value);

            states.Add(state);

            // Recursion
            if (node.HasChildNodes)
                foreach (XmlNode n in node.ChildNodes)
                    ParseStates(n);

            // Set the entry state for a compound state
            if (!isAtomic)
            {
                string entryState = node.Attributes["initial"].Value;
                ((CompoundState)state).entryChild = GetState(entryState);
            }
        }
        else if (node.Name == "parallel")
        {
            ParallelState state = new ParallelState(node.Attributes["id"].Value)
            {
                parent = GetState(node.ParentNode.Attributes["id"].Value)
            };

            states.Add(state);

            if (node.HasChildNodes)
            {
                foreach (XmlNode n in node.ChildNodes)
                    ParseStates(n);

                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n.Name == "transition")
                        continue;

                    string region = n.Attributes["id"].Value;
                    state.regions.Add((CompoundState)GetState(region));
                }
            }

        }
        else if (node.Name == "transition")
        { } // Do nothing for now
        else
        {
            Debug.LogError("Error: Unsupported XML name in statechart document: \"" + node.Name + "\"");
        }
    }


    public void ParseTransitions(XmlNode node)
    {
        if (node.Name == "state" || node.Name == "parallel")
        {
            if (node.HasChildNodes)
                foreach (XmlNode n in node.ChildNodes)
                    ParseTransitions(n);
        }
        else if (node.Name == "transition")
        {
            if (node.Attributes["priority"] == null)
                Debug.LogError("Error: Missing priority in transition in " + scxml.name);
            if (node.Attributes["target"] == null)
                Debug.LogError("Error: Missing target in transition in " + scxml.name);
            if (node.Attributes["event"] == null)
                Debug.LogError("Error: Missing trigger in transition in " + scxml.name);

            Node source = GetState(node.ParentNode.Attributes["id"].Value);
            Node target = GetState(node.Attributes["target"].Value);

            Transition trans = new Transition(source.ToString() + "->" + target.ToString(), target)
            {
                trigger = new SCInternalEvent(node.Attributes["event"].Value)
            };

            if (node.Attributes["cond"] != null)
                trans.cond = new Condition(node.Attributes["cond"].Value);

            source.outTransitions.Add(int.Parse(node.Attributes["priority"].Value), trans);
        }
    }


    public Mode GetMode()
    {
        return mode;
    }


    public List<Node> GetStates()
    {
        return states;
    }


    public State GetState(string name)
    {
        return (State)states.Find(new System.Predicate<Node>(n => n.name == name));
    }


    public List<Transition> GetTransitions()
    {
        return transitions;
    }
}
