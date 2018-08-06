using UnityEngine;

public class HitArea : MonoBehaviour {

	void Start() {
		Destroy(gameObject, 0.1f);
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Player")) {
			other.gameObject.GetComponent<PlayerHealth>().TakeDamage(15.0f);
		}
	}
}
