using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour {

    private float ZoomAmount = 0; //With Positive and negative values
private float MaxToClamp =50;
private float ROTSpeed = 10;

void Update(){

    ZoomAmount += Input.GetAxis("Mouse ScrollWheel");
    ZoomAmount = Mathf.Clamp(ZoomAmount, -MaxToClamp, MaxToClamp);
    var translate = Mathf.Min(Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")), MaxToClamp  - Mathf.Abs(ZoomAmount));
    gameObject.transform.Translate(0,0,translate * ROTSpeed * Mathf.Sign(Input.GetAxis("Mouse ScrollWheel")));

    }
}

