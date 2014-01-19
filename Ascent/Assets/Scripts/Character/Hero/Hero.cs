using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Hero : Character 
{
	public struct HeroClassStatModifier
	{
		public float PowerAttack;
		public float FinesseCritChance;
		public float FinesseCritBonus;
		public float FinesseDodge;
		public float FinesseBlock;
		public float VitalityHP;
		public float VitalityPhysRes;
		public float VitalityHPRegen;
		public float SpiritSP;
		public float SpiritMagRes;
//		HeroClassStatModifier(float _powerAttack, float _finesseCritChance, 
//		                      float _finesseCritBonus, float _finesseDodge, 
//		                      float _finesseBlock, float _vitalityHP, 
//		                      float _vitalityPhysRes, float _vitalityHpRegen, 
//		                      float _spiritSP, float _spiritMagRes)
//		{
//
//		}
	}

	protected HeroClassStatModifier classStatMod;
    protected HeroController heroController;
	protected Backpack backpack;
    protected HeroInventory heroInventory;
    protected HeroEquipment heroEquipment;
    protected FloorStats floorStatistics;

	public HeroClassStatModifier ClassStatMod
	{
		get { return classStatMod; }
	}

	public Backpack HeroBackpack
	{
		get { return backpack; }
	}

	public HeroInventory HeroInventory
	{
		get { return heroInventory; }
	}

	public HeroController HeroController
	{
		get { return heroController; }
	}

    public FloorStats FloorStatistics
    {
        get { return floorStatistics; }
    }

	public virtual void Initialise(InputDevice input, HeroSaveData saveData)
	{
        base.Initialise();

		heroInventory = new HeroInventory();
        heroEquipment = new HeroEquipment();
	}

    public override void SetColor(Color color)
    {
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer render in renderers)
        {
            render.material.color = color;
        }
    }

    public void ResetFloorStatistics()
    {
        floorStatistics = new FloorStats();
    }

    public override void Respawn(Vector3 position)
    {
        base.Respawn(position);

        Animator.PlayAnimation("Dying");

        // Reset the health
        derivedStats.ResetHealth();
    }

    /// <summary>
    /// Specific on death event functionality for heroes
    /// </summary>
    public override void OnDeath()
    {
        base.OnDeath();
    }

    public override void RefreshEverything()
    {
        // Resets hp and sp
        base.RefreshEverything();

        // Reset cooldowns
        foreach(Action a in abilities)
        {
            a.RefreshCooldown();
        }
    }

	public override void ApplyDamage(int unmitigatedDamage, Character.EDamageType type, Character owner)
	{
        base.ApplyDamage(unmitigatedDamage, type, owner);

		if (heroController.GrabbingObject)
		{
			heroController.ReleaseGrabbedObject();
			GetComponent<CharacterMotor>().StopMovingAlongGrid();
		}

        if (type == Character.EDamageType.Trap)
        {
            floorStatistics.NumberOfTrapsTripped++;
        }
	}

    protected override void OnDamageTaken(int damage)
    {
        base.OnDamageTaken(damage);

        // Record damage taken.
        floorStatistics.DamageTaken += damage;
        // Hero takes hit.
        HeroAnimator heroAnim = Animator as HeroAnimator;
        heroAnim.TakeHit = true;
    }

    protected override void OnDamageDealt(int damage)
    {
        base.OnDamageDealt(damage);

        // Record damage dealt.
        FloorStatistics.TotalDamageDealt += damage;
    }
}
