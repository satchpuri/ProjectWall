using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flock : MonoBehaviour {
    private List<GameObject> flockers;
    private Vector3 centroid;

    public Vector3 Centroid { get { return centroid; } }

    private Vector3 flockDirection;
    int maxFlockDistance = 15;
    public Vector3 FlockDirection { get { return flockDirection; } }
	// Use this for initialization
	void Start () {
        flockers = new List<GameObject>();   
	}
	
	// Update is called once per frame
	void Update () {
        CalcCentroid();
        CalcFlockDirection();
	}

    public void AddFlocker(GameObject go)
    {
        if (flockers != null)
        {
            flockers.Add(go);
        }
    }

    public bool Contains(GameObject go)
    {
        if (flockers != null)
        {
            if (flockers.Contains(go))
            {
                return true;
            }
            else return false;
        }
        else return false;
    }

    public void CalcCentroid()
    {
        Vector3 newCentroid = Vector3.zero;
        for (int i = 0; i < flockers.Count; i++)
        {
            if (flockers[i] == null)
            {
                flockers.Remove(flockers[i]);
                return;
            }
            if (Vector3.Distance(flockers[i].transform.position, centroid) > maxFlockDistance){
                flockers.Remove(flockers[i]);
                return;
            }
            else {
                newCentroid += flockers[i].transform.position;
            }
        }
        centroid = newCentroid;
        centroid = centroid / flockers.Count;
        centroid.y = Terrain.activeTerrain.SampleHeight(new Vector3(centroid.x, 0, (centroid.z)));
    }

    public void CalcFlockDirection()
    {
        if (flockers.Count > 0)
        {
            Vector3 velocitySum = Vector3.zero;
            for (int i = 0; i < flockers.Count; i++)
            {
                if (flockers[i] == null)
                {
                    flockers.Remove(flockers[i]);
                    return;
                }
                velocitySum += flockers[i].GetComponent<Vehicle>().Velocity;
            }
            flockDirection = velocitySum;
        }
    }

}
