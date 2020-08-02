using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour {

	public float hitDelay = 0.5f;
	// variable to determine if the damage function can be called
	private bool _mCanDamage = true;


	void OnTriggerEnter2D(Collider2D other){
		
		IDamageable hit = other.GetComponent<IDamageable>();

		if (hit != null) 
		{
			// if can attack
			if (_mCanDamage) 
			{
				hit.Damage();
				_mCanDamage = false;
				
				StartCoroutine(DamagePause());
			}
		}
	}

	// Coroutine to switch variable back to true after 0.5 seconds
	IEnumerator DamagePause()
	{
		yield return new WaitForSeconds(hitDelay);
		_mCanDamage = true;
	}
}
