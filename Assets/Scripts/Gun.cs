﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	public Transform muzzle; 
	public Projectile projectile;
	public float msBetweenShots = 100f;
	public float muzzleVelosity = 0.03f;

	float nextShotTime;

	public void Shoot(){
		if(Time.time > nextShotTime ){
			nextShotTime = Time.time + msBetweenShots/1000f;
	 		Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
	 		newProjectile.SetSpeed(muzzleVelosity);
	 	}
	}
}
