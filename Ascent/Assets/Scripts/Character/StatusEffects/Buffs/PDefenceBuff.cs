﻿using UnityEngine;
using System.Collections;

public class PDefenceBuff : SecondaryStatBuff
{
    protected float defenceBonusPercent = 0.25f;

    public override void ApplyStatusEffect(Character caster, Character target, float duration)
    {
        base.ApplyStatusEffect(caster, target, duration);

        target.AddStatusEffect(this);
    }
}
