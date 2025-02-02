﻿using UnityEngine;
using System.Collections;


public class HeroButtonIndicator : MonoBehaviour
{
	private enum EState
	{
		Standard,
		Growing,
		Shrinking,
	}

	public UISprite buttonSprite;
	public Character owner;

	private EState state;
	private float timeElapsed;
	private float growTime = 0.15f;
	private float shrinkTime = 0.09f;

	private float maxScale = 3.25f;
	private float prevScale;

	public void Initialise(Character _character)
	{
		buttonSprite.transform.localScale = Vector3.zero;
		owner = _character;

		Player player = Game.Singleton.GetPlayer(((Hero)owner));
		
		bool isKeyboard = !player.Input.isJoystick;
		
		if (isKeyboard)
		{
			buttonSprite.spriteName = "keyF";
		}
	}

	public void Update()
	{
		// Shrink or Grow
		switch (state)
		{
			case EState.Standard:
				{
					// Do nothing;
				}
				break;
			case EState.Growing:
				{		
					// Update timer for lerping scale
					timeElapsed += Time.deltaTime;
					if (timeElapsed >= growTime)
					{
						timeElapsed = growTime;
					}

					buttonSprite.cachedTransform.localScale = Vector3.Lerp((Vector3.one * maxScale) * 0.1f, Vector3.one * maxScale, timeElapsed / growTime);
				}
				break;
			case EState.Shrinking:
				{
					// Update timer for lerping scale
					timeElapsed += Time.deltaTime;
					if (timeElapsed >= shrinkTime)
					{
						timeElapsed = shrinkTime;
					}

					buttonSprite.cachedTransform.localScale = Vector3.Lerp(Vector3.one * prevScale, Vector3.zero, timeElapsed / shrinkTime);
				}
				break;
			default:
				{
					Debug.LogError("Unhandled case: " + state);
				}
				break;
		}

		// Track the target
		Vector3 screenPos = Game.Singleton.Tower.CurrentFloor.MainCamera.WorldToViewportPoint(owner.transform.position);
		screenPos.y += 0.11f;
		Vector3 barPos = FloorHUDManager.Singleton.hudCamera.ViewportToWorldPoint(screenPos);
		barPos = new Vector3(barPos.x, barPos.y);
		buttonSprite.transform.position = barPos;
	}

	public void Enable(bool b)
	{
		if (b && state != EState.Growing)
		{
			state = EState.Growing;

			timeElapsed = 0.0f;
		}
		else if(b == false)
		{
			// Set timer to what is remaining.
			prevScale = buttonSprite.cachedTransform.localScale.x;
			timeElapsed = shrinkTime - ((prevScale / maxScale) * shrinkTime);

			state = EState.Shrinking;
		}
	}
}
