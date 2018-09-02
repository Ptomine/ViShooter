using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent (typeof (Animator))]
public class EnemyController : MonoBehaviour {

	public Transform rightGunBone;
	public Transform leftGunBone;
	public Arsenal[] arsenal;
	public Text Text;
	public Transform CanvasTransform;
	public string Aim;

	private Animator animator;
	private NavMeshAgent _agent;
	private GameObject _target;
	public bool WithRifle;
	public float HealthPoints = 1.0f;
	
	private float reloading;
	private const float RELOADING_TIME = 1.5f;
	private const float MIN_SPEED = 0.25f;
	private const float SHOOT_DISTANCE = 4.0f;
	private const float HIT_DISTANCE = 0.502f;

	public GameObject Bullet;
	public GameObject Barrel;

	public GameObject Drop;

	
	void Awake() {
		animator = GetComponent<Animator>();
		_agent = GetComponent<NavMeshAgent>();
		_target = GameObject.FindGameObjectWithTag("Player");
		if (arsenal.Length > 0)
			SetArsenal (arsenal[(int) Weapon.Empty].name);

	}

	void Update() {
		animator.SetFloat("Speed", _agent.velocity.magnitude);
		if (reloading >= 0) {
			reloading -= Time.deltaTime;
		}
		if (_agent.isActiveAndEnabled) {
			_agent.SetDestination(_target.transform.position);
			if (_agent.velocity.magnitude < MIN_SPEED) {
				animator.SetBool("Aiming", true);
				if (WithRifle && Vector3.Distance(transform.position, _target.transform.position) <= SHOOT_DISTANCE &&
				    reloading <= 0) {
					Shoot(_target.transform.position);
				}

				if (!WithRifle && Vector3.Distance(transform.position, _target.transform.position) <= HIT_DISTANCE && reloading <= 0) {
					Hit();
				}
			}
			animator.SetBool("Aiming", false);
		}

		RotateCanvas();
	}

	private void SetArsenal(string name) {
		foreach (Arsenal hand in arsenal) {
			if (hand.name == name) {
				if (rightGunBone.childCount > 0)
					Destroy(rightGunBone.GetChild(0).gameObject);
				if (leftGunBone.childCount > 0)
					Destroy(leftGunBone.GetChild(0).gameObject);
				if (hand.rightGun != null) {
					GameObject newRightGun = Instantiate(hand.rightGun);
					newRightGun.transform.parent = rightGunBone;
					newRightGun.transform.localPosition = Vector3.zero;
					newRightGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
				}
				if (hand.leftGun != null) {
					GameObject newLeftGun = Instantiate(hand.leftGun);
					newLeftGun.transform.parent = leftGunBone;
					newLeftGun.transform.localPosition = Vector3.zero;
					newLeftGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
				}
				Bullet = hand.bullet;
				Barrel = hand.barrel;
				animator.runtimeAnimatorController = hand.controller;
				return;
				}
		}
	}

	public void PickRifle() {
		SetArsenal(arsenal[(int) Weapon.Rifle].name);
		WithRifle = true;
		_agent.stoppingDistance = SHOOT_DISTANCE;
	}

	private void RotateCanvas() {
		CanvasTransform.rotation =
			Quaternion.Euler(-transform.rotation.x, -transform.rotation.y, -transform.rotation.z);
	}

	public void TakeDamage(float damage) {
		HealthPoints -= damage;
		if (HealthPoints <= 0) {
			Death();
			return;
		}
		animator.SetTrigger("Damage");
		animator.SetInteger("DamageID", Random.Range(0, 1));
	}

	private void Death() {
		if (WithRifle) {
			Destroy(Instantiate(Drop, _agent.transform.position, _agent.transform.rotation).gameObject, 20.0f);
		}
		_agent.enabled = false;
		animator.SetTrigger("Death");
		Destroy(gameObject, 5.0f);
		Text.color = Color.grey;
		GetComponent<Collider>().enabled = false;

	}

	private void Shoot(Vector3 targetPosition) {
		animator.SetBool("Aiming", false);
		animator.SetTrigger("Attack");
		var shell = Instantiate(Bullet, Barrel.transform.position, transform.rotation);
		shell.GetComponent<Rigidbody>().AddForce((targetPosition - shell.transform.position) * 5);
		reloading = RELOADING_TIME;
	}

	private void Hit() {
		animator.SetBool("Aiming", false);
		animator.SetTrigger("Attack");
		Instantiate(Bullet, Barrel.transform.position, Barrel.transform.rotation);
		reloading = RELOADING_TIME;
	}

	[System.Serializable]
	public struct Arsenal {
		public string name;
		public GameObject rightGun;
		public GameObject barrel;
		public GameObject bullet;
		public GameObject leftGun;
		public RuntimeAnimatorController controller;
	}

	private enum Weapon {
		Empty,
		Rifle
	};
}
