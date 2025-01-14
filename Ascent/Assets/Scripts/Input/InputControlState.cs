﻿using System;
#pragma warning disable 0660, 0661

public struct InputControlState
{
	public bool State;
	public float Value;


	public void Set(float value)
	{
		Value = value;
		State = value != 0.0f;
	}


	public void Set(bool state)
	{
		State = state;
		Value = state ? 1.0f : 0.0f;
	}


	public static implicit operator bool(InputControlState state)
	{
		return state.State;
	}


	public static implicit operator float(InputControlState state)
	{
		return state.Value;
	}


	public static bool operator ==(InputControlState a, InputControlState b)
	{
		return a.Value == b.Value;
	}


	public static bool operator !=(InputControlState a, InputControlState b)
	{
		return a.Value != b.Value;
	}
}