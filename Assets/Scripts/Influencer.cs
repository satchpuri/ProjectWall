using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InfType { None, Black, Yellow, Blue, White };
public enum Team { None, Red, Green };

public class Influencer : MonoBehaviour {

    public int strength;    // strength of influence on map
    public InfType type;    // type of unit
    public Team team;       // team unit is on

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
