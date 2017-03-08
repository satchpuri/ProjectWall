using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Seeker : Vehicle {

	public GameObject seekerTarget;
    public float seekWeight = 40.0f;
    public float safeDistance = 5.0f;
    private float startSafe;
    public float avoidanceWeight = 15f;
    public float wallAvoidanceWeight = 40f;
    public float allignWeight = 10.0f;
    public float cohesionWeight = 10.0f;
    public float seperationWeight = 20f;
    public float boundsWeight = 20f;
    public Vector3 myDirection;
	Vector3 steeringForce;
    private float oldAllign;
    private float olddCohesion;
    public Vector3 wall;
	

	override public void Start () {
		base.Start();
        startSafe = safeDistance;
		steeringForce = Vector3.zero;
        oldAllign = allignWeight;
        olddCohesion = cohesionWeight;
	}

    override public void Update()
    {
        base.Update();
        safeDistance = startSafe + this.Velocity.magnitude/2;
        CheckNeighbors();
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (!flock.Contains(neighbors[i]))
            {
                flock.AddFlocker(neighbors[i]);
            }
        }
        myDirection = Vector3.Cross(Vector3.Cross(GetGroundAngle(), velocity), GetGroundAngle());
        charControl.Move(myDirection * Time.deltaTime);
        charControl.Move(new Vector3(0, -this.gravity * this.mass, 0));
        transform.forward = Vector3.Lerp(transform.forward, myDirection.normalized, Time.deltaTime*5);
    }
	protected override void CalcSteeringForces() {
		steeringForce = Vector3.zero;
        steeringForce += seekWeight * Wander(10, 25);
        if (CheckForWall() != Vector3.zero)
        {
            oldAllign = allignWeight;
            olddCohesion = cohesionWeight;
            allignWeight = 0;
            cohesionWeight = 0;
            wall = CheckForWall();
        }
        else
        {
            allignWeight = oldAllign;
            cohesionWeight = olddCohesion;
        }
        if (wall != Vector3.zero)
        {
            if (Vector3.Distance(wall, this.transform.position) < safeDistance*2)
            {
                steeringForce += wallAvoidanceWeight * Evade(wall);
            }
            else
            {
                wall = Vector3.zero;
            }
        }
		steeringForce += allignWeight * Allignment();
		steeringForce += cohesionWeight * Cohesion();
        steeringForce += seperationWeight * Separation(5);
        steeringForce += boundsWeight * StayWithinRadius(new Vector3(0,50, 0), 200);
		for (int i =0; i < gm.GetComponent<GameManager>().Obstacles.Count; i++) { 
			steeringForce += avoidanceWeight * AvoidObstacle (gm.GetComponent<GameManager>().Obstacles[i], safeDistance);
		}
		steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
		ApplyForce(steeringForce);
    }
}
