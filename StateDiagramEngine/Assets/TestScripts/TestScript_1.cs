using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript_1 : MonoBehaviour
{
    public bool manualCondition = false;


    StatechartInstance stateMachine = null;


    private void Start()
    {
        stateMachine = GetComponent<StatechartInstance>();
    }


    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            stateMachine.Step();
    }


    public bool ConditionTrue()
    {
        return true;
    }


    public bool ConditionFalse()
    {
        return false;
    }


    public bool ConditionRandom()
    {
        return Random.value > 0.5f;
    }


    public bool ConditionManual()
    {
        return manualCondition;
    }


    public void ActivityPrintA()
    {
        Debug.Log("Activity A from " + name);
    }

    
    public void ActivityPrintB()
    {
        Debug.Log("Activity B from " + name);
    }
}
