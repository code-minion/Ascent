﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebrisExplosion : Projectile
{
    private Character owner;

    private float timeElapsed;
    private float lifeSpan = 0.5f;
    private bool damageDone = false;

    private Circle circle;

    public void Initialise(Vector3 startPos, Character owner)
    {
        transform.position = startPos;
        this.owner = owner;

        circle = new Circle(transform, 2.0f, Vector3.zero);

		transform.parent = EffectFactory.Singleton.transform;
    }

    public void Update()
    {
        // At t1 deal damage.
        // At t2 destroy self

        timeElapsed += Time.deltaTime;

        if (timeElapsed >= lifeSpan * 0.25f && !damageDone)
        {
            List<Character> characters = new List<Character>();

            Room curRoom = Game.Singleton.Tower.CurrentFloor.CurrentRoom;
            if (curRoom.CheckCollisionArea(circle, Character.EScope.All, ref characters))
            {
                foreach (Character c in characters)
                {
                    if (c == owner)
                    {
                        continue;
                    }
                    // Apply damage and knockback to the enemey
                    CombatEvaluator combatEvaluator = new CombatEvaluator(owner, c);
                    combatEvaluator.Add(new PhysicalDamageProperty(owner.Stats.Attack, 1.0f));
					combatEvaluator.Add(new StatusEffectCombatProperty(new StunnedDebuff(owner, c, 1.0f)));
                    combatEvaluator.Add(new KnockbackCombatProperty(c.transform.position - transform.position, 100.0f));
                    combatEvaluator.Apply();

                    // Create a blood splatter effect on the enemy.
                    EffectFactory.Singleton.CreateBloodSplatter(c.transform.position, c.transform.rotation);
                }
            }

            damageDone = true;
        }

        if (timeElapsed >= lifeSpan)
        {
            timeElapsed = lifeSpan;
            GameObject.Destroy(this.gameObject);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (circle != null)
        {
            circle.DebugDraw();
        }
    }
#endif
}
