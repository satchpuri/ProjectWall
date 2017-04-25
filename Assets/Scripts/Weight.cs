using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weight : MonoBehaviour {

    public Color paint;
    public int str;
	// Use this for initialization
	void Start () {
        this.GetComponent<Renderer>().material.color = paint;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
