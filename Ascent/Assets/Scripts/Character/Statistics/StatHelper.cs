﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class StatHelper
{
	public static int Power(Character character)
	{
		// Get character base statistics
		int power = character.Stats.Power;

		if (character is Hero)
		{
			// Go through the list of items and add power
			Backpack backPack = ((Hero)character).Backpack;
			int itemCount = backPack.AccessoryCount;

			if (itemCount > 0)
			{
				int i;
				for (i = 0; i < itemCount; ++i)
				{
					Item item = backPack.AllItems[i];
					if (item is ConsumableItem)
					{
						continue;
					}

					power += (int)((AccessoryItem)item).Power;
				}
			}
		}

		// Add status buff modifiers
		//character.}
		List<StatusEffect> buffList = character.StatusEffects;

		int buffCount = buffList.Count;

		if (buffCount > 0)
		{
			int i;
			for (i = 0; i < buffCount; ++i)
			{
				if (buffList[i] is PrimaryStatModifierEffect)
				{
					power += (int)((PrimaryStatModifierEffect)buffList[i]).Power;
				}
			}
		}

		return power;
	}

	public static int Finesse(Character character)
	{
		// Get character base statistics
		int finesse = character.Stats.Finesse;
		if (character is Hero)
		{
			// Go through the list of items and add power
			Backpack backPack = ((Hero)character).Backpack;
			int itemCount = backPack.AccessoryCount;

			if (itemCount > 0)
			{
				for (int i = 0; i < itemCount; ++i)
				{
					Item item = backPack.AllItems[i];
					if (item is ConsumableItem)
					{
						continue;
					}

					finesse += (int)((AccessoryItem)item).Finesse;
				}
			}
		}

		return AddStatusBuffMods(character, finesse);
	}

    private static int AddStatusBuffMods(Character character, int finesse)
    {
        List<StatusEffect> buffList = character.StatusEffects;

        int buffCount = buffList.Count;
        if (buffCount == 0)
            return finesse;

        for (int i = 0; i < buffCount; ++i)
        {
            if (buffList[i] is PrimaryStatModifierEffect)
            {
                finesse += (int)((PrimaryStatModifierEffect)buffList[i]).Finesse;
            }
        }

        return finesse;
    }

	public static int Vitality(Character character)
	{
		// Get character base statistics
		int vitality = character.Stats.Vitality;

		if (character is Hero)
		{
			// Go through the list of items and add power
			Backpack backPack = ((Hero)character).Backpack;
			int itemCount = backPack.AccessoryCount;

			if (itemCount > 0)
			{
				int i;
				for (i = 0; i < itemCount; ++i)
				{
					//Backpack.BackpackSlot bs = (Backpack.BackpackSlot)i;
					Item item = backPack.AllItems[i];
					if (item is ConsumableItem)
					{
						continue;
					}

					vitality += (int)((AccessoryItem)item).Vitality;
				}
			}
		}

		// Add status buff modifiers
		//character.}
        List<StatusEffect> buffList = character.StatusEffects;

        int buffCount = buffList.Count;

		if (buffCount > 0)
		{
			int i;
			for (i = 0; i < buffCount; ++i)
			{
				if (buffList[i] is PrimaryStatModifierEffect)
				{
					vitality += (int)((PrimaryStatModifierEffect)buffList[i]).Vitality;
				}
			}
		}

		return vitality;
	}

	public static int Spirit(Character character)
	{
		// Get character base statistics
		int spirit = character.Stats.Spirit;

		if (character is Hero)
		{
			// Go through the list of items and add power
			Backpack backPack = ((Hero)character).Backpack;
			int itemCount = backPack.AccessoryCount;

			if (itemCount > 0)
			{
				int i;
				for (i = 0; i < itemCount; ++i)
				{
					Item item = backPack.AllItems[i];
					if (item is ConsumableItem)
					{
						continue;
					}

					spirit += (int)((AccessoryItem)item).Spirit;
				}
			}
		}

		// Add status buff modifiers
		//character.}
        List<StatusEffect> buffList = character.StatusEffects;

        int buffCount = buffList.Count;

		if (buffCount > 0)
		{
			int i;
			for (i = 0; i < buffCount; ++i)
			{
				if (buffList[i] is PrimaryStatModifierEffect)
				{
					spirit += (int)((PrimaryStatModifierEffect)buffList[i]).Spirit;
				}
			}
		}

		return spirit;
	}
}
