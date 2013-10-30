﻿using UnityEngine;
using System.Collections;

public class ArrowShooter : MonoBehaviour 
{
    public GameObject projectile;
    public int projectilePoolCount = 5;
    public float frequency = 1.0f;
    public float projectileSpeed = 5.0f;
    public float projectileDamage = 2.0f;

    private ObjectPool arrowPool;
    private Vector3 direction;
    private Vector3 spawnPoint;
    private float timeElapsed = 0.0f;
    private float arrowLifeSpawn = 2.0f;

	// Use this for initialization
	void Start () 
    {
        arrowPool = new ObjectPool(projectile, projectilePoolCount, this.transform, "Arrow");
        direction = (transform.FindChild("Shooter").transform.position - transform.FindChild("Base").transform.position).normalized;
        spawnPoint = transform.FindChild("Shooter").transform.position + (direction * 1.0f);
	}
	
	// Update is called once per frame
	void Update () 
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > frequency)
        {
            timeElapsed -= frequency;

            // Instantiate an arrow
            ObjectPool.PoolObject po = arrowPool.GetInactive();
            if (po != null)
            {
                Arrow newArrow = po.script as Arrow;
                newArrow.Initialise(arrowLifeSpawn, direction, projectileSpeed, projectileDamage);
                po.go.SetActive(true);
                po.go.transform.position = spawnPoint;
                po.go.transform.LookAt(new Vector3(1.0f, 0.0f, 0.0f));
                //po.go.transform.tra
                //po.go.rigidbody.velocity = Vector3.zero;
                //po.go.rigidbody.angularVelocity = Vector3.zero;
                //po.go.rigidbody.AddForce(direction * 50.0f);
            }
            else
            {
                Debug.Log(name + " ran out of " + projectile.ToString() + " to fire.");
            }
        }
	}
}
