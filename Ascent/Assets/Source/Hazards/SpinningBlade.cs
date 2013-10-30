﻿using UnityEngine;
using System.Collections;

public class SpinningBlade : MonoBehaviour 
{
    public enum EBladeDirection
    {
        left = -1,
        right = 1
    }

	struct TBlade
	{
		public GameObject go;
		public Blade script;
	}

    public GameObject blade;
    public int bladeCount;
    public EBladeDirection bladeDirection;
    public float rotationSpeed;
    public float bladeDamage;
	public float bladeLength;

	private TBlade[] blades;
	private int previousBladeCount;
	private float previousBladeLength;

	void Start () 
    {
		Initialise();
	}

	void Initialise()
	{
		if (bladeCount < 1)
		{
			bladeCount = 1;
		}
		previousBladeCount = bladeCount;
		previousBladeLength = bladeLength;

		blades = new TBlade[bladeCount];

		transform.FindChild("Blades");

		for (int i = 0; i < bladeCount; ++i)
		{
			GameObject newBlade = GameObject.Instantiate(blade) as GameObject;
			blades[i].go = newBlade;
			blades[i].script = newBlade.GetComponent<Blade>();
			blades[i].script.Initialise(bladeDamage);

			newBlade.transform.parent = this.transform;

			newBlade.transform.localScale = new Vector3(newBlade.transform.localScale.x + bladeLength, newBlade.transform.localScale.y, newBlade.transform.localScale.z);

			Vector3 offset = new Vector3(0.0f, 1.0f, 3.5f + bladeLength * 0.5f);
			newBlade.transform.position += offset + new Vector3(transform.position.x, 0.0f, transform.position.z);

			float angle = (360.0f / bladeCount) * (float)bladeDirection;
			transform.Rotate(Vector3.up, angle);
		}
	}

	void Shutdown()
	{
		for (int i = 0; i < previousBladeCount; ++i)
		{
			Object.Destroy(blades[i].go);
		}
	}
	
	void Update () 
    {
       gameObject.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f) * rotationSpeed * (float)bladeDirection);

		// TODO: Remove this for optimisation
	   if (previousBladeCount != bladeCount || previousBladeLength != bladeLength)
		{
			Shutdown();
			Initialise();
		}
	}
}
