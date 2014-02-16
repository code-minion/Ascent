﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(CharacterAnimator))]
public class CharacterMotor : MonoBehaviour
{
	public GameObject actor;

	private Vector3 movementForce;

	public float currentSpeed = 0.0f;
	public float acceleration = 1.0f;
	public float minSpeed = 5.5f;

	protected float originalSpeed = 6.0f;
	protected float movementSpeed = 6.0f;

	protected float buffBonusSpeed;
	protected float maxVelocityChange = 5.0f;
	protected bool canMove = true;


	private float knockbackMag;
	private float knockbackDecel = 0.65f;

	private Vector3 targetVelocity;
	private Vector3 knockbackDirection;

	private bool usingMovementForce = true;

	// Grid Movement
	public bool MovingAlongGrid;
	float offset = 1.0f;
	float moveTime = 0.5f;
	float timeAccum;
	Vector3 startPos;
	Vector3 targetPos;

	public float MovementSpeed
	{
		get { return movementSpeed; }
		set { movementSpeed = value; }
	}

	public float OriginalSpeed
	{
		get { return originalSpeed; }
		set { originalSpeed = value; }
	}

	public float BuffBonusSpeed
	{
		get { return buffBonusSpeed; }
		set { buffBonusSpeed = value; }
	}

	public float MaxVelocityChange
	{
		get { return maxVelocityChange; }
		set { maxVelocityChange = value; }
	}

	public bool IsHaltingMovementToPerformAction
	{
		get { return canMove; }
		set { canMove = value; }
	}

	public bool IsUsingMovementForce
	{
		get { return usingMovementForce; }
	}

    public Vector3 TargetVelocity
    {
        get { return targetVelocity; }
    }
	
	public virtual void Initialise()
	{
		// Physics is mostly disabled and handled by code.
		rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
		rigidbody.freezeRotation = true;
		rigidbody.useGravity = false;
	}

	public virtual void FixedUpdate()
	{
		if (MovingAlongGrid)
		{
			ProcessGridMovement();
		}
		else
		{
			ProcessMovement();
		}
	}

	public void ProcessGridMovement()
	{
		// TODO: Do a check in HeroController to stop prevent this movement happening.
		// Grid movement uses Lerp. This is dangerous because it can go through other objects.
		timeAccum += Time.deltaTime;
		if (timeAccum > moveTime)
		{
			timeAccum = 1.0f;
			MovingAlongGrid = false;
		}

		transform.position = Vector3.Lerp(startPos, targetPos, timeAccum / moveTime);
	}

	public void ProcessMovement()
	{
		// Reset the targets
		targetVelocity = Vector3.zero;
		Vector3 knockbackVel = Vector3.zero;
		int forces = 0;

		
		if (knockbackMag > 0.0f)
		{
			//float knockbackWeight = 1000000.0f;
			//targetVelocity += (knockbackDirection * knockbackMag) * knockbackWeight;
			//++forces;

			knockbackMag -= knockbackMag * knockbackDecel;

			knockbackVel = (knockbackDirection * knockbackMag);

			if (knockbackMag < 0.01f)
			{
				knockbackMag = 0.0f;
			}
		}

		if (movementForce != Vector3.zero && usingMovementForce)
		{
			// Add to forces
			targetVelocity += new Vector3(movementForce.x, 0.0f, movementForce.z);
			++forces;

			float speed = Mathf.Abs(movementForce.x) > Mathf.Abs(movementForce.z) ? Mathf.Abs(movementForce.x) : Mathf.Abs(movementForce.z);
			float maxAccel = speed;

			float buffedSpeed = movementSpeed + buffBonusSpeed;

			currentSpeed += (speed * buffedSpeed) * acceleration * Time.deltaTime;
			currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxAccel * buffedSpeed);
		}
		else if (movementForce == Vector3.zero)
		{
			currentSpeed = 0.0f;
		}

		if (forces > 0)
		{
			targetVelocity /= (float)forces;

			targetVelocity = new Vector3(targetVelocity.x, 0.0f, targetVelocity.z);
		}

		targetVelocity = (targetVelocity * currentSpeed) + knockbackVel;

		// Apply a force that attempts to reach our target velocity
		Vector3 velocity = rigidbody.velocity;
		Vector3 velocityChange = (targetVelocity - velocity);

		float buffedMaxVelocityChange = maxVelocityChange + (buffBonusSpeed * 0.5f);

		velocityChange.x = Mathf.Clamp(velocityChange.x, -buffedMaxVelocityChange, buffedMaxVelocityChange);
		velocityChange.z = Mathf.Clamp(velocityChange.z, -buffedMaxVelocityChange, buffedMaxVelocityChange);
		velocityChange.y = 0;

		rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
	}

	public virtual void Move(Vector3 motion)
	{
		movementForce = motion;
        movementForce.y = 0.0f;

		//FixRotationOrthogonally(motion);
	}

	public virtual void FixRotationOrthogonally(Vector3 curDirection)
	{
        if (Mathf.Abs(movementForce.x) > Mathf.Abs(movementForce.z))
        {
            float sign = movementForce.x > 0.0f ? 1.0f : -1.0f;
            transform.LookAt(transform.position + new Vector3(1.0f * sign, 0.0f, 0.0f));
        }
        else if (Mathf.Abs(movementForce.x) < Mathf.Abs(movementForce.z))
        {
            float sign = movementForce.z > 0.0f ? 1.0f : -1.0f;
            transform.LookAt(transform.position + new Vector3(0.0f, 0.0f, 1.0f * sign));
        }
	}

	public virtual void SetKnockback(Vector3 direction, float mag)
	{
		knockbackDirection = new Vector3(direction.x, 0.0f, direction.z);
		knockbackMag = mag * 100.0f;
	}

	public void MoveAlongGrid(Vector3 direction)
	{
		if (!MovingAlongGrid)
		{
			MovingAlongGrid = true;
			timeAccum = 0.0f;
			startPos = transform.position;
			targetPos = startPos + direction.normalized * offset;
		};
	}

	public void StopMovingAlongGrid()
	{
		timeAccum = 1.0f;
		MovingAlongGrid = false;
	}

    public void StopMotion()
    {
        movementForce = Vector3.zero;
        currentSpeed = 0.0f;
    }

	public void EnableMovementForce(bool b)
	{
		usingMovementForce = b;
	}
}
