using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITown_BackpackPanel : UITown_Panel 
{
	enum EButtons
	{
		ACC1 = 0,
		ACC2,
		ACC3,
		ACC4,
		ITM1,
		ITM2,
		ITM3,
		ITM4,
		MAX,
	}

	int lastActiveButton = 0;

	public override void Initialise()
	{
		buttons = new UIButton[(int)EButtons.MAX];
	
		Transform acc = transform.FindChild ("BackpackTab");
		Transform con = transform.FindChild ("BackpackTab");
		buttons[(int)EButtons.ACC1] = acc.FindChild("Accessory 1").GetComponent<UIButton>();
		buttons[(int)EButtons.ACC2] = acc.FindChild("Accessory 2").GetComponent<UIButton>();
		buttons[(int)EButtons.ACC3] = acc.FindChild("Accessory 3").GetComponent<UIButton>();
		buttons[(int)EButtons.ACC4] = acc.FindChild("Accessory 4").GetComponent<UIButton>();
		buttons[(int)EButtons.ITM1] = con.FindChild("Consumable 1").GetComponent<UIButton>();
		buttons[(int)EButtons.ITM2] = con.FindChild("Consumable 2").GetComponent<UIButton>();
		buttons[(int)EButtons.ITM3] = con.FindChild("Consumable 3").GetComponent<UIButton>();
		//buttons[(int)EButtons.ITM4] = con.FindChild("Consumable 4").GetComponent<UIButton>();

		currentButton = (int)EButtons.ACC1;
		currentSelection = buttons[(int)EButtons.ACC1];
		lastActiveButton = currentButton;
		buttonMax = (int)EButtons.MAX;

		initialised = true;

		UpdateItems();
	}

	public override void OnEnable()
	{
//		if (initialised)
//		{
//			UICamera.Notify(currentSelection.gameObject, "OnHover", true);
//		}

		base.OnEnable();
	}

	public override void OnDisable()
	{
		if (initialised)
		{
			lastActiveButton = currentButton;
			currentSelection = buttons[lastActiveButton];
		}

		base.OnDisable();
	}

	public void UpdateItems()
	{
		//Debug.Log(parent);
		// Change Button Icons in accordance to backpack data
		Backpack bp = parent.Player.Hero.Backpack;
		
		Item[] arrayItems = bp.AllItems;
		for (int i = 0; i < 7; ++i)
		{
			//Debug.Log("UpdateItems:"+i+" "+arrayItems[i]);
			if (arrayItems[i] != null)
			{
				Color temp = new Color();
				switch ((Item.ItemGrade)bp.AllItems[i].ItemStats.Grade)
				{
				case Item.ItemGrade.E:
					temp = Color.red;
					break;
				case Item.ItemGrade.D:
					temp = Color.magenta;
					break;
				case Item.ItemGrade.C:
					temp = Color.blue;
					break;
				case Item.ItemGrade.B:
					temp = Color.yellow;
					break;
				case Item.ItemGrade.A:
					temp = Color.cyan;
					break;
				case Item.ItemGrade.S:
					temp = Color.green;
					break;
				}
				buttons[i].transform.FindChild("Item").GetComponent<UISprite>().color = temp;
				NGUITools.SetActive(buttons[i].transform.FindChild("Item").gameObject, true);
			}
			else
			{
				
				NGUITools.SetActive(buttons[i].transform.FindChild("Item").gameObject, false);
			}
		}
	}

	UIButton GetButton(EButtons button)
	{
		return buttons[(int)button];
	}

	public override void OnMenuUp(InputDevice device)
	{
		UICamera.Notify(currentSelection.gameObject, "OnHover", false);

		currentSelection = PrevButton();

		UICamera.Notify(currentSelection.gameObject, "OnHover", true);
	}

	public override void OnMenuDown(InputDevice device)
	{
		UICamera.Notify(currentSelection.gameObject, "OnHover", false);

		currentSelection = NextButton();

		UICamera.Notify(currentSelection.gameObject, "OnHover", true);
	}
	
	public override void OnMenuLeft(InputDevice device)
	{
	}

	public override void OnMenuRight(InputDevice device)
	{		
	}

	public override void OnMenuOK(InputDevice device)
	{
	}


	public override void OnMenuCancel(InputDevice device)
	{
		// TODO: Link back to the main town screen

		//parent.CloseWindow();
	}
	
	public override void OnMenuHax(InputDevice device)
	{
		//((UITownScreen)parent.ParentScreen).StartGame();
		Game.Singleton.LoadLevel("Sewer_Levels", Game.EGameState.TowerRandom);
	}
}