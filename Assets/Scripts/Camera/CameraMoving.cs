using UnityEngine;

public class CameraMoving : MonoBehaviour {

	private GameObject _player;
	private bool isPlayerFounded;

	private const float ACCELERATION = 0.05f;
	private float speed;
	private const float MAX_SPEED = 0.45f;
	
	private Vector3 CameraOffset = new Vector3(0.0f, 7.8f, 5.0f);

	void Update () {
		if (!isPlayerFounded) {
			_player = GameObject.FindGameObjectWithTag("Player");
			isPlayerFounded = true;
		} else {
			Move();
		}
		
	}

	private void Move() {
		Vector3 playerPosition = _player.transform.position;
		float distance = Vector2.Distance(new Vector2(playerPosition.x, playerPosition.z),
			new Vector2(transform.position.x + CameraOffset.x, transform.position.z + CameraOffset.z));
		Vector3 direction = new Vector3(playerPosition.x - transform.position.x - CameraOffset.x, 0,
			playerPosition.z - transform.position.z - CameraOffset.z).normalized;

		if (distance > 0.2f) {
			if (speed < MAX_SPEED) {
				speed += ACCELERATION;
			}

			transform.position += direction * speed * Time.deltaTime;
		} else {
			if (speed > MAX_SPEED) {
				speed -= ACCELERATION;
			} else {
				speed = 0f;
			}
		}
	}
}
