﻿
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//add using System.Collections.Generic; to use the generic list format
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public GameObject target;
    public Terrain terrain;

    public int numberObstacles;

    public GameObject flockerPrefab;

    public GameObject targetPrefab;

    public List<GameObject> flockers;

    private List<Flock> flocks;

    public List<Flock> Flocks { get { return flocks; } }

    public int numberOfFlockers;

    private List<GameObject> obstacles;

    public List<GameObject> Obstacles
    {
        get { return obstacles; }
    }

    public Text allignTxt;
    public Text cohesionTxt;
    public Text seperationTxt;

    public bool Cohesion = true;
    public bool Allign = true;
    public bool Seperation = true;

    void Start () {      
        flocks = new List<Flock>();     
        for (int i = 0; i < numberOfFlockers; i++)
        {
            int posX = Random.Range((int)terrain.transform.position.x, (int)terrain.transform.position.x + 500);
            int posZ = Random.Range((int)terrain.transform.position.z, (int)terrain.transform.position.z + 500);
            Vector3 pos = new Vector3(posX, terrain.SampleHeight(new Vector3(posX, 0, posZ))+1, posZ);
            GameObject flocker = (GameObject)GameObject.Instantiate(flockerPrefab, pos, Quaternion.identity);
            flocker.GetComponent<Seeker>().seekerTarget = target;
            flockers.Add(flocker);
        }
        obstacles = new List<GameObject>();
        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Rock"))
        {
            obstacles.Add(obstacle);
        }
        allignTxt.text = Allign.ToString();
        cohesionTxt.text = Cohesion.ToString();
        seperationTxt.text = Seperation.ToString();
    }

	void Update () {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Allign = !Allign;
            allignTxt.text = Allign.ToString();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Cohesion = !Cohesion;
            cohesionTxt.text = Cohesion.ToString();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Seperation = !Seperation;
            seperationTxt.text = Seperation.ToString();
        }
    }
}
