﻿using UnityEngine;
using System.Collections;

public class FrozenDebuff : StatusEffect
{
	public FrozenDebuff()
	{
        type = EEffectType.Debuff;
	}

	public FrozenDebuff(Character caster, Character target, float duration)
	{
		ApplyStatusEffect(caster, target);
	}

	public override void ApplyStatusEffect(Character caster, Character target)
	{
		overridePrevious = true;

		base.ApplyStatusEffect(caster, target);

		if (target.IsVulnerableTo(EStatus.Frozen) || caster == target)
		{
			target.Status |= EStatus.Frozen;
			target.StatusColour |= EStatusColour.Blue;

			target.Motor.StopMotion();
			target.Motor.StopMovingAlongGrid();

			FloorHUDManager.Singleton.TextDriver.SpawnDamageText(target.gameObject, "Frozen!", Color.blue);
		}
		else
		{
            ProcessImmuneEffect(target);
		}
	}

	protected override void EndEffect()
	{
		target.Status &= ~EStatus.Frozen;
		target.StatusColour &= ~EStatusColour.Blue;
	}
}
