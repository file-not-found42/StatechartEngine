using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript_Many : MonoBehaviour
{
    [SerializeField]
    Statechart statechart;
    [SerializeField]
    long instanceCount = 1;


    void Start()
    {
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
