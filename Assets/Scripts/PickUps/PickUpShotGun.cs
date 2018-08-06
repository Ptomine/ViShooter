using UnityEngine;

public class PickUpShotGun : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            other.gameObject.GetComponent<PlayerController>().PickShotGun();
            Destroy(gameObject);
        }
    }
}
