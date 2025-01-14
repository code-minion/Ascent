﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Tile
{
    [NonSerialized]
    private Vector3 position;
    [NonSerialized]
    private GameObject gameObject;

    // TODO: Get rid of this as this tile is just a place holder.
    private bool isOccupied;

    // Serialized fields.
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }

    public Directions FacingDirection { get; set; }

    // These attributes could be any objects in the game.
    public List<TileAttribute> TileAttributes { get; set; }

    // Non Serialized
    public Vector3 Position
    {
        get { return new Vector3(PosX, PosY, PosZ); }
        set 
        {
            position = value; 
            PosX = position.x;
            PosY = position.y;
            PosZ = position.z;
        }
    }

    /// <summary>
    /// The game object associated with this tile.
    /// </summary>
    public GameObject GameObject
    {
        get { return gameObject; }
        set { gameObject = value; }
    }

    public bool IsOccupied
    {
        get { return isOccupied; }
        set { isOccupied = value; }
    }

    public bool ContainsAttribute(EnvironmentID type)
    {
        foreach (TileAttribute att in TileAttributes)
        {
            if (att.Type == type)
            {
                return true;
            }
        }

        return false;
    }
}