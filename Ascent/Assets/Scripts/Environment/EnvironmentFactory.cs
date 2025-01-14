﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;

public enum MiscObjectType
{
    barrelCluster,
    barrel
}

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public static class EnvironmentFactory
{
#pragma warning disable 0414
	private static GameObject floorObject = Resources.Load("Prefabs/RoomModules/GroundTiles/Ground1") as GameObject;
    private static GameObject wallObject = Resources.Load("Prefabs/RoomWalls/Wall") as GameObject;
    private static GameObject wallCorner = Resources.Load("Prefabs/RoomWalls/WallCorner") as GameObject;
	private static GameObject wallWindow = Resources.Load("Prefabs/RoomWalls/WallWindow") as GameObject;
    private static GameObject doorObject = Resources.Load("Prefabs/RoomWalls/Door") as GameObject;
	private static GameObject barrelObject = Resources.Load("Prefabs/Environment/Barrel") as GameObject;
    private static GameObject barrelCluster = Resources.Load("Prefabs/Environment/BarrelCluster") as GameObject;
    private static GameObject brazierObject = Resources.Load("Prefabs/Environment/Brazier") as GameObject;
    private static GameObject pillarObject = Resources.Load("Prefabs/Environment/Pillar") as GameObject;
    private static GameObject arrowShooter = Resources.Load("Prefabs/Hazards/ArrowShooter") as GameObject;
    private static GameObject spinningBlade = Resources.Load("Prefabs/Hazards/SpinningBlade") as GameObject;
    private static GameObject chest = Resources.Load("Prefabs/Environment/Chest") as GameObject;

    private static UnityEngine.Object[] listGroundTiles = new UnityEngine.Object[0];


    static EnvironmentFactory()
    {
        LoadResources();
    }

    public static void LoadResources()
    {
        listGroundTiles = Resources.LoadAll("Prefabs/RoomModules/GroundTiles");
    }

    /// <summary>
    /// Loads and instantiates prefab from the prefab folder in resources.
    /// </summary>
    /// <param name="envPath">The path to the prefab.</param>
    /// <param name="asInstance">If this is true it will instantiate an instanced prefab.</param>
    /// <returns></returns>
    public static GameObject InstantiateEnvPrefab(string envPath, bool asInstance)
    {
        string prefabPath = "Prefabs/" + envPath;

        GameObject go = Resources.Load(prefabPath) as GameObject;

        if (go != null)
        {
            if (asInstance)
            {
				#if UNITY_EDITOR
                go = PrefabUtility.InstantiatePrefab(go) as GameObject;
#endif
            }
            else
            {
                go = GameObject.Instantiate(go) as GameObject;
            }

            return go;
        }
        else
        {
            Debug.LogWarning("Prefab was not found at location " + prefabPath);
        }

        return go;
    }

    public static GameObject CreateMiscObject(MiscObjectType type)
    {
        GameObject go = null;

		#if UNITY_EDITOR
        switch (type)
        {
            case MiscObjectType.barrel:
                go = UnityEditor.PrefabUtility.InstantiatePrefab(barrelObject) as GameObject;
                go.name = barrelObject.name;
                break;

            case MiscObjectType.barrelCluster:
                go = UnityEditor.PrefabUtility.InstantiatePrefab(barrelCluster) as GameObject;
                go.name = barrelCluster.name;
                break;
        }
#endif
        return go;
    }

    /// <summary>
    /// Returns a newly created object based on the type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static GameObject CreateGameObjectByType(EnvironmentID type)
    {
        GameObject go = null;
		#if UNITY_EDITOR
        switch (type)
        {
            case EnvironmentID.groundTile:
                {
                    LoadResources();

                    if (listGroundTiles != null)
                    {
                        int random = UnityEngine.Random.Range(0, listGroundTiles.Length);
                        UnityEngine.Object floorObj = listGroundTiles[random];
                        go = UnityEditor.PrefabUtility.InstantiatePrefab(floorObj) as GameObject;
                        go.name = floorObject.name;
                    }
                }
                break;

            case EnvironmentID.standardWall:
                go = UnityEditor.PrefabUtility.InstantiatePrefab(wallObject) as GameObject;
                go.name = wallObject.name;
                break;

            case EnvironmentID.cornerWallTile:
                go = UnityEditor.PrefabUtility.InstantiatePrefab(wallCorner) as GameObject;
                go.name = wallCorner.name;
                break;

            case EnvironmentID.brazier:
                go = UnityEditor.PrefabUtility.InstantiatePrefab(brazierObject) as GameObject;
                go.name = brazierObject.name;
                break;

            case EnvironmentID.chest:
                go = UnityEditor.PrefabUtility.InstantiatePrefab(chest) as GameObject;
                go.name = chest.name;
                break;

            case EnvironmentID.pillar:
                //go = GameObject.Instantiate(pillarObject, Vector3.zero, pillarObject.transform.rotation) as GameObject;
                go = UnityEditor.PrefabUtility.InstantiatePrefab(pillarObject) as GameObject;
                go.name = pillarObject.name;
                break;

            case EnvironmentID.door:
                go = UnityEditor.PrefabUtility.InstantiatePrefab(doorObject) as GameObject;
                go.name = doorObject.name;
                break;

            case EnvironmentID.arrowShooter:
                go = UnityEditor.PrefabUtility.InstantiatePrefab(arrowShooter) as GameObject;
                go.name = arrowShooter.name;
                break;

            case EnvironmentID.spinningBlade:
                go = UnityEditor.PrefabUtility.InstantiatePrefab(spinningBlade) as GameObject;
                go.name = spinningBlade.name;
                break;
        }
#endif
        return go;
    }
}
