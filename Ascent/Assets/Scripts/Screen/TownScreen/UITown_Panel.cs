//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

public class UITown_Panel : UIPlayerMenuPanel
{
	protected Dictionary<float, int> AngleIndex;
	static float ANGLETOLERANCE = 20f;

	protected bool CloseTo(float a, float b)
	{
		return (Mathf.Abs(a - b) <= ANGLETOLERANCE);
	}

	public override void Initialise ()
	{
		base.Initialise ();
		
		AngleIndex = new Dictionary<float, int>();
	}

	protected void HighlightButton ()
	{
		float angle = (parent as UITownWindow).PointerAngle - 90f;
		
		foreach (KeyValuePair<float,int> p in AngleIndex)
		{
			Debug.Log("Testing Angle:" + angle + " against:" + p.Key);
			if (CloseTo(angle,p.Key))
			{
				Debug.Log("WIN!! Angle:" + angle + " against:" + p.Key);
				UICamera.Notify(currentSelection.gameObject, "OnHover", false);
				currentSelection = buttons[p.Value];
				currentHighlightedButton = p.Value;
				UICamera.Notify(currentSelection.gameObject, "OnHover", true);
			}
			else
			{
				Debug.Log("FAIL!! Angle:" + angle + " against:" + p.Key);
			}
		}
	}
}


