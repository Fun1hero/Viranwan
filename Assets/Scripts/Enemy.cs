using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity {

	public enum State{ Idle, Chasing, Attacking}
	State currentState;

	public ParticleSystem deathEffect;

	NavMeshAgent pathfinder;
	Transform target;
	LivingEntity targetEntity;
	Material skinMaterial;

	Color originalColor;

	float attackDistanceThreshhold = 0.5f;
	float timeBetweenAttacks = 1;
	float damage = 1;

	float nextAttackTime;
	float myCollisionRadius;
	float targetCollisionRadius;

	bool hasTarget;

	// Use this for initialization
	protected override void Start (){
		base.Start ();
		pathfinder = GetComponent<NavMeshAgent> ();
		skinMaterial = GetComponent<Renderer> ().material;
		originalColor = skinMaterial.color;


		if (GameObject.FindGameObjectWithTag ("Player") != null) {
			currentState = State.Chasing;
			target = GameObject.FindGameObjectWithTag ("Player").transform;

			hasTarget = true;

			targetEntity = target.GetComponent<LivingEntity> ();
			targetEntity.onDeath += OnTargetDeath;

			myCollisionRadius = GetComponent<CapsuleCollider> ().radius;
			targetCollisionRadius = target.GetComponent<CapsuleCollider> ().radius;

			StartCoroutine (UpdatePath ());
		}
	}

	public override void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDiretion)
	{
		if (damage >= health){
			Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDiretion)) as GameObject, deathEffect.main.startLifetime.constantMax);
		}
		base.TakeHit (damage, hitPoint, hitDiretion);
	}

	void OnTargetDeath (){
		hasTarget = false;
		currentState = State.Idle;
	}
	
	// Update is called once per frame
	void Update (){
		if (hasTarget) {
			if (Time.time > nextAttackTime) {
				float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
				if (sqrDstToTarget < Mathf.Pow (attackDistanceThreshhold + myCollisionRadius + targetCollisionRadius, 2)) {
					nextAttackTime = Time.time + timeBetweenAttacks;
					StartCoroutine (Attack ());
				}
			}
		}
	}

	IEnumerator Attack (){
		currentState = State.Attacking;
		pathfinder.enabled = false;

		Vector3 originalPosition = transform.position;
		Vector3 dirToTarget = (target.position - transform.position).normalized;
		Vector3 attackingPosition = target.position - dirToTarget * (myCollisionRadius);

		float percent = 0;
		float attackSpeed = 3;

		skinMaterial.color = Color.red;
		bool hasAppliedDamage = false;

		while (percent <= 1) {

			if (percent >= .5f && !hasAppliedDamage){
				hasAppliedDamage = true;
				targetEntity.TakeDamage(damage);
			}

			percent += Time.deltaTime * attackSpeed;
			float interpolation =  (-Mathf.Pow (percent, 2) + percent) * 4;
			transform.position = Vector3.Lerp (originalPosition, attackingPosition, interpolation);

			yield return null;
		}

		skinMaterial.color = originalColor;
		currentState = State.Chasing;
		pathfinder.enabled = true;
	}

	IEnumerator UpdatePath (){
		float refreshRate = 0.25f;

		while  (hasTarget){
			if  (currentState == State.Chasing){
				Vector3 dirToTarget = (target.position - transform.position).normalized;
				Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshhold/2);
				if (!dead) pathfinder.SetDestination (targetPosition);
			}
			yield return new WaitForSeconds (refreshRate);
		}
	}
}
