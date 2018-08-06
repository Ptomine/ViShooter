using UnityEngine;

public class Bullet : MonoBehaviour {

	public float Damage;
	
	void Start () {
		Destroy(gameObject, 1.5f);	
	}
	
	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Enemy")) {
			other.gameObject.GetComponent<EnemyController>().TakeDamage(Damage);
		}

		if (other.gameObject.CompareTag("Player")) {
			other.gameObject.GetComponent<PlayerHealth>().TakeDamage(Damage);
		}
		if (!other.gameObject.CompareTag("Bullet"))
			Destroy(gameObject);
	}
}
