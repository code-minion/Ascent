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
using UnityEngine;

public class UITownWindow : UIPlayerMenuWindow
{
	public Transform pointerTransform;
	public List<UIPlayerMenuPanel> TownPanels;
	protected float pointerAngle = 90f;
	public float PointerAngle
	{
		get 
		{
			return pointerAngle;
		}
	}

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
		//parentScreen = transform.parent.parent.parent.GetComponent<UIPlayerMenuScreen>();
		OnMenuLeftStickMove += HandleOnMenuLeftStickMove;
		base.Initialise ();
	}

	void HandleOnMenuLeftStickMove (InputDevice device)
	{		
		pointerAngle = Utilities.VectorToAngleInDegrees(device.LeftStickX.Value,device.LeftStickY.Value);
		//Debug.Log ("AnalogX:" + device.LeftStickX.Value + " AnalogY:" + device.LeftStickY.Value + " Angle:" + angle);
		pointerTransform.rotation = Quaternion.Euler(0f,0f,pointerAngle - 90f);
	}

//	protected override void HandleInputEvents()
//	{
//		base.HandleInputEvents();
//
//		if (leftStickSignificantMovement)
//		{
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
			panels[i].SetParent(this);
			panels[i].Initialise();
			panels[i].gameObject.SetActive(false);
		}
		
		activePanel = panels[(int)EBackpackPanels.BACKPACK];
		player.activePlayerPanel = activePanel;
		NGUITools.SetActive(activePanel.gameObject,true);
	}

}

