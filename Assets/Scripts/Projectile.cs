using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	float _speed = 10f;
	float damage = 1f;
	float lifetime = 3;
	float skinWIdth = .1f;

	public LayerMask collisionMask;

	// Use this for initialization
	void Start () {
		Destroy(gameObject, lifetime);

		Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
		if (initialCollisions.Length > 0){
			OnHitObject(initialCollisions[0], transform.position);
		}
	}

	public void SetSpeed (float speed){
		_speed = speed;
	}

	// Update is called once per frame
	void Update () {
		float moveDistance = _speed * Time.deltaTime;
		CheckCollisions (moveDistance);
		transform.Translate(Vector3.forward * moveDistance);
	}

	void CheckCollisions (float moveDistance){
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, moveDistance + skinWIdth, collisionMask, QueryTriggerInteraction.Collide)){
			 OnHitObject(hit.collider, hit.point);
		}
	}

	void OnHitObject (Collider c, Vector3 hitPoint){
		IDamageble damagebleObject = c.GetComponent<Collider>().GetComponent<IDamageble>();
		if (damagebleObject != null){
			damagebleObject.TakeHit(damage, hitPoint, transform.forward); 
		}
		Destroy(gameObject);
	}
}
