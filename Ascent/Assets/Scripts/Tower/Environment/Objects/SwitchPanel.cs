﻿using UnityEngine;
using System.Collections;

public class SwitchPanel : MonoBehaviour 
{
	bool isDown = false;
	public bool IsDown
	{
		get
		{
			return isDown;
		}
		set
		{
			isDown = value;
			SoundManager.PlaySound(AudioClipType.switchclick, transform.position, 1f);
		}
	}

	void OnCollisionStay(Collision collision)
	{
		gameObject.renderer.material.color = Color.green;
		IsDown = true;
	}

	void OnCollisionExit(Collision collision)
	{
		gameObject.renderer.material.color = Color.red;
		IsDown = false;
	}
}
