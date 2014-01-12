﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Character : MonoBehaviour
{
	public enum EScope
	{
		Hero,
		Enemy,
		All,
	}

	public enum EHeroClass
	{
		Warrior,
		Rogue,
		Mage
	}

    public enum EDamageType
    {
        Physical,
        Magical,
    }
	
	protected List<Action> 			abilities = new List<Action>();
	protected Action 				activeAbility;
	protected GameObject 			weaponPrefab;
    protected bool 					isDead = false;
    protected Color 				originalColour;

    protected float 				stunDuration;
	protected float					stunTimeAccum;
	protected float					flickerDuration = 0.5f;
	protected float					flickerTimeAccum;
	
	protected Transform 			weaponSlot;
	protected Collidable 			chargeBall;
	protected Weapon 				equipedWeapon;
	protected AnimatorController 	characterAnimator;
	protected BaseStats 			baseStatistics;
	protected DerivedStats			derivedStats;
	protected List<Object> 			lastObjectsDamagedBy = new List<Object>();
	protected BetterList<Buff>		buffList = new BetterList<Buff>();

	protected CharacterMotor motor;
	public CharacterMotor Motor
	{
		get { return motor;  }
	}

	public delegate void DamageTaken(float amount);
	public event DamageTaken OnDamageTaken;

    public Transform WeaponSlot
    {
        get { return weaponSlot; }
    }

	public Collidable ChargeBall
	{
		get { return chargeBall; }
	}

    public Weapon Weapon
    {
        get { return equipedWeapon; }
    }

    public AnimatorController Animator
    {
        get { return characterAnimator; }
	}
	
	public BaseStats CharacterStats
	{
		get { return baseStatistics; }
	}
	
	public DerivedStats DerivedStats
	{
		get { return derivedStats; }
	}

    public List<Object> LastObjectsDamagedBy
    {
        get { return lastObjectsDamagedBy; }
    }

	public BetterList<Buff> BuffList
	{
		get { return buffList; }
	}

    public List<Action> Abilities
    {
        get { return abilities; }
    }

    /// <summary>
    /// Returns true if the character is dead. 
    /// </summary>
    public bool IsDead
    {
        get { return isDead; }
    }


	public virtual void Initialise()
	{
		Shadow shadow = GetComponentInChildren<Shadow>();
		shadow.Initialise();

		OnMove();

		motor = GetComponentInChildren<CharacterMotor>();
		motor.Initialise();
	}

    public virtual void Update()
    {
        UpdateActiveAbility();

		// Update abilities that require cooldown
		foreach (Action ability in abilities)
		{
			if (ability.IsOnCooldown == true)
			{
				ability.UpdateCooldown();
			}
		}
        
		// Process all the buffs
        foreach(Buff b in buffList)
        {
            b.Process();
        }
    }

	/// <summary>
	/// Updates the character tilt and shadow
	/// </summary>
	public void OnMove()
	{
		if (!isDead)
		{
			//CharacterTilt tilt = GetComponentInChildren<CharacterTilt>();
			//tilt.Process();

			Shadow shadow = GetComponentInChildren<Shadow>();
			shadow.Process();
		}
	}

    private void UpdateActiveAbility()
    {
        if (activeAbility != null)
        {
            activeAbility.UpdateAbility();
        }
    }

    public virtual void UseAbility(int abilityID)
    {
		if (activeAbility == null)
		{
            Action ability = abilities[abilityID];

            // Make sure the cooldown is off otherwise we cannot use the ability
            if (ability != null && ability.IsOnCooldown == false && (derivedStats.CurrentSpecial - ability.SpecialCost) > 0 )
            {
                ability.StartAbility();
                activeAbility = ability;

                derivedStats.CurrentSpecial -= ability.SpecialCost;
            }
		}
    }

	public Action GetAbility(string ability)
	{
		if (activeAbility == null)
		{
			Action action = abilities.Find(a => a.Name == ability); // this is a lambda 
			if (action == null)
			{
				Debug.LogError("Could not find and return ability: " + ability);
			}

			return(action);
		}
		return null;
	}

	public virtual void StopAbility()
	{
		if (activeAbility != null)
		{
			activeAbility.EndAbility();
			activeAbility = null;
		}
	}

    public virtual void ApplyDamage(int unmitigatedDamage, EDamageType type)
    {
		int finalDamage = unmitigatedDamage;

        // Obtain the health stat and subtract damage amount to the health.
        derivedStats.CurrentHealth -= finalDamage;

        // When the player takes a hit, spawn some damage text.
        HudManager.Singleton.TextDriver.SpawnDamageText(this.gameObject, unmitigatedDamage);

		if(OnDamageTaken != null)
		{
			OnDamageTaken.Invoke(finalDamage);
		}

        // If the character is dead
		if (derivedStats.CurrentHealth <= 0 && !isDead)
        {
            // On Death settings
            // Update states to kill character
            OnDeath();
        }
    }



    public virtual void ApplyKnockback(Vector3 direction, float magnitude)
    {
		// Taking damage may or may not interrupt the current ability
		direction = new Vector3(direction.x, 0.0f, direction.z);
		//transform.GetComponent<CharacterController>().Move(direction * magnitude);
		motor.SetKnockback(direction, magnitude);
		//transform.rigidbody.AddForce(direction * magnitude, ForceMode.Impulse);
    }

    public virtual void ApplySpellEffect()
    {
		// Taking damage may or may not interrupt the current ability
    }

    public virtual void ApplyStunEffect(float duration)
    {
        Debug.Log("Stunned: " + duration);
        stunDuration = duration;
    }

    /// <summary>
    /// When the character needs to respawn into the game.
    /// </summary>
    /// <param name="position">The position to spawn the character.</param>
    public virtual void Respawn(Vector3 position)
    {
        isDead = false;

        // Play this animation.
        //Animator.PlayAnimation("Respawn");
        transform.position = position;
    }

    public virtual void OnDeath()
    {
        // We may internally tell this character that they are dead.
        // The reason we do this is when we pool objects we will re-use 
        // this character.
        isDead = true;
        
        // Obtain the last object that killed this character
        if (lastObjectsDamagedBy.Count > 0)
        {
            Object obj = LastObjectsDamagedBy[lastObjectsDamagedBy.Count - 1];
            System.Type type = obj.GetType();

            // Check if the type of object is a weapon
            // then we can get the owner character.
            // TODO: Maybe the character should only know the object it was killed by and other characters will handle being killed by specific objects.
            if (type == typeof(Weapon))
            {
                Weapon weapon = obj as Weapon;
            }
        }
        else
        {
            //Debug.Log("Character somehow died by somethign and we do not know what caused it.");
        }
    }
	
	protected void AddSkill(Action skill)
	{
		skill.Initialise(this);
		abilities.Add(skill);
	}

    public virtual void RefreshEverything()
    {
        derivedStats.CurrentHealth = derivedStats.MaxHealth;
        derivedStats.CurrentSpecial = derivedStats.MaxSpecial;
    }

    public virtual void AddBuff(Buff buff)
    {
        buffList.Add(buff);
    }

    public virtual void RemoveBuff(Buff buff)
    {
        buffList.Remove(buff);
    }

	#if UNITY_EDITOR
	public void OnDrawGizmos()
	{
		if (activeAbility !=null)
		{
			activeAbility.DebugDraw();
		}
	}
	#endif
}
