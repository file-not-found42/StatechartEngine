using UnityEngine;

public class TestScript_Many : MonoBehaviour
{
    [SerializeField]
    Statechart statechart;
    [SerializeField]
    long instanceCount = 1;


    void Start()
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length - 1; i++)
            if (args[i] == "--instance_count")
                try
                {
                    instanceCount = long.Parse(args[i + 1]);
                }
                catch (System.Exception)
                {
                    Debug.LogError("Invalid number format for the argument '--instance_count'");
                    return;
                }

        for (long i = 0; i < instanceCount; i++)
        {
            var go = new GameObject();

            go.SetActive(false);

            var instance = go.AddComponent<StatechartInstance>();
            instance.Initialize(statechart);

            go.AddComponent<TestScript_SingleRandom>();

            go.SetActive(true);
        }
    }
}
