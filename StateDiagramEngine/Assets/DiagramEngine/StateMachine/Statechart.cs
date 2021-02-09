using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

//[CreateAssetMenu(fileName = "Machine", menuName = "StateMachine", order = 3)]
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
    TextAsset scxml = null;
    [SerializeField]
    Mode mode = Mode.MANUAL;

    //[SerializeField]
    readonly List<Node> states = new List<Node>();
    //[SerializeField]
    readonly List<Transition> transitions = new List<Transition>();
    //[SerializeField]
    CompoundState root = null;

    
    public Configuration Instantiate()
    {
        if (root == null)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(scxml.text);

            ReadXMLNode(doc);
        }
        
        // TODO
        Configuration config = new Configuration();
        //config.atomicState.Add(root.entryChild);
        return config;
    }


    void ReadXMLNode(XmlNode node)
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
                state = new AtomicState(node.Attributes.GetNamedItem("id").Value);
            else
                state = new CompoundState(node.Attributes.GetNamedItem("id").Value);

            state.parent = GetState(node.ParentNode.Attributes.GetNamedItem("id").Value);
            
            states.Add(state);

            // Recursion
            if (node.HasChildNodes)
                foreach (XmlNode n in node.ChildNodes)
                    ReadXMLNode(n);

            // Set the entry state for a compound state
            if (!isAtomic)
            {
                string entryState = node.Attributes.GetNamedItem("initial").Value;
                ((CompoundState)state).entryChild = GetState(entryState);
            }
        }
        else if (node.Name == "parallel")
        {
            ParallelState state = new ParallelState(node.Attributes.GetNamedItem("id").Value)
            {
                parent = GetState(node.ParentNode.Attributes.GetNamedItem("id").Value)
            };

            states.Add(state);

            if (node.HasChildNodes)
            {
                foreach (XmlNode n in node.ChildNodes)
                    ReadXMLNode(n);

                foreach (XmlNode n in node.ChildNodes)
                {
                    string region = n.Attributes.GetNamedItem("id").Value;
                    state.regions.Add((CompoundState)GetState(region));
                }
            }

        }
        else if (node.Name == "transition")
        {
            Node source = GetState(node.ParentNode.Attributes.GetNamedItem("id").Value);
            Node target = GetState(node.ParentNode.Attributes.GetNamedItem("target").Value);

            source.outTransitions.Add(
                int.Parse(node.ParentNode.Attributes.GetNamedItem("cond").Value), 
                new Transition(source.name + "->" + target.name, target)
                {
                    trigger = new SCEvent(node.ParentNode.Attributes.GetNamedItem("event").Value),
                    cond = new Condition(node.ParentNode.Attributes.GetNamedItem("cond").Value)
                }
            );

        }
        else if (node.Name == "#document")
        {
            foreach (XmlNode n in node.ChildNodes)
                ReadXMLNode(n);
        }
        else if (node.Name == "scxml") { } // Ignore
        else
        {
            Debug.LogError("Error: Unsupported XML name in statechart document: \"" + node.Name + "\"");
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
