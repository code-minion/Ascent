﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Character : BaseCharacter
{
	#region Enums

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
        Trap
    }

	#endregion


	// The event delegate handler we will use to take in the character.
	public delegate void CharacterEventHandler(Character character);
	public event CharacterEventHandler onDeath;
	public event CharacterEventHandler onSpawn;

    public bool spawnOnLoad = true;

	// The event delegate handler we will use for damage taken.
	public delegate void Damage(int amount);
	public event Damage onDamageTaken;
	public event Damage onDamageDealt; // Not handled by the character.

    protected List<StatusEffect> statusEffects = new List<StatusEffect>();
    protected AbilityLoadout loadout;
	protected CharacterStats stats;
	protected bool hitTaken;
	protected bool isDead;
	protected Character lastDamagedBy;
	protected EStatus vulnerabilities = EStatus.All;
	protected EStatus status;
	protected EStatusColour statusColour = EStatusColour.White;
	protected bool colourHasChanged = false;

	protected float hitDuration = 0.3f;
	protected float hitTimerElapsed;

	public Transform stunEffectPosition;

    public AbilityLoadout Loadout
    {
        get { return loadout; }
    }

    public bool HitTaken
    {
        get { return hitTaken; }
        set 
        {
            if (value && hitTaken != value)
            {              
                StartCoroutine("SetHitTaken");
            }
            
        }
    }

	public CharacterStats Stats
	{
		get { return stats; }
	}

	public Character LastDamagedBy
	{
		get { return lastDamagedBy; }
		set { lastDamagedBy = value; }
	}

	public List<StatusEffect> StatusEffects
	{
		get { return statusEffects; }
	}

	public EStatus Vulnerabilities
	{
		get { return vulnerabilities; }
		set { vulnerabilities = value; }
	}

	public EStatus Status
	{
		get { return status; }
		set { status = value; }
	}

	public EStatusColour StatusColour
	{
		get { return statusColour; }
		set 
		{
			// If this is a new colour flag it for updating.
			colourHasChanged = true;
			statusColour = value; 
		}
	}	 

	public bool CanMove
	{
        get { return !IsInState(EStatus.Stun) && !IsInState(EStatus.Frozen); }
	}

	public bool CanAct
	{
        get { return !IsInState(EStatus.Stun) && !IsInState(EStatus.Frozen); }
	}

	public bool CanAttack
	{
        get { return !IsInState(EStatus.Stun) && !IsInState(EStatus.Frozen); }
	}

	/// <summary>
	/// Returns true if the character is dead. 
	/// </summary>
	public bool IsDead
	{
		get { return isDead; }
	}

	public override void Initialise()
	{
		base.Initialise();
	}

	public override void Update()
	{
        loadout.Process();

		if (colourHasChanged)
		{
			SetColor(StatusEffectUtility.GetColour(StatusColour));
			colourHasChanged = false;
		}

        // Remove any expired buffs
        for (int i = statusEffects.Count - 1; i >= 0; --i)
        {
            if (statusEffects[i].ToBeRemoved)
            {
                statusEffects.RemoveAt(i);
            }
        }

		// Process all the buffs
		foreach (StatusEffect b in statusEffects)
		{
			b.Process();
		}

		if (!isDead && hitTimerElapsed > 0.0f)
		{
			hitTimerElapsed -= Time.deltaTime;
			if (hitTimerElapsed < 0.0f)
			{
				hitTimerElapsed = 0.0f;
				hitTaken = false;
				Animator.PlayAnimation("Hit", false);
				motor.EnableStandardMovement(true);
			}
		}
	}

	/// <summary>
	/// Applys damage to this chracter.
	/// </summary>
	/// <param name="unmitigatedDamage">The amount of damage.</param>
	/// <param name="type">The type of damage.</param>
	/// <param name="owner">The character that has dealt the damage to this character.</param>
	public virtual void ApplyCombatEffects(DamageResult result)
	{
		int finalDamage = result.finalDamage;

		if (!result.dodged)
		{
			if (this is Enemy)
			{
				if (loadout.CanInterruptActiveAbility)
				{
					loadout.StopAbility();
				}
				if (loadout.ActiveAbility == null)
				{
					motor.StopMotion();
					motor.EnableStandardMovement(false);
				}
			}

            // Don't set this to self
            lastDamagedBy = (result.source == this) ? lastDamagedBy : result.source;


#if UNITY_EDITOR
			if (this is Hero && Game.Singleton.invincible)
			{
				finalDamage = 0;
			}
#endif

			// Let the owner know of the amount of damage done.
			if (result.source != null)
			{
				result.source.OnDamageDealt(finalDamage);
			}

			// Tell this character how much damage it has done.
            if (finalDamage > 0)
            {
	
		
				// Obtain the health stat and subtract damage amount to the health.
				stats.CurrentHealth -= finalDamage;
	
				if ((stats.CurrentHealth > 0))
				{
					HitTaken = true;

					if (this is Enemy)
					{
						Animator.PlayAnimation("Hit", true);
						hitTimerElapsed = hitDuration;
					}
				}

                OnDamageTaken(finalDamage);
            }

			// Do knockback
			if (result.knockbackMagnitute != 0.0f)
			{
				motor.SetKnockback(result.knockbackDirection, result.knockbackMagnitute);
			}
		}



		// If the character is dead
		if (stats.CurrentHealth <= 0 && !isDead)
		{
			// On Death settings
			// Update states to kill character
			OnDeath();
		}
	}

	private Color hitColor = Color.black;
	public IEnumerator SetHitTaken()
	{
		if (this is Hero)
		{
			if (hitColor == Color.black)
			{
				hitColor = Player.GetPlayerColor(Game.Singleton.GetPlayer(((Hero)this)).PlayerID);
				hitColor *= 0.5f;
				hitColor.r += .35f;
				hitColor.g += .35f;
				hitColor.b += .35f;
				hitColor.a = 0.85f;
			}

			Renderer[] renderers = Renderers;
			renderers[0].materials[2].shader = Shader.Find("Outlined/Diffuse");
			renderers[0].materials[2].SetColor("_OutlineColor", hitColor);
			renderers[0].materials[2].SetFloat("_Outline", 0.002f);
				//_Outline ("Outline width", Range (.002, 0.03)) = .003

			//renderers[0].materials[2].SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 0.75f));
		}

		hitTaken = true;
		yield return new WaitForSeconds(0.25f);

		if (this is Hero)
		{
			Renderer[] renderers = Renderers;

			renderers[0].materials[2].shader = Shader.Find("DiffuseNormalSpecular");

			hitTaken = false;
		}
	} 


	public virtual void ApplyKnockback(Vector3 direction, float magnitude)
	{
		if (IsVulnerableTo(EStatus.Knock))
		{
			// Taking damage may or may not interrupt the current ability
			direction = new Vector3(direction.x, 0.0f, direction.z);
			//transform.GetComponent<CharacterController>().Move(direction * magnitude);
			motor.SetKnockback(direction, magnitude);
			//transform.rigidbody.AddForce(direction * magnitude, ForceMode.Impulse);
		}
	}

	/// <summary>
	/// When the character needs to respawn into the game.
	/// </summary>
	/// <param name="position">The position to spawn the character.</param>
	public virtual void Respawn(Vector3 position)
	{
		isDead = false;

		// Play this animation.
		transform.position = position;
		OnSpawn();
	}

    protected virtual void OnDeath()
	{
		if (loadout.IsAbilityActive)
		{
			loadout.StopAbility();
		}
		// We may internally tell this character that they are dead.
		// The reason we do this is when we pool objects we will re-use 
		// this character.
		isDead = true;

		for (int i = statusEffects.Count - 1; i >= 0; --i)
		{
			statusEffects[i].EndEarly();
			statusEffects.RemoveAt(i);
		}

		// Notify subscribers of the death.
		if (onDeath != null)
		{
			onDeath(this);
		}
	}

    protected virtual void OnSpawn()
    {
        if (onSpawn != null)
        {
            onSpawn(this);
        }
    }

    /// <summary>
    /// The event called when this character deals damage.
    /// </summary>
    /// <param name="damage">The amount of damage dealt.</param>
    protected virtual void OnDamageDealt(int damage)
    {
        if (onDamageDealt != null)
        {
            onDamageDealt(damage);
        }
    }

    /// <summary>
    /// The event called when this character takes damage. 
    /// </summary>
    /// <param name="damage">The amount of damage taken.</param>
    protected virtual void OnDamageTaken(int damage)
	{
		SoundManager.PlaySound(AudioClipType.wethit, transform.position, .04f);
        if (onDamageTaken != null)
        {
            onDamageTaken(damage);
        }
    }

    public virtual void RefreshEverything()
    {	
		if (stats != null)
		{
			stats.CurrentHealth = stats.MaxHealth;
			stats.CurrentSpecial = stats.MaxSpecial;

            if (isDead)
            {
                motor.IsHaltingMovementToPerformAction = false;
                Animator.Dying = false;
                collider.enabled = true;
                isDead = false;
                ((HeroAnimator)animator).playingDeath = false;
            }
		}
    }

	public bool IsVulnerableTo(EStatus statusEffect)
	{
		return (vulnerabilities & statusEffect) == statusEffect;
	}

	public bool IsInState(EStatus statusEffect)
	{
		return (status & statusEffect) == statusEffect;
	}

	public virtual void ApplyStatusEffect(StatusEffect effect)
	{
		statusEffects.Add(effect);
	}

    public virtual void RemoveStatusEffect(StatusEffect effect)
	{
        effect.ToBeRemoved = true;
	}

#if UNITY_EDITOR
	public void OnDrawGizmos()
	{
        if(loadout != null) loadout.DebugDraw();
	}
#endif
}
