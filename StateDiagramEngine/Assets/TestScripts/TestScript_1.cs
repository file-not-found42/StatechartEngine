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

        stateMachine.Subscribe(new Action("RA", Action.Type.STAY), new ActionDelegate(ActivityPrintA));
    }


    private void Update()
    {
        //if (Input.GetButtonDown("Fire1"))
        //    stateMachine.Step();
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
