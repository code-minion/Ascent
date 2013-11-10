﻿// Developed by Kit Chan 2013

// Dependencies
using UnityEngine;
using System.Collections;

public class Charge : IAction 
{
	Character owner;
	private const float animationTime = 2.333f;
	private const float animationSpeed = 2.0f;
	private float timeElapsed;
    private float distanceMax = 20.0f;
    private float distanceTraveled;

    public void Initialise(Character owner)
    {
        this.owner = owner;
    }
    
	public void StartAbility()
	{
		timeElapsed = 0.0f;
		owner.Animator.PlayAnimation("SwingAttack");
        // get owner's charge box and enable collision
		owner.Weapon.EnableCollision = true;
		owner.Weapon.SetAttackProperties(999,Character.EDamageType.Physical);
	}

	public void UpdateAbility()
	{

        Vector3 moveVec = owner.transform.forward * Time.deltaTime * animationSpeed;
        distanceTraveled += moveVec.magnitude;
        
        owner.transform.position += moveVec;

		if (distanceTraveled >= distanceMax)
		{
			owner.StopAbility();
		}
	}

	public void EndAbility()
	{
		owner.Weapon.EnableCollision = false;
		owner.Animator.StopAnimation("SwingAttack");
	}
}

/*
// Developed by Kit Chan 2013

// Dependencies
using UnityEngine;
using System.Collections;

public class Charge : IAction 
{
	Character owner;
	private const float animationTime = 2.333f;
	private const float animationSpeed = 2.0f;
	private float timeElapsed;
    //private float distanceMax = 20.0f;
    //private float distanceTraveled;

    public void Initialise(Character owner)
    {
        this.owner = owner;
    }
    
	public void StartAbility()
	{
		timeElapsed = 0.0f;
		owner.Animator.PlayAnimation("SwingAttack");
		
        // get owner's charge box and enable collision
		owner.Weapon.EnableCollision = true;
		owner.Weapon.SetAttackProperties(999,Character.EDamageType.Physical);
	}

	public void UpdateAbility()
	{
//
//        Vector3 moveVec = owner.transform.forward * Time.deltaTime * animationSpeed;
//        distanceTraveled += moveVec.magnitude;
//        Debug.Log(distanceTraveled);
//        owner.transform.position += moveVec;
//
//		if (distanceTraveled >= distanceMax)
//		{
//			owner.StopAbility();
//		}
	}

	public void EndAbility()
	{
		owner.Weapon.EnableCollision = false;
		owner.Animator.StopAnimation("SwingAttack");
	}
}
*/