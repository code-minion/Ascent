﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bomb : MonoBehaviour 
{
	private Character owner;
	private float damage;
	private float radius;
	private float fuseTimeMax;
	private float timeElapsed;

	private Circle damageArea;

	public void Initialise(Character owner, float fuseTime, float radius, float damage)
	{
		this.owner = owner;
		this.damage = damage;
		this.radius = radius;
		fuseTimeMax = fuseTime;

		// Defines the collision shape and properties of this ability.
		damageArea = new Circle(transform, this.radius, new Vector3(0.0f, 0.0f, 0.0f));

		transform.position = owner.transform.position + (owner.transform.forward * 0.25f) + Vector3.up;
		rigidbody.AddForce((owner.transform.forward * 25.0f) + (Vector3.up * 25.0f), ForceMode.Impulse);
		rigidbody.AddTorque(new Vector3(Random.Range(10.0f, 100.0f), Random.Range(10.0f, 100.0f), Random.Range(10.0f, 100.0f)), ForceMode.Impulse);
	}
	
	void Update () 
	{
		if (timeElapsed != fuseTimeMax)
		{
			timeElapsed += Time.deltaTime;
			if (timeElapsed >= fuseTimeMax)
			{
				timeElapsed = fuseTimeMax;

				// Blow up
				List<Character> characters = new List<Character>();

				if (Game.Singleton.InTower)
				{
					// Do damage
					Room room = Game.Singleton.Tower.CurrentFloor.CurrentRoom;
					if (room.CheckCollisionArea(damageArea, Character.EScope.All, ref characters))
					{
						foreach (Character c in characters)
						{
							// Apply damage and knockback to the enemey
							CombatEvaluator combatEvaluator = new CombatEvaluator(owner, c);
							combatEvaluator.Add(new PhysicalDamageProperty(damage, 1.0f));
							combatEvaluator.Apply();

							// Create a blood splatter effect on the enemy.
                            EffectFactory.Singleton.CreateBloodSplatter(c.transform.position, c.transform.rotation);
						}
					}

					// Check for any doors that can be opened
					if (room.Doors.hiddenDoorCount > 0)
					{
						HiddenDoor[] hiddenDoors = room.Doors.HiddenDoors;

						foreach (HiddenDoor d in hiddenDoors)
						{
							if (d.opened)
							{
								continue;
							}
							if (MathUtility.IsWithinCircle(d.transform.position, damageArea.Position, damageArea.radius))
							{
								d.Open();
							}
						}
					}
				}

				Destroy(this.gameObject);
			}
		}

		rigidbody.AddForce(Vector3.down * 9.8f, ForceMode.Acceleration);
	}

#if UNITY_EDITOR
	public void OnDrawGizmos()
	{
		damageArea.DebugDraw();
	}
#endif
}
