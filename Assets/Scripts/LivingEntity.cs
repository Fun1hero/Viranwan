using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageble {

	public float startingHealth;
	protected float health;
	protected bool dead;

	public event System.Action onDeath;

	protected virtual void Start (){
		health = startingHealth;
	}

	public virtual void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDiretion){
		TakeDamage(damage);
	}

	public virtual void TakeDamage (float damage){
		health -= damage;
		if (health <= 0 && !dead){
			
			Die();
		}
	}

	[ContextMenu ("Self Destruct")]
	protected void Die(){
		dead = true;
		if (onDeath != null) onDeath(); 
		Destroy(gameObject);
	}
}
