//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System.Collections.Generic;

public class UITownWindow : UIPlayerMenuWindow
{
//	public UIPlayerMenuPanel inventoryPanel;
//	public UIPlayerMenuPanel backpackPanel;
	public List<UIPlayerMenuPanel> TownPanels;

	public enum EBackpackTab
	{
		Accessory = 0,
		Consumable
	}

	EBackpackTab currentTab = EBackpackTab.Accessory;

	/// <summary>
	/// Affects the display and logic of Main and Inventory panel
	/// Also invokes the transition logic
	/// </summary>
	public EBackpackTab CurrentTab
	{
		get { return currentTab; }
		set 
		{
			currentTab = value;

			//gameObject.GetComponent<TweenColor>().PlayReverse();
			// TODO: Invoke transition logic here
		}
	}

	public enum EBackpackPanels
	{
		INVALID = -1,
		BACKPACK,
		INVENTORY,
		MAX
	}

	public override void Initialise ()
	{
		parentScreen = transform.parent.parent.parent.GetComponent<UIPlayerMenuScreen>();
		base.Initialise ();
	}

//	public void Equip(int destinationSlot, int originSlot)
//	{
//		if (ValidSlot(destinationSlot))
//		{
//			if (player.GetComponent<Hero>().HeroInventory.Items.Count >= originSlot)
//			{
//				Item insertingItem = player.GetComponent<Hero>().HeroInventory.Items[originSlot];
//				if (insertingItem != null)
//				{
//					Item returnItem = player.Hero.GetComponent<Hero>().HeroBackpack.ReplaceItem(destinationSlot, insertingItem);
//					player.GetComponent<Hero>().HeroInventory.Items.Insert(originSlot,returnItem);
//				}
//			}	
//		}
//	}

	/// <summary>
	/// Return item to inventory if space permits.
	/// </summary>
	/// <param name="slot">Slot.</param>
	public bool Unequip(int slot)
	{
		return true;
	}

	public override void TransitionToPanel(int index)
	{	
		activePanel.gameObject.SetActive(false);
		activePanel = panels[index];
		activePanel.gameObject.SetActive(true);
	}

	public override void AddAllMenuPanels()
	{
		panels[(int)EBackpackPanels.BACKPACK] = TownPanels[0];
		panels[(int)EBackpackPanels.INVENTORY] = TownPanels[1];

		for (int i = 0; i < panels.Count; ++i)
		{
			panels[i].gameObject.SetActive(false);
		}
		
		activePanel = panels[(int)EBackpackPanels.BACKPACK];
	}

//	bool ValidSlot(int slot)
//	{
//		if (slot > 3)
//		{
//			return false;
//		}
//		return true;
//	}
}

