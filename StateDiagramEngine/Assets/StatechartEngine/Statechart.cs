using System.Collections.Generic;
using System.Xml;
using System.Linq;
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

    enum Valid
    {
        Active,
        Inactive,
        Error
    }

    [SerializeField]
    TextAsset scxml = null;
    [SerializeField]
    Mode mode = Mode.Manual;
    [SerializeField] [Range(1, 32)]
    uint stepSize = 1;
    
    Status initial;

    List<CompactNode> nodes;
    List<int> relations;
    List<CompactTransition> transitions;
    List<string> node_to_name;
    List<string> transition_to_name;
    Dictionary<string, int> name_to_node;

    public Status Instantiate()
    {
        if (nodes == null)
        {
            LoadStatechart();

            initial = new Status(this, new HashSet<int>(), new Dictionary<string, bool>(), new HashSet<SCEvent>());

            var start = TryEnter(0, initial);
            if (start == (null, null))
                throw new System.Exception("The statechart in " + scxml + " could not be initialized.");

            initial.b_configuration.UnionWith(start.destinations);
        }

        return new Status(initial);
    }


    public (ISet<int> destinations, ISet<long> waypoints) TryExit(int node, Status snap)
    {
        if (node != 0)
        {
            var next = TryExit(nodes[node].superstate, snap);
            if (next != (null, null))
                return next;
        }

        for (int i = nodes[node].transitions; 
            i < nodes[node+1].transitions; 
            i++)
        {
            var next = TryThrough(i, snap);
            if (next != (null, null))
                return next;
        }

        return (null, null);
    }


    public (HashSet<int> destinations, HashSet<long> waypoints) TryEnter(int node, Status snap)
    {
        switch (nodes[node].type)
        {
            case CompactNode.Type.Compound:
                return TryEnter(nodes[node].data, snap);
            case CompactNode.Type.Parallel:
                var result = (new HashSet<int>(), new HashSet<long>());
                for (int i = nodes[node].components; 
                    i < nodes[node+1].components; 
                    i++)
                {
                    var next = TryEnter(relations[i], snap);

                    if (next == (null, null))
                        return (null, null);

                    result.Item1.UnionWith(next.destinations);
                    result.Item2.UnionWith(next.waypoints);
                }
                return result;
            case CompactNode.Type.Basic:
                return (new HashSet<int>() { node }, new HashSet<long>());
            case CompactNode.Type.Pseudo:
                for (int i = nodes[node].transitions;
                    i < nodes[node+1].transitions;
                    i++)
                {
                    var next = TryThrough(i, snap);
                    if (next != (null, null))
                    {
                        next.waypoints.Add(node);
                        return next;
                    }
                }
                return (null, null);
            default:
                return (null, null);
        }

    }


    public (HashSet<int> destinations, HashSet<long> waypoints) TryThrough(int trans, Status snap)
    {
        bool active = (
                   transitions[trans].guard == null 
                || transitions[trans].guard.Evaluate(snap))
            && 
                  (transitions[trans].trigger == SCEvent.emptyEvent 
                || snap.ContainsEvent(transitions[trans].trigger));

        if (!active)
            return (null, null);

        var next = TryEnter(transitions[trans].destination, snap);

        if (next != (null, null))
            next.waypoints.Add(trans + (long)int.MaxValue);

        return next;
    }


    public List<int> GetSuperstates(int state, int limit = -1)
    {
        if (state == limit)
            return new List<int> { };
        else if (state == 0)
            return new List<int> { state };
        else
        {
            var result = GetSuperstates(nodes[state].superstate, limit);
            result.Add(state);
            return result;
        }
    }


    public int GetCommonSuperstate(List<int> states)
    {
        int lowest = states.Max();
        states.RemoveAll(i => i == lowest);
        
        if (states.Count == 0)
            return lowest;
        
        states.Add(nodes[lowest].superstate);

        return GetCommonSuperstate(states);
    }


    public CompactNode.Type GetNodeType(int node)
    {
        return nodes[node].type;
    }


    public string GetNodeName(int node)
    {
        return node_to_name[node];
    }


    public string GetElementName(long element)
    {
        if (element >= int.MaxValue)
            return transition_to_name[(int)(element - int.MaxValue)];
        else
            return node_to_name[(int)element];
    }


    public int GetNodeByName(string name)
    {
        if (name_to_node.TryGetValue(name, out int index))
            return index;
        else
            return -1;
    }


    public ISet<int> GetNodeComponents(int node)
    {
        var result = new HashSet<int>();

        for (int i = nodes[node].components; i < nodes[node + 1].components; i++)
            result.Add(relations[i]);

        return result;
    }


    public Mode GetMode()
    {
        return mode;
    }


    public uint GetStepSize()
    {
        return stepSize;
    }


    public bool IsValid(Status status)
    {
        return IsValidInternal(0, status) == Valid.Active;
    }


    private Valid IsValidInternal(int subtree, Status status)
    {
        long count = 0;
        switch (nodes[subtree].type)
        {
            case CompactNode.Type.Compound:
                for (int i = nodes[subtree].components; i < nodes[subtree+1].components; i++)
                    switch (IsValidInternal(relations[i], status))
                    {
                        case Valid.Active:
                            count++;
                            break;
                        case Valid.Error:
                            return Valid.Error;
                    }

                if (count == 0)
                    return Valid.Inactive;
                else if (count == 1)
                    return Valid.Active;
                else
                    return Valid.Error;
            case CompactNode.Type.Parallel:
                for (int i = nodes[subtree].components; i < nodes[subtree + 1].components; i++)
                    switch (IsValidInternal(relations[i], status))
                    {
                        case Valid.Active:
                            count++;
                            break;
                        case Valid.Error:
                            return Valid.Error;
                    }

                if (count == 0)
                    return Valid.Inactive;
                else if (count == nodes[subtree + 1].components - nodes[subtree].components)
                    return Valid.Active;
                else
                    return Valid.Error;
            case CompactNode.Type.Basic:
                return status.b_configuration.Contains(subtree) ? Valid.Active : Valid.Inactive;
            default:
                return Valid.Error;
        }
    }


    //###############################################################
    // Loading Methods
    //###############################################################


    void LoadStatechart()
    {
        // Prepare
        var doc = new XmlDocument();

        // Allocate data structures
        nodes = new List<CompactNode>();
        transitions = new List<CompactTransition>();
        relations = new List<int>();
        node_to_name = new List<string>();
        name_to_node = new Dictionary<string, int>();
        transition_to_name = new List<string>();

        // Load XML into custom RAM structure
        doc.LoadXml(scxml.text);
        ParseStates(doc.LastChild);
        ParseRelations(doc.LastChild);
        ParseTransitions(doc.LastChild);

        // Add closing state to nodes
        nodes.Add(new CompactNode(CompactNode.Type.Error, -1, 0, 0, 0));

        // Minimize
        nodes.TrimExcess();
        transitions.TrimExcess();
        relations.TrimExcess();
        node_to_name.TrimExcess();
        transition_to_name.TrimExcess();
    }


    void ParseStates(XmlNode node)
    {
        if (node.Name == "state" 
            || node.Name == "parallel" 
            || node.Name == "pseudo")
        {
            name_to_node[node.Attributes["id"].Value] = nodes.Count;
            nodes.Add(new CompactNode(CompactNode.Type.Error, -1, -1, -1, -1));

            foreach (XmlNode n in node.ChildNodes)
                ParseStates(n);
        }
        else if (node.Name == "transition")
        { } // Do nothing for now
        else
        {
            Debug.LogError("Error: Unsupported XML name in statechart document: \"" + node.Name + "\"");
        }
    }


    void ParseRelations(XmlNode node)
    {
        if (node.Name == "state"
            || node.Name == "parallel"
            || node.Name == "pseudo")
        {
            var state_name = node.Attributes["id"].Value;
            int index = name_to_node[state_name];

            // Superstate
            int superstate = index == 0 ? -1 : name_to_node[node.ParentNode.Attributes["id"].Value];
            
            // Components
            int components_start = relations.Count;
            foreach (XmlNode n in node.ChildNodes)
                if (n.Name == "state" || n.Name == "parallel")
                {
                    if (n.Attributes["id"] == null)
                        Debug.LogError("Attribute \"id\" missing in node " + node);
                    var comp_name = n.Attributes["id"].Value;
                    relations.Add(name_to_node[comp_name]);
                }

            // Data (default state)
            int data = 0;

            // Type
            CompactNode.Type type = CompactNode.Type.Error;
            if (components_start == relations.Count)
            {
                if (node.Name == "state")
                    type = CompactNode.Type.Basic;
                else if (node.Name == "pseudo")
                    type = CompactNode.Type.Pseudo;
            }
            else
            {
                if (node.Name == "state")
                {
                    type = CompactNode.Type.Compound;

                    if (node.Attributes["initial"] == null)
                        Debug.LogError("Missing default state of " + state_name + " in " + scxml.name);
                    if (!name_to_node.TryGetValue(node.Attributes["initial"].Value, out data))
                        Debug.LogError("The default state of " + state_name + " in " + scxml.name + " does not exist.");
                }
                else if (node.Name == "parallel")
                    type = CompactNode.Type.Parallel;
            }

            // Name
            if (superstate >= 0)
                node_to_name.Add(node_to_name[superstate] + "." + state_name);
            else
                node_to_name.Add(state_name);
            name_to_node[node_to_name[index]] = index;
            
            nodes[index] = new CompactNode(type, superstate, components_start, data, 0);
            
            foreach (XmlNode n in node.ChildNodes)
                ParseRelations(n);
        }
        else if (node.Name == "transition")
        { }
        else
        {
            Debug.LogError("Error: Unsupported XML name in statechart document: \"" + node.Name + "\"");
        }
    }

    void ParseTransitions(XmlNode node)
    {
        if (node.Name == "state"
            || node.Name == "parallel"
            || node.Name == "pseudo")
        {
            int index = name_to_node[node.Attributes["id"].Value];

            // Transitions
            int transitions_start = transitions.Count;

            nodes[index] = new CompactNode(nodes[index].type, 
                nodes[index].superstate, 
                nodes[index].components, 
                nodes[index].data, 
                transitions_start);

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

            var trigger = new SCEvent(node.Attributes["event"].Value);

            Guard guard = null;
            if (node.Attributes["cond"] != null)
                guard = new Guard(node.Attributes["cond"].Value);

            int destination = name_to_node[node.Attributes["target"].Value];

            transition_to_name.Add(
                node_to_name[name_to_node[node.ParentNode.Attributes["id"].Value]]
                + "->"
                + node_to_name[destination]);
            transitions.Add(new CompactTransition(trigger, guard, destination));
        }
        else
        {
            Debug.LogError("Error: Unsupported XML name in statechart document: \"" + node.Name + "\"");
        }
    }
}
