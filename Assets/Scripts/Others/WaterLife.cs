using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLife : MonoBehaviour {

    private DateTime startTime;

	// Use this for initialization
	void Start () {
        startTime = DateTime.Now;
	}
	
	// Update is called once per frame
	void Update () {
		if(DateTime.Now > startTime.AddSeconds(1))
        {
            GetComponent<ParticleSystem>().enableEmission = false;
        }
        if(DateTime.Now > startTime.AddSeconds(5))
        {
            Destroy(gameObject);
        }
	}
}
