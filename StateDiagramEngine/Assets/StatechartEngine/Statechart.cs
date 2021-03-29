using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[CreateAssetMenu(fileName = "Machine", menuName = "StateMachine", order = 3)]
public class Statechart : ScriptableObject
{
    public enum Mode
    {
        Manual,
        On_Event,
        Unity_Update,
        Unity_Late_Update,
        Unity_Fixed_Update,
    }

    [SerializeField]
    TextAsset scxml = null;
    [SerializeField]
    Mode mode = Mode.Manual;
    [SerializeField] [Range(1, 32)]
    uint stepSize = 1;

    readonly List<Node> states = new List<Node>();
    readonly List<Transition> transitions = new List<Transition>();
    public State Root { get; private set; }

    
    public Configuration Instantiate()
    {
        if (Root == null)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(scxml.text);

            ParseStates(doc.LastChild);
            ParseTransitions(doc.LastChild);
        }
        
        var start = Root.TryEnter(new Status(new Dictionary<string, bool>(), new HashSet<SCEvent>()));
        if (start == (null, null))
            throw new System.Exception("The statechart in " + scxml + " could not be initialized.");

        Configuration config = new Configuration(this, start.destinations);
        return config;
    }


    void ParseStates(XmlNode node)
    {
        if (node.Name == "state" || node.Name == "parallel" || node.Name == "pseudo")
        {
            string name = node.Attributes["id"].Value;
            State parent = Root == null ? null : (State)GetNode(node.ParentNode.Attributes["id"].Value);
            
            if (node.Name == "state")
            {
                // Test if the state is an atomic state
                bool isAtomic = true;
                foreach (XmlNode n in node.ChildNodes)
                    if (n.Name != "transition")
                        isAtomic = false;

                if (isAtomic)
                {
                    AtomicState state = new AtomicState(name, parent);
                    states.Add(state);

                    if (Root == null)
                        Root = state;

                    // Recursion
                    foreach (XmlNode n in node.ChildNodes)
                        ParseStates(n);
                }
                else
                {
                    CompoundState state = new CompoundState(name, parent);
                    states.Add(state);

                    if (Root == null)
                        Root = state;

                    // Recursion
                    foreach (XmlNode n in node.ChildNodes)
                        ParseStates(n);

                    foreach (XmlNode n in node.ChildNodes)
                        if (n.Name == "state" || n.Name == "parallel")
                            state.components.Add((State)GetNode(n.Attributes["id"].Value));

                    // Set the entry state for a compound state
                    if (node.Attributes["initial"] == null)
                        Debug.LogError("Error: Missing default state of " + name + " in " + scxml.name);
                    string entryState = node.Attributes["initial"].Value;
                    var def = GetNode(entryState);
                    if (def == null)
                        Debug.LogError("Error: Default state of " + name + " in " + scxml.name + " does not exist");
                    state.defaultComponent = def;
                }
            }
            else if (node.Name == "parallel")
            {
                ParallelState state = new ParallelState(name, parent);
                states.Add(state);

                if (Root == null)
                    Root = state;

                foreach (XmlNode n in node.ChildNodes)
                    ParseStates(n);

                foreach (XmlNode n in node.ChildNodes)
                    if (n.Name == "state" || n.Name == "parallel")
                        state.components.Add((State)GetNode(n.Attributes["id"].Value));
            }
            else if (node.Name == "pseudo")
            {
                PseudoState pseudo = new PseudoState(name, parent);
                states.Add(pseudo);
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
        if (node.Name == "state" 
            || node.Name == "parallel"
            || node.Name == "pseudo")
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

            Node source = GetNode(node.ParentNode.Attributes["id"].Value);
            Node target = GetNode(node.Attributes["target"].Value);

            Transition trans = new Transition(source.ToString() + "->" + target.ToString(), target)
            {
                trigger = new SCEvent(node.Attributes["event"].Value)
            };

            if (node.Attributes["cond"] != null)
                trans.guard = new Guard(node.Attributes["cond"].Value);

            source.outTransitions.Add(int.Parse(node.Attributes["priority"].Value), trans);
        }
    }


    public Mode GetMode()
    {
        return mode;
    }


    public uint GetStepSize()
    {
        return stepSize;
    }


    Node GetNode(string name)
    {
        return states.Find(new System.Predicate<Node>(n => n.name == name));
    }
}
