﻿using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour 
{	
	public static MusicManager Instance;
	private static AudioClip towerMusic;
	private static AudioClip bossMusic;
	private static AudioClip menuMusic;

	private MusicSelections nextMusic;
	private MusicSelections currentMusic;

	float FadeDuration = 1f;
	float elapsedTime;
	public float MusicVolume = .02f;

	public enum State
	{
		Stop,
		In,
		Play,
		Out
	}

	public enum MusicSelections
	{
		None,
		Tower,
		Boss,
		Menu
	}
	
	State musicState = State.Stop;

	void Start()
	{
		if (Instance == null)
		{
			Instance = this;

			towerMusic = Resources.Load("Sounds/music/tower") as AudioClip;
			bossMusic = Resources.Load("Sounds/music/boss") as AudioClip;
			menuMusic = Resources.Load("Sounds/music/mainmenu") as AudioClip;
		}
	}

	void FixedUpdate()
	{
		elapsedTime += Time.fixedDeltaTime;
		switch (musicState)
		{
		case State.In:
			FadeInMusic();
			break;
		case State.Out:
			FadeOutMusic();
			break;
		case State.Stop:
			return;
		}
		
		if (!audio.isPlaying) audio.Play();
	}

	public void PlayMusic(MusicSelections choice, bool immediate = false)
	{
		if (currentMusic == choice)
		{
			return;
		}

		if (immediate)
		{
			SwapMusic(choice);
			currentMusic = choice;
			audio.Stop();
			audio.volume = MusicVolume;
			musicState = State.Play;
		}
		else
		{
			switch(musicState)
			{
			case State.Play:
				elapsedTime = 0f;
				musicState = State.Out;
				break;
			case State.In:
				elapsedTime = 0f;
				musicState = State.Out;
				break;
			case State.Stop:
				elapsedTime = 0f;
				audio.volume = 0f;
				SwapMusic(choice);
				currentMusic = choice;
				musicState = State.In;
				return;
			}
			nextMusic = choice;
		}
	}

	public void SetVolume(float val)
	{
		audio.volume = val;
	}

	public void SlowStop()
	{
		elapsedTime = 0f;
		nextMusic = MusicSelections.None;
		musicState = State.Out;
	}

	public void StopMusic()
	{
		musicState = State.Stop;
		audio.Stop();
		if (nextMusic != MusicSelections.None)
		{
			PlayMusic(nextMusic);
			currentMusic = nextMusic;
			nextMusic = MusicSelections.None;
		}
		else
		{
			currentMusic = MusicSelections.None;
		}
	}

	void FadeOutMusic()
	{
		audio.volume = Mathf.Lerp(MusicVolume, 0f, elapsedTime/FadeDuration);
		if (audio.volume <= 0f)
		{
			StopMusic();
		}
	}
	
	void FadeInMusic()
	{
		audio.volume = Mathf.Lerp(0f, MusicVolume, elapsedTime/FadeDuration);
		if (audio.volume >= MusicVolume)
		{
			audio.volume = MusicVolume;
			Play();
		}
	}

	[ContextMenu("Play")]
	public void Play()
	{
		musicState = State.Play;
		audio.Play();
	}

//	void OnMusicEnd()
//	{
//		print ("OnMusicEnd");
//		SwapMusic(nextMusic);
//		musicState = State.In;
//		audio.Play();
//	}
	
	void SwapMusic(MusicSelections choice)
	{
		audio.clip = ParseEnum(choice);
	}
	
	AudioClip ParseEnum(MusicSelections choice)
	{		
		AudioClip retval = null;
		switch (choice)
		{
		case MusicSelections.Tower:
			retval = towerMusic;
			MusicVolume = 0.03f;
			break;
		case MusicSelections.Boss:
			retval = bossMusic;
			MusicVolume = 0.06f;
			break;
		case MusicSelections.Menu:
			retval = menuMusic;
			MusicVolume = 0.035f;
			break;
		}
		return retval;
	}
}
