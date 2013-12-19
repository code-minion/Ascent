﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class LootGenerator  
{
	// TODO: Allow this generator to take weight values to change how the randoms work

	public enum ELootType
	{
		Accessory = 0,
		Consumable,
		Any,
	}

	public static Item RandomlyGenerateItem(int floorNum, ELootType type)
	{
		// Randomly select the type of item
		if (type == ELootType.Any)
		{
			type = (ELootType)Random.Range(0, 1);
		}

		Item brandSpankingNewItem = null;
		
		// Initialise based on type
		switch (type)
		{
			case ELootType.Accessory:
				{
					brandSpankingNewItem = RandomlyGenerateAccessory(floorNum);
				}
				break;
			case ELootType.Consumable:
				{
					brandSpankingNewItem = RandomlyGenerateConsumable(floorNum);
				}
				break;
		}

		return brandSpankingNewItem;
	}

	public static Item RandomlyGenerateAccessory(int floorNum)
	{
		Item.ItemGrade grade = RandomGrade();

		Item newItem = new AccessoryItem()
		{
			Level = floorNum,
			Name = RandomAccessoryName(),
			Description = RandomAccessoryDescription(),
			GradeEnum = grade,
		};

		RandomAccessorySpecialProperties((AccessoryItem)newItem);
		RandomAccessoryBaseStats((AccessoryItem)newItem);

		return newItem;
	}

	public static Item RandomlyGenerateConsumable(int floorNum)
	{
		Item.ItemGrade grade = RandomGrade();

		ConsumableItem.EConsumableType consumableType = (ConsumableItem.EConsumableType)(Random.Range((int)ConsumableItem.EConsumableType.INVALID + 1, (int)ConsumableItem.EConsumableType.MAX + 1));

		Item newItem = null;

		// Create the consumable item
		switch (consumableType)
		{
			case ConsumableItem.EConsumableType.Health:
				{
					newItem = new ConsumableItem();
				} 
				break;
		}

		// TODO: Randomly generate quanity and power of the consumable (Not all consumables benefit from power)

		return newItem;
	}

	public static Item.ItemGrade RandomGrade()
	{
		return (Item.ItemGrade)Random.Range((int)Item.ItemGrade.E, (int)Item.ItemGrade.S);
	}

	public static void RandomAccessorySpecialProperties(AccessoryItem accessoryItem)
	{
		// Randomly choose properties based on level and grade.
		// Property quantity is affected by grade.

		//ConstantStatItemProperty prop = new ConstantStatItemProperty();
		//prop.Stats.Power = 1;

		//accessoryItem.ItemProperties.Add(prop);
	}

	public static void RandomAccessoryBaseStats(AccessoryItem accessoryItem)
	{
		// Randomly choose stat distribution based on level and grade
		accessoryItem.Stats.Power = 1;
		accessoryItem.Stats.Finesse = 1;
		accessoryItem.Stats.Spirit = 1;
		accessoryItem.Stats.Vitality = 1;
	}

	public static string RandomAccessoryName()
	{
		return "RandomName";
	}

	public static string RandomAccessoryDescription()
	{
		return "RandomDescription";
	}
}