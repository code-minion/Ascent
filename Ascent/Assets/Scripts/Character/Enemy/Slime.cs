﻿// Developed by Mana Khamphanpheng 2013

// Dependencies
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Slime : Enemy
{
    private AITrigger OnAttackedTrigger;
    private AITrigger OnWanderEndTrigger;
    private AITrigger OnReplicateTrigger;

    private int replicateActionID;

    public override void Initialise()
    {
        // Populate with stats
        baseStatistics = new BaseStats();
        baseStatistics.Vitality = (int)((((float)health * (float)Game.Singleton.NumberOfPlayers) * 0.80f) / 10.0f);

        baseStatistics.CurrencyBounty = 1;
        baseStatistics.ExperienceBounty = 50;
        derivedStats = new DerivedStats(baseStatistics);
        derivedStats.Attack = 5;

        // Add abilities
        Action replicate = new SlimeReplicate();
        replicate.Initialise(this);
        abilities.Add(replicate);
        replicateActionID = 0;

        originalColour = Color.white;

        base.Initialise();

        InitialiseAI();
    }

    public void InitialiseAI()
    {
        agent.Initialise(transform);

        AIBehaviour behaviour = null;

        // Defensive behaviour
        behaviour = agent.MindAgent.AddBehaviour(AIMindAgent.EBehaviour.Defensive);
        {
            // OnAttacked, Triggers if attacked
            OnAttackedTrigger = behaviour.AddTrigger();
            OnAttackedTrigger.Priority = AITrigger.EConditionalExit.Stop;
            OnAttackedTrigger.AddCondition(new AICondition_Attacked(this));
            OnAttackedTrigger.AddCondition(new AICondition_ActionCooldown(abilities[replicateActionID]), AITrigger.EConditional.And);
            OnAttackedTrigger.OnTriggered += OnAttacked;

            OnReplicateTrigger = behaviour.AddTrigger();
            OnReplicateTrigger.AddCondition(new AICondition_Timer(5.5f, 2.0f, 10.0f));
            OnReplicateTrigger.OnTriggered += OnAttacked;

            // OnWanderEnd, Triggers if time exceeds 2s or target reached.
            OnWanderEndTrigger = behaviour.AddTrigger();
            OnWanderEndTrigger.Priority = AITrigger.EConditionalExit.Stop;
            OnWanderEndTrigger.AddCondition(new AICondition_Timer(1.5f, 2.0f, 4.0f));
            OnWanderEndTrigger.AddCondition(new AICondition_ReachedTarget(agent.SteeringAgent), AITrigger.EConditional.Or);
            OnWanderEndTrigger.OnTriggered += OnWanderEnd;

        }

        agent.MindAgent.SetBehaviour(AIMindAgent.EBehaviour.Defensive);
        agent.SteeringAgent.SetTargetPosition(containedRoom.NavMesh.GetRandomOrthogonalPositionWithinRadius(transform.position, 7.5f));
    }

    public void OnWanderEnd()
    {
        // Choose a new target location
        agent.SteeringAgent.SetTargetPosition(containedRoom.NavMesh.GetRandomOrthogonalPositionWithinRadius(transform.position, 7.5f));

        // Reset behaviour
        OnWanderEndTrigger.Reset();

    }

    public void OnAttacked()
    {
        UseAbility(replicateActionID);
        OnAttackedTrigger.Reset();
        OnReplicateTrigger.Reset();
    }

    public void OnCollisionEnter(Collision other)
    {
        string tag = other.transform.tag;

        switch (tag)
        {
            case "Hero":
                {
                    Character otherCharacter = other.transform.GetComponent<Character>();

                    if (!IsStunned && !otherCharacter.IsInvulnerable)
                    {
                        CollideWithHero(otherCharacter as Hero, other);
                    }
                }
                break;
        }
    }

    private void CollideWithHero(Hero hero, Collision collision)
    {
        hero.LastDamagedBy = this;

        Vector3 direction = (collision.transform.position - transform.position).normalized;
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, direction);

        Game.Singleton.EffectFactory.CreateBloodSplatter(collision.transform.position, rot, hero.transform, 3.0f);

        hero.ApplyDamage(1, EDamageType.Physical, this);
        hero.ApplyKnockback(direction, 10.0f);
    }
}
