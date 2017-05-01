using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddObjects : MonoBehaviour {

    [SerializeField] GameObject yellow;
    [SerializeField]
    GameObject white;
    [SerializeField]
    GameObject blue;
    [SerializeField]
    GameObject black;
    [SerializeField]
    GameObject camera;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp("g"))
        {
            Instantiate(yellow, camera.transform.position, transform.rotation) ;
        }
        if (Input.GetKeyUp("h"))
        {
            Instantiate(white, camera.transform.position, transform.rotation);
        }
        if (Input.GetKeyUp("j"))
        {
            Instantiate(blue, camera.transform.position, transform.rotation);
        }
        if (Input.GetKeyUp("k"))
        {
            Instantiate(black, camera.transform.position, transform.rotation);
        }
    }
}
