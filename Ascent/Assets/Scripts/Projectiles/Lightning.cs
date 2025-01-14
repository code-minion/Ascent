﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lightning : Projectile
{
    private Character owner;

    private int targets;

    private List<Character> charactersHit = new List<Character>();

    private Circle circle;

    private bool hitSomething;

    private Vector3 velocity;

    public GameObject lightningEffectHit;

	int damage = 1;

    public void Initialise(int targets, int damage, Vector3 startPos, Character owner)
    {
		this.damage = damage;
        this.targets = targets;
        this.owner = owner;

        transform.position = new Vector3(startPos.x, 1.0f, startPos.z);
        rigidbody.AddForce(owner.transform.forward * 10.0f, ForceMode.VelocityChange);

        velocity = owner.transform.forward * 1.0f;

        circle = new Circle(transform, 7.5f, Vector3.zero);
    }

    public void Update()
    {
		rigidbody.AddForce(velocity, ForceMode.VelocityChange);
    }

    public void OnTriggerEnter(Collider collision)
    {
        bool lightningExpired = false;

        Vector3 pos = collision.ClosestPointOnBounds(this.transform.position);
		GameObject light = GameObject.Instantiate(lightningEffectHit, pos, collision.transform.rotation) as GameObject;

		light.transform.parent = EffectFactory.Singleton.transform;

		if (owner == null)
		{
			return;
		}

        if (!hitSomething)
        {
            switch ((Layer)collision.gameObject.layer)
            {
                case Layer.Monster:
                case Layer.Hero:
                    {
                        Character.EScope scope = owner is Enemy ? Character.EScope.Hero : Character.EScope.Enemy;

                        Character hitCharacter = collision.gameObject.GetComponent<Character>();

						//  this is the biggest hack to ensure that we get the actual character
						if (hitCharacter == null)
						{
							hitCharacter = collision.transform.parent.gameObject.GetComponent<Character>();

							if (hitCharacter == null)
							{
								hitCharacter = collision.transform.parent.parent.gameObject.GetComponent<Character>();
							}
						}
		
                        bool isOnSameTeam = false;

                        if ((owner is Enemy && hitCharacter is Enemy) ||
                            (owner is Hero && hitCharacter is Hero))
                        {
                            isOnSameTeam = true;
                        }

                        if (!charactersHit.Contains(hitCharacter))
                        {
                            hitSomething = true;
                            charactersHit.Add(hitCharacter);

                            // Apply damage and knockback to the enemey
                            CombatEvaluator combatEvaluator = new CombatEvaluator(owner, hitCharacter);

                            if (!isOnSameTeam)
                            {
                                combatEvaluator.Add(new PhysicalDamageProperty(damage, 1.0f));

                                damage -= 1;
                                if (damage <= 0)
                                {
                                    damage = 1;
                                }

                                // Create a blood splatter effect on the enemy.
                                //EffectFactory.Singleton.CreateBloodSplatter(hitCharacter.transform.position, hitCharacter.transform.rotation, hitCharacter.transform, 2.0f);
                            }
	
                            combatEvaluator.Apply();

                            // Find next target
                            if (charactersHit.Count < targets)
                            {
                                List<Character> characters = new List<Character>();
                                Room curRoom = Game.Singleton.Tower.CurrentFloor.CurrentRoom;

                                Character nextTarget = null;

                                if (curRoom.CheckCollisionArea(circle, scope, ref characters))
                                {
                                    foreach (Character c in characters)
                                    {
                                        if (!charactersHit.Contains(c))
                                        {
                                            nextTarget = c;
                                            break;
                                        }
                                    }

                                    if (nextTarget != null)
                                    {
                                        // Move to next target
                                        //projectile.transform.position = collision.gameObject.transform.position;
                                        rigidbody.velocity = Vector3.zero;
                                        velocity = nextTarget.transform.position - transform.position;
                                        rigidbody.AddForce(velocity, ForceMode.VelocityChange);
                                        hitSomething = false;
								SoundManager.PlaySound(AudioClipType.lightning, transform.position, 0.3f / (charactersHit.Count + 2) );
                                    }
                                    else
                                    {
                                        // No targets around to just expire
                                        lightningExpired = true;
                                    }
                                }
                            }
                            else
                            {
                                // No targets around to just expire
                                lightningExpired = true;
                            }
                        }
                        else
                        {
                            lightningExpired = true;
                        }

                    }
                    break;
				case Layer.Environment:
					{
						charactersHit.Add(null);
						if (charactersHit.Count < targets)
						{
							Character.EScope scope = owner is Enemy ? Character.EScope.Hero : Character.EScope.Enemy;

							List<Character> characters = new List<Character>();
							Room curRoom = Game.Singleton.Tower.CurrentFloor.CurrentRoom;

							Character nextTarget = null;

							if (curRoom.CheckCollisionArea(circle, scope, ref characters))
							{
								foreach (Character c in characters)
								{
									if (!charactersHit.Contains(c))
									{
										nextTarget = c;
										break;
									}
								}

								if (nextTarget != null)
								{
									// Move to next target
									//projectile.transform.position = collision.gameObject.transform.position;
									rigidbody.velocity = Vector3.zero;
									velocity = nextTarget.transform.position - transform.position;
									rigidbody.AddForce(velocity, ForceMode.VelocityChange);
									hitSomething = false;
									//SoundManager.PlaySound(AudioClipType.lightning, transform.position, 0.2f / charactersHit.Count + 2);
								}
								else
								{
									// No targets around to just expire
									lightningExpired = true;
								}
							}
						}
						else
						{
							lightningExpired = true;
						}
					}
					break;
                default:
                    {
                        lightningExpired = true;
                    }
                    break;
            }
        }


        if (lightningExpired)
        {
            GameObject.Destroy(this.gameObject);

        }
    }
}
