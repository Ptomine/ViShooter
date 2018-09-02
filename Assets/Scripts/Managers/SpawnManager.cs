using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour {
	public GameObject EnemyPrefab;
	
	private const float RANGE = 10.5f;
	
	private Vector3 _player_position;
	private AIMManager _aim;

	private const float MIN_SPEED = 0.33f;
	private const float MAX_SPEED = 0.7f;

	private const int RIFFLE_CHANCE = 5;
	private const int RIFFLE_BOTTOM_LINE = 4;
	
	private const float TEMPO = 30.0f;
	private const float MIN_INTERVAL = 5.0f;
	private const float MAX_INTERVAL = 10.0f;
	private float _time_to_spawn;
	private float _timer;
	private int _difficulty;
	
	void Start () {
		_aim = GameObject.FindGameObjectWithTag("Aim").GetComponent<AIMManager>();
		
		_time_to_spawn = Random.Range(1.0f, 5.0f);
	}
	
	void Update () {
		_time_to_spawn -= Time.deltaTime;
		_timer += Time.deltaTime;
		if (_timer >= TEMPO) {
			_difficulty += 1;
			_timer = 0f;
		}
		
		_player_position = GameObject.FindWithTag("Player").transform.position;

		if (_time_to_spawn <= 0) {
			if (SpawnEnemy()) {
				_time_to_spawn = Random.Range(MIN_INTERVAL - _difficulty / 2.0f, MAX_INTERVAL - _difficulty);
			}
		}
	}

	private bool SpawnEnemy() {
		Vector3 spawnPosition = RandomPosition();
		if (spawnPosition != Vector3.zero) {
			GameObject enemy = Instantiate(EnemyPrefab, spawnPosition, new Quaternion(0, 0, 0, 0));
			enemy.GetComponent<NavMeshAgent>().speed = Random.Range(MIN_SPEED + _difficulty / 10.0f, MAX_SPEED + _difficulty / 10.0f);
			enemy.GetComponent<EnemyController>().HealthPoints += Random.Range(0.0f, _difficulty);
			enemy.GetComponent<EnemyController>().Aim = _aim.NewTarget(enemy, _difficulty);
			enemy.GetComponent<EnemyController>().Text.text = enemy.GetComponent<EnemyController>().Aim;
			if (Random.Range(0, RIFFLE_CHANCE + _difficulty) > RIFFLE_BOTTOM_LINE) {
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
