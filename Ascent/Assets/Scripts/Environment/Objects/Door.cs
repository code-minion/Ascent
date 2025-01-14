﻿using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR 
using UnityEditor;
#endif

public class Door : EnvironmentBreakable
{
	public Floor.TransitionDirection direction;

	public Door targetDoor;
    public bool isConnected;
    public bool isFinalDoor;

    public bool isEntryDoor = false;

    public Collider immediateArea;

    public GameObject openedDoor;
    public GameObject sealedDoor;
    public Transform spawnLocation;

    private float standingOnDoorTimer = 0.0f;

	protected bool[] playersLeftDoor;

	private bool startDoor = false;

    private DoorLockIndicator doorLockInidicator;
    public Transform lockDoorIndicatorPosition;

	private bool isOpen;

	public bool IsOpen
	{
		get { return isOpen; }
		set { isOpen = value; }
	}

	public bool StartDoor
	{
		get { return startDoor; }
		set { startDoor = value; }
	}

	protected bool walkedOutOfTheDoor;

#if UNITY_EDITOR 
	public void OnDrawGizmos()
	{
		if (targetDoor != null)
		{
			Vector3 dir = (targetDoor.transform.position - transform.position).normalized;

			Vector3 a = transform.position + dir * 7.5f;
			Vector3 b = targetDoor.transform.position - dir * 9.0f;

			a.y = 2.5f;
			b.y = 2.5f;

			Gizmos.DrawLine(a, b);
			Handles.ArrowCap(0, a, Quaternion.LookRotation(dir, Vector3.up), 1.5f);

			a = transform.position;
			a.y = 5.0f;

			return;
		}

		if (isFinalDoor)
		{
			Vector3 a = transform.position;
			//Vector3 b = a + Vector3.up * 10f;

			//Gizmos.DrawLine(a,b);
			Handles.ArrowCap(0, a, Quaternion.LookRotation(Vector3.up), 8f);
		}
	}
#endif

	public void Start()
	{
		if (openedDoor.activeInHierarchy)
		{
			isOpen = true;
		}
		else
		{
			isOpen = false;
		}
	}

	public virtual void OnEnable()
	{
        if (Game.Singleton.Tower.CurrentFloor != null)
        {
            walkedOutOfTheDoor = false;
            playersLeftDoor = new bool[Game.Singleton.Players.Count];

            if (doorLockInidicator == null)
            {
                GameObject buttonIndicatorGO = NGUITools.AddChild(FloorHUDManager.Singleton.mainPanel.gameObject, Resources.Load("Prefabs/UI/DoorLockIndicator") as GameObject);
                doorLockInidicator = buttonIndicatorGO.GetComponent<DoorLockIndicator>();
                doorLockInidicator.Initialise(lockDoorIndicatorPosition);

                doorLockInidicator.Enable(false);

                // Set scale similar to the character size
                doorLockInidicator.transform.localScale = Vector3.one * 2.5f;
            }
        }
	}

	public void Process()
	{
		if (startDoor)
		{
			// Wait for all players to get out before enabling self
			int countPlayersLeftDoor = 0;
			for (int i = 0; i < playersLeftDoor.Length; ++i)
			{
				if (!playersLeftDoor[i])
				{
					if (!Game.Singleton.Players[i].Hero.collider.bounds.Intersects(collider.bounds))
					{
						playersLeftDoor[i] = true;
						++countPlayersLeftDoor;
					}
				}
				else
				{
					++countPlayersLeftDoor;
				}
			}

			if (countPlayersLeftDoor == playersLeftDoor.Length)
			{
				walkedOutOfTheDoor = false;
				startDoor = false;
			}
		}

        if (targetDoor != null || isFinalDoor)
        {
			int playerCount = Game.Singleton.AlivePlayerCount;

			// If only 1 player then they can quickly transition between rooms
			if (playerCount == 1)
			{
                foreach (Player p in Game.Singleton.Players)
                {
                    if (!p.Hero.IsDead)
                    {
                       if(p.Hero.collider.bounds.Intersects(immediateArea.bounds))
                       {
                           if (isFinalDoor)
							{
								GoThroughDoorToNextFloor();
                           }
                           else
                           {
                               if (targetDoor != null)
                               {
                                   Game.Singleton.Tower.CurrentFloor.TransitionToRoom(targetDoor);
                               }
                           }
                       }
                    }
                }
			}

            int currentPlayerCount = 0;

            // Check if all heroes are in here.
            foreach (Player p in Game.Singleton.Players)
            {
                if (p.Hero.collider.bounds.Intersects(collider.bounds))
                {
                    ++currentPlayerCount;
                }
            }

            if (currentPlayerCount > 0 &&
                currentPlayerCount == playerCount)
            {
				if (!walkedOutOfTheDoor)
				{
					standingOnDoorTimer += Time.deltaTime;
					if (standingOnDoorTimer >= 0.5f)
					{
                        if (isFinalDoor)
						{
							GoThroughDoorToNextFloor();
                        }
                        else
                        {
                            if (targetDoor != null)
                            {
                                Game.Singleton.Tower.CurrentFloor.TransitionToRoom(targetDoor);
                            }
                        }

						walkedOutOfTheDoor = true;
					}
				}
				else
				{
					// If one player is standing on the immediate area then a transition can occur immediately.
					foreach (Player p in Game.Singleton.Players)
					{
						if (p.Hero.collider.bounds.Intersects(immediateArea.bounds))
						{
                            if (isFinalDoor)
                            {
								GoThroughDoorToNextFloor();
                            }
                            else
                            {
                                if (targetDoor != null)
                                {
                                    Game.Singleton.Tower.CurrentFloor.TransitionToRoom(targetDoor);
                                }
                            }
							walkedOutOfTheDoor = true;
						}
					}
				}
            }
            else
            {
                standingOnDoorTimer = 0.0f;
            }
        }
	}

	public void GoThroughDoorToNextFloor()
	{
		FloorHUDManager.Singleton.LevelCompleteScreen();
		Game.Singleton.Tower.CurrentFloor.gameOver = true;
	}

	public void SetAsStartDoor()
	{
		startDoor = true;
		walkedOutOfTheDoor = true;
	}

    [ContextMenu("OpenDoor")]
    public void OpenDoor()
    {
		isOpen = true;
        openedDoor.SetActive(true);
        sealedDoor.SetActive(false);
		doorLockInidicator.Enable(false);
		SoundManager.PlaySound(AudioClipType.dooropen, transform.position, 0.5f);
    }

    [ContextMenu("CloseDoor")]
    public void CloseDoor()
    {
		isOpen = false;
        openedDoor.SetActive(false);
        sealedDoor.SetActive(true);
        doorLockInidicator.Enable(true);
		//SoundManager.PlaySound(AudioClipType.dooropen, transform.position, 1f);
    }
}
