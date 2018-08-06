using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour {
	public GameObject EnemyPrefab;
	
	private const float RANGE = 10.5f;
	private float _time_to_spawn;
	
	private Vector3 _player_position;
	private AIMManager _aim;

	private const float TEMP = 30.0f;
	private float _timer = 0;
	private int _difficulty = 0;
	
	void Start () {
		_aim = GameObject.FindGameObjectWithTag("Aim").GetComponent<AIMManager>();
		
		_time_to_spawn = Random.Range(1.0f, 5.0f);
	}
	
	void Update () {
		_time_to_spawn -= Time.deltaTime;
		_timer += Time.deltaTime;
		if (_timer >= TEMP) {
			_difficulty += 1;
			_timer = 0f;
		}
		
		_player_position = GameObject.FindWithTag("Player").transform.position;

		if (_time_to_spawn <= 0) {
			if (SpawnEnemy()) {
				_time_to_spawn = Random.Range(5.0f - _difficulty / 2f, 10.0f - _difficulty);
			}
		}
	}

	private bool SpawnEnemy() {
		Vector3 spawnPosition = RandomPosition();
		if (spawnPosition != Vector3.zero) {
			GameObject enemy = Instantiate(EnemyPrefab, spawnPosition, new Quaternion(0, 0, 0, 0));
			enemy.GetComponent<NavMeshAgent>().speed = Random.Range(0.33f + _difficulty / 10f, 0.7f + _difficulty / 5f);
			enemy.GetComponent<EnemyController>().HealthPoints += Random.Range(0f,_difficulty);
			enemy.GetComponent<EnemyController>().Aim = _aim.NewTarget(enemy, _difficulty);
			enemy.GetComponent<EnemyController>().Text.text = enemy.GetComponent<EnemyController>().Aim;
			if (Random.Range(0, 5 + _difficulty) > 4) {
				enemy.GetComponent<EnemyController>().PickRifle();
			}
			return true;
		}

		return false;
	}

	private Vector3 RandomPosition() {
		Vector2 direction = Random.insideUnitCircle;
		if (direction.x >= 0) {
			direction.x += RANGE;
		} else {
			direction.x -= RANGE;
		}

		if (direction.y >= 0) {
			direction.y += RANGE;
		} else {
			direction.y -= RANGE;
		}

		Vector3 RandomPoint = _player_position + new Vector3(direction.x, 0, direction.y);
		NavMeshHit hit;
		if (NavMesh.SamplePosition(RandomPoint, out hit, 1.0f, NavMesh.AllAreas)) {
			return hit.position;
		}
		
		return Vector3.zero;
	}
}
