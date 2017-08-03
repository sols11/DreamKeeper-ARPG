using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class BTtest : MonoBehaviour {

    public BehaviorTree bt;

	void Start () {
        bt=GetComponent<BehaviorTree>();
        InvokeRepeating("TestFunc", 0.1f,0.1f);
	}
	
    void TestFunc()
    {
        bt.SendEvent<int>("Hit",1);
    }

    void Update () {
        //bt.SendEvent("Hit");
    }
}
