using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PlayerController : MonoBehaviour {

	Vector3 _velocity;
	Rigidbody _rigidbody;

	void Start () {
		_rigidbody = GetComponent<Rigidbody>();
	}

	public void Move(Vector3 velocity){
		_velocity = velocity;
	}

	public void LookAt (Vector3 lookPoint){
		Vector3 highCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
		transform.LookAt(highCorrectedPoint);
	}

	void FixedUpdate (){
		_rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
	}
}
