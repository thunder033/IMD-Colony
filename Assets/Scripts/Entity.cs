using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

public abstract class Entity : MonoBehaviour {


	public float maxSpeed = 6.0f;
	public float maxForce = 3.0f;
	public float mass = 1.0f;
	public float radius = 1.0f;
	public float gravity = 20.0f;

	//wander
	float wanderRad = 4.0f;
	float wanderDist = 5.0f;
	float wanderRand = 3.0f;
	float wanderAng = 0.0f;

	protected CharacterController characterController;
	protected Vector3 acceleration;	//change in velocity per second
	protected Vector3 velocity;		//change in position per second
	protected Vector3 dv;			//desired velocity
	public Vector3 Velocity
	{
		get { return velocity; }
		set { velocity = value; }
	}

	//Classes that extend Vehicle must override CalcSteeringForce
	abstract protected void CalcSteeringForce();

	virtual public void Start()
	{
		acceleration = Vector3.zero;
		velocity = transform.forward;
		//get component references
		characterController = this.GetComponent<CharacterController>();
	}


	// Update is called once per frame
	protected void Update()
	{
		CalcSteeringForce();

		//update velocity
		velocity += acceleration * Time.deltaTime;
		velocity.y = 0;	// we are staying in the x/z plane
		velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

		//orient the transform to face where we going
		if (velocity != Vector3.zero)
			transform.forward = velocity.normalized;

		// keep us grounded
		velocity.y -= gravity * Time.deltaTime;

		// the CharacterController moves us subject to physical constraints
		characterController.Move(velocity * Time.deltaTime);

		//reset acceleration for next cycle
		acceleration = Vector3.zero;
	}

	protected void ApplyForce(Vector3 steeringForce)
	{
		acceleration += steeringForce / mass;
	}


	//-------- functions that return steering forces -------------//
	protected Vector3 Seek(Vector3 targetPos)
	{
		//find dv, desired velocity
		dv = targetPos - transform.position;
		dv = dv.normalized * maxSpeed;	//scale by maxSpeed
		dv -= velocity;
		dv.y = 0;						// only steer in the x/z plane
		return dv;
	}


	protected Vector3 AvoidObstacle(GameObject obst, float safeDistance)
	{
		dv = Vector3.zero;
		Vector3 steer = Vector3.zero;
		float obRadius = obst.collider.bounds.extents.magnitude + 1;
		float avoidRadius = safeDistance;
		safeDistance += radius + obRadius;

		//calculate displacement
		Vector3 disp = obst.transform.position - transform.position;

		//check if the entity is close enough to care about
		if (disp.magnitude < safeDistance)
		{
			//get the distance to the object
			float dist = disp.magnitude - obRadius;

			//get point on bounding circle closests to entity
			Vector3 obstacleEdge = disp.normalized;
			obstacleEdge *= -obRadius;

			//calculate the tangent to the edge point
			Vector3 tangent = new Vector3(obstacleEdge.z, 0, -obstacleEdge.x).normalized;
			Vector3 projection = Vector3.Dot(tangent, transform.forward) * tangent;

			//get point that is desirable distance from the obstacle
			Vector3 collisionBuffer = disp.normalized * (-avoidRadius + 2);

			//calculate target point along the tangent line, offset by collision buffer
			Vector3 targetPos = obst.transform.position + obstacleEdge + collisionBuffer + projection * 3;

			//get displacement to target point
			Vector3 targetDisp = obst.transform.position - targetPos;

			//project out position in 5 frames on current heading
			Vector3 curPathPos = transform.position + transform.forward * maxSpeed * 5 * Time.deltaTime;
			Vector3 curPathDisp = obst.transform.position - curPathPos;

			if (curPathDisp.magnitude < targetDisp.magnitude)
			{
				steer = Seek(targetPos).normalized * ((avoidRadius + 1) / ((dist / 2) * (dist / 2)));
				//print("tangent avoid: " + steer.magnitude +" dist: " + dist);
			}
		}

		return steer;
	}

	// Try this one on your own		
	protected Vector3 Wander()
	{
		Vector3 target = transform.position + transform.forward * wanderDist;
		Quaternion rot = Quaternion.Euler(0, wanderAng, 0);
		Vector3 offset = rot * transform.forward;
		target += offset * wanderRad;
		wanderAng += Random.Range(-wanderRand, wanderRand);
		return Seek(target);
	}

	//-------------- flocking functions ----------------------
	public Vector3 Separation(List<GameObject> neighbors, float separationDistance)
	{
		//create vector to hold total
		Vector3 total = Vector3.zero;
		//check distance from each neighbor flocker
		foreach (GameObject n in neighbors)
		{
			Vector3 dv = transform.position - n.transform.position;
			float dist = dv.magnitude;
			//if neighbor is in my space
			if (dist > 0 && dist < separationDistance)
			{
				//scale for importance based on distance
				dv *= separationDistance / dist;
				//zero out Y plane
				dv.y = 0;
				//gather up all the totals
				total += dv;
			}
		}
		total = total.normalized * maxSpeed;
		total -= velocity;
		return total;
	}

	public Vector3 Alignment(Vector3 direction)
	{
		dv = direction.normalized * maxSpeed;
		dv -= velocity;
		dv.y = 0;
		return dv;
	}

	public Vector3 Cohesion(Vector3 targetPos)
	{
		return Seek(targetPos);
	}

	public Vector3 StayInBounds(float radius, Vector3 center)
	{
		//if I'm out of bounds, seek the center
		if (Vector3.Distance(transform.position, center) > radius)
		{
			return Seek(center);
		}
		//otherwise I'm good
		else
			return Vector3.zero;
	}

	//--------------------------------------------------------

}
