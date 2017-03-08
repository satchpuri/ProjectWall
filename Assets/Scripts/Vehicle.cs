using UnityEngine;
using System.Collections;

//use the Generic system here to make use of a Flocker list later on
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

abstract public class Vehicle : MonoBehaviour {

	public CharacterController charControl;

	protected Vector3 acceleration;
	protected Vector3 velocity;
	public Vector3 Velocity {
		get { return velocity; }
	}

	public float maxSpeed = 6.0f;
	public float maxForce = 12.0f;
	public float radius = 1.0f;
	public float mass = 1.0f;
	public float gravity = 20.0f;
    public float neighborhood = 50f;

    float wanderAngle;
	protected GameManager gm;

    public List<GameObject> neighbors;

    public Flock flock;

	virtual public void Start(){
		acceleration = Vector3.zero;
		velocity = transform.forward;
		charControl = GetComponent<CharacterController>();
		gm = GameObject.Find ("GameManager").GetComponent<GameManager>();
        flock = this.gameObject.AddComponent<Flock>();
	}

    public Vector3 GetGroundAngle()
    {
        RaycastHit frontRay = new RaycastHit();
        Vector3 angle;
        if (Physics.Raycast(this.transform.position - this.transform.up * 1f, -this.transform.up, out frontRay, 3))
        {
            angle = frontRay.normal;
        }
        else
        {
            angle = new Vector3(0, 1, 0);
        }
        return angle;
    }

    public Vector3 CheckForWall()
    {
        RaycastHit frontRay = new RaycastHit();
        Vector3 wallPos = Vector3.zero;
        if (Physics.Raycast(this.transform.position - this.transform.up * 1f, this.transform.forward, out frontRay, 10))
        {
            if (frontRay.collider)
            {
                if (frontRay.collider.CompareTag("Wall"))
                {
                    wallPos = frontRay.point;
                }
            }
        }
        return wallPos;
    }

    public Vector3 GetFrontAngle()
    {
        RaycastHit frontRay = new RaycastHit();
        Vector3 angle;
        if (Physics.Raycast(this.transform.position - this.transform.up * 1f, this.transform.forward, out frontRay, 1))
        {
            angle = frontRay.normal;
        }
        else
        {
            angle = new Vector3(0, 0, 0);
        }
        return angle;
    }

    public RaycastHit GetFrontCast(float dist)
    {
        RaycastHit frontRay = new RaycastHit();
        if (Physics.Raycast(this.transform.position, this.transform.forward, out frontRay, dist))
        {
            return frontRay;
        }
        return frontRay;
    }
	
	// Update is called once per frame
	virtual public void Update () {
		CalcSteeringForces ();
		velocity += new Vector3(acceleration.x, acceleration.y, acceleration.z) * Time.deltaTime;
		velocity = Vector3.ClampMagnitude (velocity, maxSpeed);
        acceleration = Vector3.zero;

	}

	abstract protected void CalcSteeringForces();

	
	protected void ApplyForce(Vector3 steeringForce) {
		acceleration += steeringForce/mass;
	}

    protected Vector3 Seek(Vector3 targetPosition)
    {
		Vector3 force = Vector3.zero;
		Vector3 desired = targetPosition - transform.position;
		desired.Normalize();
		desired = desired * maxSpeed;
		force = desired - velocity;
		force.y = 0;
		return force;
    }

    protected Vector3 Wander(float dist, float d)
    {
        Vector3 force = Vector3.zero;
        Vector3 target = transform.position + (transform.forward * d);
        Vector3 oldTarget = target;
        float rand = Random.Range(-Mathf.PI/5, Mathf.PI/5);
        wanderAngle = wanderAngle + rand;
        Vector3 olddesired = target - transform.position;
        Debug.DrawRay(transform.position, olddesired.normalized * d, Color.white);
        target = new Vector3(target.x + Mathf.Sin(wanderAngle) * dist, target.y, target.z + Mathf.Tan(wanderAngle) * dist);
        Vector3 desired = target - transform.position;
        Debug.DrawRay(this.transform.position + olddesired.normalized * d, target.normalized * dist, Color.red);
        desired.Normalize();
        desired = desired * maxSpeed;
        force = desired - velocity;
        force.y = 0;
        return force;
    }

    protected Vector3 AvoidObstacle(GameObject obst, float safeDistance)
    {
        Vector3 deltaVector = obst.transform.position - transform.position;
        float distToObst = (transform.position - obst.transform.position).magnitude;
        if (distToObst - obst.GetComponent<ObstacleScript>().radius < (safeDistance))
        { //If within safe distance.
            if (Vector3.Dot(transform.forward, deltaVector) > 0)
            { //If ahead of seeker
                if (Vector3.Dot(deltaVector, this.transform.right) < radius + obst.GetComponent<ObstacleScript>().Radius)
                { // distance between centers
                    if (Vector3.Dot(deltaVector, this.transform.right) > 0)
                    { // Steer left or right
                        Vector3 desired = -this.transform.right * maxSpeed;
                        desired = desired.normalized;
                        desired = desired * maxSpeed;
                        Vector3 steer = desired - velocity;
                        return steer;
                    }
                    else
                    {
                        Vector3 desired = this.transform.right * maxSpeed;
                        desired = desired.normalized;
                        desired = desired * maxSpeed;
                        Vector3 steer = desired - velocity;
                        return steer;
                    }
                }
                else
                {
                    return new Vector3(0, 0, 0);
                }
            }
            else
            {
                return new Vector3(0, 0, 0);
            }
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    public void CheckNeighbors()
    {
        if (this.GetComponent<Seeker>())
        {
            foreach (GameObject go in gm.flockers)
            {
                if (go != this.gameObject)
                {
                    if (gameObject == null && !ReferenceEquals(gameObject, null))
                    {
                        if (neighbors.Contains(go))
                        {
                            neighbors.Remove(go);
                        }
                    }
                    else
                    {
                        if (Vector3.Distance(this.transform.position, go.transform.position) < neighborhood)
                        {
                            if (!neighbors.Contains(go))
                            {
                                neighbors.Add(go);
                            }
                        }
                        else
                        {
                            if (neighbors.Contains(go))
                            {
                                neighbors.Remove(go);
                            }
                        }
                    }
                }
            }
        }
    }

    public Vector3 StayWithinRadius(Vector3 target, float radius)
    {
        Vector3 force = Vector3.zero;
        if (Vector3.Distance(this.transform.position, target) > radius)
        {
            force = Seek(target);
        }       
        return force;
    }

    public Vector3 Separation(float seperationDistance)
    {
        Vector3 force = Vector3.zero;
        foreach (GameObject go in neighbors)
        {
            if (go != null)
            {
                if (neighbors.Count > 0)
                {
                    if (Vector3.Distance(go.transform.position, this.transform.position) < seperationDistance)
                    {
                        force += go.transform.position - this.transform.position;
                    }
                }
            }
        }
        force = force.normalized * maxSpeed;
        return new Vector3(-force.x, 0, -force.z);
    }

    public Vector3 Allignment()
    {
        Vector3 force = Vector3.zero;
        foreach (GameObject go in neighbors)
        {
            if (go != null) { 
                if (neighbors.Count > 0)
                {
                    force.x += go.GetComponent<Vehicle>().Velocity.x;
                    force.y += go.GetComponent<Vehicle>().Velocity.y;
                }
            }
        }
        force = force.normalized * maxSpeed;
        force = force - velocity;
        force.y = 0;
        return force - velocity;
    }

    public Vector3 Cohesion()
    {
        if (flock)
        {
            return Seek(flock.Centroid);
        }
        else return Vector3.zero;
    }


    public Vector3 Arrive(float radius, Vector3 target)
    {
        Vector3 force = Vector3.zero;
        force = Seek(target);
        if (Vector3.Distance(this.transform.position, target) < radius)
        {
            force = force * (Vector3.Distance(this.transform.position, target) / radius);
        }
        return force;
    }

    public Vector3 Evade(GameObject go)
    {
        Vector3 force = Vector3.zero;
        Vector3 distance = go.transform.position - this.transform.position;
        Vector3 target = go.transform.position + go.GetComponent<Vehicle>().velocity*distance.magnitude/maxSpeed;
        force = -Seek(target);
        return force;
    }


    public Vector3 Evade(Vector3 pos)
    {
        Vector3 force = Vector3.zero;
        Vector3 distance = pos - this.transform.position;
        force = -Seek(pos);
        return force;
    }


}
