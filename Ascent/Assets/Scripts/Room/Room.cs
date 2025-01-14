﻿using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// The room class should be attached to every room on a floor. It contains references to all of the objects in the room.
/// </summary>
public class Room : MonoBehaviour
{
	public enum ERoomObjects
	{
		INVALID = -1,

		Enemy,
		Chest,
		Loot,
        Barrel,
		MAX,
	}

    public enum EMonsterTypes
    {
        Rat,
        Slime,
        Imp,
        EnchantedStatue,
        Abomination,
        Boss,

        MAX
    }

    #region Fields

    private Dictionary<int, GameObject> parentRootNodes = new Dictionary<int, GameObject>();
    private int numberOfTilesX;
    private int numberOfTilesY;
    public bool restrictCamera = false;

	public float maxLeft = -1.0f;
	public float maxRight = 1.0f;
	public float maxTop = 1.0f;
	public float maxBottom = -9.0f;

	private float maxLeftOld = 0.0f;
	private float maxRightOld = 0.0f;
	private float maxTopOld = 0.0f;
	private float maxBottomOld = 0.0f;
	
	[HideInInspector]
	public float cameraHeight = 20.0f;
	
	[HideInInspector]
	public float cameraOffsetZ = 0.35f;

	protected Doors doors;
	[HideInInspector]
    public bool startRoom = false;

    protected List<Character> enemies = new List<Character>();
    protected List<TreasureChest> chests = new List<TreasureChest>();
    protected List<LootDrop> lootDrops = new List<LootDrop>();
    protected List<MoveableBlock> moveables = new List<MoveableBlock>();
    protected List<Shrine> shrines = new List<Shrine>();
    protected List<EnvironmentBreakable> breakables = new List<EnvironmentBreakable>();
	protected List<Interactable> interactables = new List<Interactable>();

    public Doors Doors
    {
        get { return doors; }
        set { doors = value; }
    }
    
    private Door entryDoor;
    public Door EntryDoor
    {
        get { return entryDoor; }
        set { entryDoor = value; }
    }

    public List<Character> Enemies
    {
        get { return enemies; }
    }

	public List<Character> AliveEnemies
	{
		get 
		{
			List<Character> aliveEnemies = new List<Character>();

			foreach (Character c in enemies)
			{
				if (!c.IsDead)
					aliveEnemies.Add(c);
			}

			return aliveEnemies; 
		}
	}

	public List<TreasureChest> Chests
	{
		get { return chests; }
	}

	public List<LootDrop> LootDrops
	{
		get { return lootDrops; }
	}

	public List<MoveableBlock> Moveables
	{
		get { return moveables; }
	}

    public List<Shrine> Shrines
    {
        get { return shrines; }
    }

    public List<EnvironmentBreakable> Breakables
    {
        get { return breakables; }
    }

    protected RoomFloorNav navMesh;
	public RoomFloorNav NavMesh
	{
		get 
        {
            if (navMesh == null)
            {
				navMesh = gameObject.GetComponentInChildren<RoomFloorNav>();

				if (navMesh == null)
				{

					GameObject go = GameObject.Instantiate(Resources.Load("Prefabs/Environment/RoomNav")) as GameObject;
					go.transform.position = transform.position + go.transform.position;
					go.transform.parent = transform;

					navMesh = go.GetComponent<RoomFloorNav>();
				}
                
            }
            return navMesh; 
        }
		set { navMesh = value; }
	}

    private Transform environmentNode;
    private Transform monstersNode;

    public Transform MonsterParent
    {
        get
        {
            if (monstersNode == null)
            {
                monstersNode = GetNodeByLayer("Monster").transform;

                if (monstersNode != null)
                {
                    return monstersNode.transform;
                }
                else
                {
                    monstersNode = AddNewParentCategory("Monsters", (int)Layer.Monster).transform;
                    return monstersNode;
                }
            }
            else
            {
                return monstersNode;
            }
        }
    }

    public Transform EnvironmentParent
    {
        get
        {
            if (environmentNode == null)
            {
                environmentNode = GetNodeByLayer("Environment").transform;

                if (environmentNode != null)
                {
                    return environmentNode;
                }
                else
                {
					environmentNode = AddNewParentCategory("Environment", (int)Layer.Environment).transform;
                    return environmentNode;
                }
            }
            else
            {
                return environmentNode;
            }
        }
    }

    #endregion

	public void Initialise()
	{
		// Make sure all the nodes are found and references.
		FindAllNodes();

        // Find the monsters for this room
        monstersNode = GetNodeByLayer("Monster").transform;

        if (monstersNode == null)
        {
            // Obviously it does not exist so we can create one.
			monstersNode = AddNewParentCategory("Monsters", (int)Layer.Monster).transform;
            Debug.LogWarning("Could not find Monsters GameObject in Room. Creating one now.", gameObject);
        }

		navMesh = NavMesh;

        // Find the doors for this room
        doors = EnvironmentParent.GetComponentInChildren<Doors>();

        if (doors == null)
        {
            Debug.LogWarning("Could not find any doors for this room");
        }

		// Find and populate all the lists of objects in the room.
		PopulateListOfObjects(ref enemies, gameObject);
		PopulateListOfObjects(ref breakables, gameObject);
		PopulateListOfObjects(ref moveables, gameObject);
		PopulateListOfObjects(ref chests, gameObject);
		PopulateListOfObjects(ref shrines, gameObject);

		// Doors
		doors.Initialise();

		// Add Blocks
		foreach (Interactable i in moveables)
		{
			interactables.Add(i);
		}

		// Add Shrines
		foreach (Interactable i in shrines)
		{
			interactables.Add(i);
		}

		// Add Chests
		foreach (Interactable i in chests)
		{
			interactables.Add(i);
		}

		if (chests != null)
		{
			if (lootDrops == null)
			{
				foreach (TreasureChest c in chests)
				{
					LootDrop[] roomLoot = c.gameObject.GetComponentsInChildren<LootDrop>() as LootDrop[];

					if (roomLoot.Length > 0)
					{
						lootDrops = new List<LootDrop>();

						foreach (LootDrop t in roomLoot)
						{
							if (!lootDrops.Contains(t))
							{
								lootDrops.Add(t);
							}
						}
					}
				}
			}
		}
	}

    public void OnEnable()
    {
		if (Game.Singleton.Tower.CurrentFloor != null && Game.Singleton.Tower.CurrentFloor.initialised)
		{
			FloorCamera camera = Game.Singleton.Tower.CurrentFloor.FloorCamera;
			if (camera != null)
			{
				camera.Restrict = restrictCamera;
				camera.minCamera = transform.position + new Vector3(maxLeft, 0.0f, maxBottom);
				camera.maxCamera = transform.position + new Vector3(maxRight, 0.0f, maxTop);

				maxLeftOld = maxLeft;
				maxRightOld = maxRight;
				maxBottomOld = maxBottom;
				maxTopOld = maxTop;
			}
		}
    }

    public void RotateFacingDirection(float angle)
    {
        transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), angle);
    }

    /// <summary>
    /// Pass in a list to populate it with found objects of that type in the parent.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <param name="populateList">The list to populate.</param>
    /// <param name="parent">The parent to get components from children.</param>
    private void PopulateListOfObjects<T>(ref List<T> populateList, GameObject parent) where T: Component
    {
        T[] foundObjects = parent.GetComponentsInChildren<T>();

        if (foundObjects != null && foundObjects.Length > 0)
        {
            populateList = new List<T>();

            foreach (T type in foundObjects)
            {
                if (!populateList.Contains(type))
                {
                    Enemy enemy = type as Enemy;

                    if (enemy != null)
                    {
                        enemy.ContainedRoom = this;

                        if (enemy.spawnOnLoad == false)
                        {
                            enemy.gameObject.SetActive(false);
                        }
                    }

                    populateList.Add(type);
                }
            }
        }
    }

	void Update()
	{
		CheckDoors();

		if (maxLeftOld != maxLeft ||
			maxRightOld != maxRight ||
			maxBottomOld != maxBottom ||
			maxTopOld != maxTop)
		{
			FloorCamera camera = Game.Singleton.Tower.CurrentFloor.FloorCamera;
			if (camera == null)
				return;
				
			camera.minCamera = transform.position + new Vector3(maxLeft, 0.0f, maxBottom);
			camera.maxCamera = transform.position + new Vector3(maxRight, 0.0f, maxTop);

			maxLeftOld = maxLeft;
			maxRightOld = maxRight;
			maxBottomOld = maxBottom;
			maxTopOld = maxTop;
		}
	}

    public void SetNavMeshDimensions(float width, float height)
    {
        NavMesh.transform.localScale = new Vector3(width - 1.0f, height - 1.0f, 0.0f);
    }

	void CheckDoors()
	{
        if (doors == null)
        {
            Debug.Log("Doors null");
            return;
        }

		List<Door> roomDoors = doors.RoomDoors;

		for (int i = 0; i < roomDoors.Count; ++i)
		{
			if (roomDoors[i] != null)
			{
				roomDoors[i].Process();
			}
		}
	}

    /// <summary>
    /// Adds a root node child to the room tree. This will serve as a transform for adding items
    /// of same layer type to.
    /// </summary>
    /// <param name="name">The name of the node</param>
    /// <param name="layer">The layer objects that the node holds</param>
    public GameObject AddNewParentCategory(string name, int layer)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = transform;
        go.tag = "RoomNode";
        go.layer = layer;

        if (!parentRootNodes.ContainsKey(layer))
        {
            parentRootNodes.Add(layer, go);
        }

        return go;
    }

    public GameObject AddSubParent(string name, GameObject parent, int layer)
    {
        GameObject go = null;

        if (parent != null)
        {
            go = new GameObject(name);
            go.transform.parent = parent.transform;
            go.tag = "RoomNode";
            go.layer = layer;
        }

        return go;
    }

    /// <summary>
    /// Gets the node associated with the given layer. These nodes are used for organizing 
    /// objects of same layer into 1 node for the room.
    /// </summary>
    /// <param name="layer">The layer of the node we want to retrieve.</param>
    /// <returns></returns>
    public GameObject GetNodeByLayer(string layer)
    {
        GameObject go = null;
        int nameLayer = LayerMask.NameToLayer(layer);

        if (parentRootNodes.ContainsKey(nameLayer))
        {
            go = parentRootNodes[nameLayer];

            if (go == null)
            {
                Debug.Log("The node for layer: " + layer + " does not exist");
            }
        }
        else
        {
            Debug.Log("Could not find node with layer " + layer + 
                " you may want to use AddNewParentCategory to add it");
        }

        return go;
    }

    /// <summary>
    /// Fixes the tree structure by categorizing all objects into layers and nodes.
    /// TODO: Implement this function.
    /// Note: This function could be potentially very slow.
    /// </summary>
    public void FixTreeStructure()
    {
        foreach (Transform T in transform)
        {
            // If the object is not a parent container we need to put it in the right place.
            if (T.tag != "RoomNode")
            {
                // Find out if the objects layer exists in here.
                if (!parentRootNodes.ContainsKey(T.gameObject.layer))
                {
                    // We should create this layer node
                    // and assign the transform to this 
                    GameObject go = AddNewParentCategory(LayerMask.LayerToName(T.gameObject.layer), T.gameObject.layer);
                    T.parent = go.transform;
                }
                else
                {
                    GameObject go = GetNodeByLayer(LayerMask.LayerToName(T.gameObject.layer));
                    T.parent = go.transform;
                }

                return;
            }
        }
    }

    /// <summary>
    /// Helper function to find all the nodes and add them to 
    /// the dictionairy of node references.
    /// </summary>
    public void FindAllNodes()
    {
        // We want to get the second level nodes from this room object.
        foreach (Transform T in transform)
        {
            if (!parentRootNodes.ContainsKey(T.gameObject.layer))
            {
                // We want to make sure that we have a reference to 
                // all of the nodes in our room object.
                parentRootNodes.Add(T.gameObject.layer, T.gameObject);
            }
        }
    }

	public GameObject InstantiateGameObject(ERoomObjects type, string name)
	{
		GameObject newObject = null;


		switch (type)
		{
			case ERoomObjects.Chest:
				{
#if UNITY_EDITOR
					newObject = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/Environment/" + name)) as GameObject;
#else
					newObject = GameObject.Instantiate(Resources.Load("Prefabs/Environment/" + name)) as GameObject;
#endif
                    if (chests == null)
                    {
                        chests = new List<TreasureChest>();
                    }

                    TreasureChest chest = newObject.GetComponent<TreasureChest>();
                    chest.transform.parent = this.transform;
                    chest.transform.localPosition = Vector3.zero;
                    chests.Add(chest);
				}
				break;
			case ERoomObjects.Loot:
				{

#if UNITY_EDITOR
					newObject = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/Environment/" + name)) as GameObject;
#else
					newObject = GameObject.Instantiate(Resources.Load("Prefabs/Environment/" + name)) as GameObject;
#endif
                    if (lootDrops == null)
					{
						lootDrops = new List<LootDrop>();
					}

					lootDrops.Add(newObject.GetComponent<LootDrop>());
				}
				break;
			case ERoomObjects.Enemy:
				{
#if UNITY_EDITOR
					newObject = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/Enemies/" + name)) as GameObject;
#else
					newObject = GameObject.Instantiate(Resources.Load("Prefabs/Enemies/" + name)) as GameObject;
#endif
                    if (enemies == null)
                    {
                        enemies = new List<Character>();
                    }

                    Enemy enemy = newObject.GetComponent<Enemy>();
                    enemies.Add(enemy);
                    enemy.transform.parent = monstersNode.transform;
                    enemy.transform.localPosition = Vector3.zero;
                    enemy.ContainedRoom = this;

                    if (Game.Singleton != null)
                    {
                        enemy.Initialise();
                        enemy.InitiliseHealthbar();
                    }
				}
				break;

            case ERoomObjects.Barrel:
                {
#if UNITY_EDITOR
					newObject = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/Enemies/" + name)) as GameObject;
#else
					newObject = GameObject.Instantiate(Resources.Load("Prefabs/Enemies/" + name)) as GameObject;
#endif
                    newObject.transform.parent = EnvironmentParent;
                }
                break;
		}

		return newObject;
	}


	public GameObject InstantiateGameObject(string name)
	{
		GameObject newObject = null;

		newObject = GameObject.Instantiate(Resources.Load(name)) as GameObject;

		Vector3 localPosition = newObject.transform.position;
		Vector3 localScale = newObject.transform.localScale;
		Quaternion localRotation = newObject.transform.localRotation;

		newObject.transform.parent = this.transform;
		newObject.transform.position = localPosition;
		newObject.transform.localScale = localScale;
		newObject.transform.rotation = localRotation;

		return newObject;
	}

    public void RemoveObject(ERoomObjects type, GameObject go)
	{
		switch(type)
		{
			case ERoomObjects.Loot:
				{
					lootDrops.Remove(go.GetComponent<LootDrop>());
					Destroy(go);
				}
				break;
		}
	}

    public void ProcessCollisionBreakables(Shape2D shape)
    {
        Shape2D.EType type = shape.type;

        switch (type)
        {
            case Shape2D.EType.Circle:
                {
                    foreach (EnvironmentBreakable b in breakables)
                    {
                        if (b.IsDestroyed || b.IsBreakable == false)
                        {
                            continue;
                        }

                        if (CheckCircle(shape as Circle, b.GetComponentInChildren<Collider>()))
                        {
                            // Destroy the breakable.
                            b.BreakObject();
                        }
                    }
                }
                break;
            case Shape2D.EType.Arc:
                {
                    foreach (EnvironmentBreakable b in breakables)
                    {
                        if (b.IsDestroyed)
                        {
                            continue;
                        }

                        if (CheckArc(shape as Arc, b.GetComponentInChildren<Collider>()))
                        {
                            // Destroy the breakable.
                            b.BreakObject();
                        }
                    }
                }
                break;
            default:
                {
                    Debug.LogError("No more shapezies for you");
                }
                break;
        }
    }

	public bool CheckCollisionArea(Shape2D shape, Character.EScope scope, ref List<Character> charactersColliding)
	{
		Shape2D.EType type = shape.type;

		List<Character> characters = GetCharacterList(scope);

		if (characters == null || characters.Count == 0)
		{
			return false;
		}

		switch(type)
		{
			case Shape2D.EType.Circle:
				{
					foreach (Character c in characters)
                    {
						if (c.IsDead)
                        {
                            continue;
                        }

						if (CheckCircle(shape as Circle, c.gameObject))
                        {
							if (!charactersColliding.Contains(c))
							{
								charactersColliding.Add(c);
							}
                        }
                    }
				}
				break;
			case Shape2D.EType.Arc:
				{
					foreach (Character c in characters)
					{
						if(c.IsDead)
						{
							continue;
						}					

						if(CheckArc(shape as Arc, c.gameObject))
						{
							if (!charactersColliding.Contains(c))
							{
								charactersColliding.Add(c);
							}
						}
					}
				}
				break;
			default:
				{
					Debug.LogError("Unhandled case");
				}
				break;
		}


		return charactersColliding.Count > 0;
	}

	public bool CheckCollisionArea(Shape2D shape, ref List<Interactable> interactablesInArea)
	{
		Shape2D.EType type = shape.type;

		if (interactables == null || interactables.Count == 0)
		{
			return false;
		}

		switch (type)
		{
			case Shape2D.EType.Circle:
				{
					foreach (Interactable i in interactables)
					{
						if (CheckCircle(shape as Circle, i.gameObject))
						{
							if (!interactablesInArea.Contains(i))
							{
								interactablesInArea.Add(i);
							}
						}
					}
				}
				break;
			case Shape2D.EType.Arc:
				{
					foreach (Interactable i in interactables)
					{
						if (CheckArc(shape as Arc, i.gameObject))
						{
							if (!interactablesInArea.Contains(i))
							{
								interactablesInArea.Add(i);
							}
						}
					}
				}
				break;
			default:
				{
					Debug.LogError("Unhandled case");
				}
				break;
		}


		return interactablesInArea.Count > 0;
	}

	public List<Character> GetCharacterList(Character.EScope scope)
	{
		List<Character> characters = null;

		switch(scope)
		{
			case Character.EScope.All:
				{
					List<Player> players = Game.Singleton.Players;

					if (enemies != null)
					{
						characters = new List<Character>(enemies);

						foreach (Player p in players)
						{
							characters.Add(p.Hero);
						}
					}
					else
					{
						characters = new List<Character>();

						foreach (Player p in players)
						{
							characters.Add(p.Hero);
						}
					}
				}
				break;
			case Character.EScope.Enemy:
				{
					if (enemies != null)
					{
						characters = new List<Character>(enemies);
					}
				}
				break;
			case Character.EScope.Hero:
				{
					List<Player> players = Game.Singleton.Players;

					characters = new List<Character>();

					foreach (Player p in players)
					{
						characters.Add(p.Hero.GetComponent<Hero>());
					}

					return characters;
				}
			default:
				{
					Debug.LogError("Invalid case");
				}
				break;
		}

		return characters;
	}

	public bool CheckCircle(Circle circle, GameObject go)
	{
		// Check the radius of the circle shape against the extents of the enemy.
		Vector3 extents = new Vector3(go.collider.bounds.extents.x, 0.5f, go.collider.bounds.extents.z);
		Vector3 pos = new Vector3(go.transform.position.x, 0.5f, go.transform.position.z);

		return (MathUtility.IsCircleCircle(circle.Position, circle.radius, pos, (extents.x + extents.z * 0.5f)));
	}

    public bool CheckCircle(Circle circle, Collider collider)
    {
        Vector3 extents = new Vector3(collider.bounds.extents.x, 0.5f, collider.bounds.extents.z);
        Vector3 pos = new Vector3(collider.transform.position.x, 0.5f, collider.transform.position.z);

        return (MathUtility.IsCircleCircle(circle.Position, circle.radius, pos, (extents.x + extents.z * 0.5f)));
    }


	public bool CheckArc(Arc arc, GameObject c)
	{
        Transform xform = c.transform.FindChild("Colliders");
        if (xform != null)
        {
            Collider[] colliders = xform.gameObject.GetComponentsInChildren<Collider>();

            foreach (Collider col in colliders)
            {
                if ((CheckArc(arc, col)))
                {
                    return true;
                }
            }
            return false;
        }

        return CheckArc(arc, c.collider);
	}

    public bool CheckArc(Arc arc, Collider col)
    {
        Vector3 extents = new Vector3(col.bounds.extents.x, 0.1f, col.bounds.extents.z);
        Vector3 pos = col.transform.position;

        bool inside = false;

        // TL
        Vector3 point = new Vector3(pos.x - extents.x, pos.y, pos.z + extents.z);
        inside = MathUtility.IsWithinCircleArc(point, arc.Position, arc.Line1, arc.Line2, arc.radius);

        // T
        if (!inside)
        {
            point = new Vector3(pos.x + extents.x, pos.y, pos.z + extents.z);
            inside = MathUtility.IsWithinCircleArc(point, arc.Position, arc.Line1, arc.Line2, arc.radius);
        }

        // TR
        if (!inside)
        {
            point = new Vector3(pos.x + extents.x, pos.y, pos.z + extents.z);
            inside = MathUtility.IsWithinCircleArc(point, arc.Position, arc.Line1, arc.Line2, arc.radius);
        }

        // BL
        if (!inside)
        {
            point = new Vector3(pos.x - extents.x, pos.y, pos.z - extents.z);
            inside = MathUtility.IsWithinCircleArc(point, arc.Position, arc.Line1, arc.Line2, arc.radius);
        }

        // B
        if (!inside)
        {
            point = new Vector3(pos.x + extents.x, pos.y, pos.z + extents.z);
            inside = MathUtility.IsWithinCircleArc(point, arc.Position, arc.Line1, arc.Line2, arc.radius);
        }

        // BR
        if (!inside)
        {
            point = new Vector3(pos.x + extents.x, pos.y, pos.z - extents.z);
            inside = MathUtility.IsWithinCircleArc(point, arc.Position, arc.Line1, arc.Line2, arc.radius);
        }

        // L
        if (!inside)
        {
            point = new Vector3(pos.x + extents.x, pos.y, pos.z + extents.z);
            inside = MathUtility.IsWithinCircleArc(point, arc.Position, arc.Line1, arc.Line2, arc.radius);
        }

        // R
        if (!inside)
        {
            point = new Vector3(pos.x + extents.x, pos.y, pos.z + extents.z);
            inside = MathUtility.IsWithinCircleArc(point, arc.Position, arc.Line1, arc.Line2, arc.radius);
        }

        return inside;
    }

	public GameObject FindHeroTarget(Hero hero, Shape2D shape)
	{
		return FindTargetsInFront(hero, shape);
	}

	private GameObject FindTargetsInFront(Hero hero, Shape2D shape)
	{
		GameObject target = null;
		List<Character> enemyTargets = new List<Character>();

		Character closestCharacter = null;

		float enemyDistance = 10000000.0f;

		if (CheckCollisionArea(shape, Character.EScope.Enemy, ref enemyTargets))
		{
			float closestDistance = 1000000.0f;
			foreach (Character e in enemyTargets)
			{
				float distance = (hero.transform.position - e.transform.position).sqrMagnitude;

				if (distance < closestDistance)
				{
					enemyDistance = distance;
					closestDistance = distance;
					closestCharacter = e;
				}
			}
		}

		if (closestCharacter != null)
		{
			target = closestCharacter.gameObject;	
		}

		// Check interactables.
		Interactable closestInteractable = null;

		List<Interactable> interactables = new List<Interactable>();

		float interactDistance = 10000000.0f;

		if (CheckCollisionArea(shape, ref interactables))
		{
			float closestDistance = 1000000.0f;
			foreach (Interactable i in interactables)
			{
				float distance = (hero.transform.position - i.transform.position).sqrMagnitude;

				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestInteractable = i;
				}
			}
		}

		if (target == null && closestInteractable != null)
		{
			return closestInteractable.gameObject;
		}
		else if (target != null && closestInteractable == null)
		{
			return target;
		}
		else if (target == null && closestInteractable == null)
		{
			return null;
		}

		return interactDistance > enemyDistance ? target : closestInteractable.gameObject;
	}

	[ContextMenu("Go to Room")]
	public void Reposition()
	{
		Floor floor = Game.Singleton.Tower.CurrentFloor;
		Room currentRoom = floor.CurrentRoom;

		if (currentRoom == this)
		{
			return;
		}

		// Find direction to enter from (We want to enter from a direction that makes sense, not just any door).
		float heading = MathUtility.ConvertVectorToHeading((transform.position - currentRoom.transform.position).normalized) * Mathf.Rad2Deg;

		bool transitioned = false;

		// Try north
		if (heading >= -0.45f && heading <= 45.0f)
		{
			Door door = doors.GetDoorFacingDirection(Floor.TransitionDirection.South);
			if (door != null)
			{
				Game.Singleton.Tower.CurrentFloor.TransitionToRoom(door);
				transitioned = true;
			}
		}

		// Try East
		if (!transitioned && (heading >= 0.45f && heading <= 135.0f))
		{
			Door door = doors.GetDoorFacingDirection(Floor.TransitionDirection.West);
			if (door != null)
			{
				Game.Singleton.Tower.CurrentFloor.TransitionToRoom(door);
				transitioned = true;
			}
		}

		// Try South
		if (!transitioned && ((heading >= 135.0f && heading <= 180.0f) || (heading <= -135.0f)))
		{
			Door door = doors.GetDoorFacingDirection(Floor.TransitionDirection.North);
			if (door != null)
			{
				Game.Singleton.Tower.CurrentFloor.TransitionToRoom(door);
				transitioned = true;
			}
		}

		// Try West
		if (!transitioned && (heading <= -0.45f && heading >= -135.0f))
		{
			Door door = doors.GetDoorFacingDirection(Floor.TransitionDirection.East);
			if (door != null)
			{
				Game.Singleton.Tower.CurrentFloor.TransitionToRoom(door);
				transitioned = true;
			}
		}

		if(!transitioned)
		{
			// Try any door :<

			for (Floor.TransitionDirection direction = Floor.TransitionDirection.North; direction < Floor.TransitionDirection.MAX; ++direction )
			{
				Door correctDoor = doors.GetDoorFacingDirection(Floor.TransitionDirection.South);
				if (correctDoor != null)
				{
					 Game.Singleton.Tower.CurrentFloor.TransitionToRoom(correctDoor);
					transitioned = true;
					break;
				}
			}

			if (!transitioned)
			{
				Debug.Log("No Direction or no door to enter, " + heading);
			}
		}
	}
}
