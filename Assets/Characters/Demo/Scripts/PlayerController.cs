using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent (typeof (Animator))]
public class PlayerController : MonoBehaviour {

	public InputField InputLine;
	public AIMManager AimSystem;
	
	public float Speed;
	private const float ROTATION_SPEED = 10f;
	private const float AIM_ROTATION_SPEED = 300f;
	private float HealthPoints;
	private const float SHOT_POWER = 5;
	
	public Transform rightGunBone;
	public Transform leftGunBone;
	public Arsenal[] arsenal;

	private AudioSource _audioSource;
	
	private Rigidbody _rigidbody;
	private Animator _animator;
	public GameObject Bullet;
	public GameObject RightBarrel;
	public GameObject LeftBarrel;

	private bool isNormalMode = true;
	public bool isDead;

	private const float DOUBLECLICK_TIMING = 1.5f;
	private bool firstButtonPressed;
	private float timeOfFirstButton;
	private bool reset;
	
	private int _misses;
	private int _hit;
	public int Accuracy;
	
	private bool shotGun;

	void Awake() {
		_rigidbody = GetComponent<Rigidbody>();
		_animator = GetComponent<Animator> ();
		_audioSource = GetComponent<AudioSource>();
		
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		
		if (arsenal.Length > 0)
			SetArsenal (arsenal[(int) Weapon.TwoPistols].name);
	}
	
	void FixedUpdate() {
		if (isNormalMode && !isDead) {
			Move();
		}
	}
	
	void Update() {
		if (isDead) {
			if (_hit != 0 || _misses != 0) {
				Accuracy = (int) (_hit * 100f / (_hit + (float) _misses));
			}
			return;
		}
		
		if (Input.GetKey(KeyCode.I)) {
			ChangeModeToInput();	
		}

		if (Input.GetKey(KeyCode.Escape) || DoubleJ()) {
			ChangeModeToNormal();	
		}
		
		if (isNormalMode) {
		}

		if (!isNormalMode) {
			if (InputLine.text != "" && Input.GetKey(KeyCode.Return)) {
				Debug.Log(InputLine.text);
				if (InputLine.text.Normalize() == ":q") {
					Application.Quit();
				}
				AimAndShoot(InputLine.text.ToLower().Normalize());
				InputLine.text = "";
				ChangeModeToNormal();
				ChangeModeToInput();
			}
		}
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
					RightBarrel = hand.rightBarrel;
					Bullet = hand.bullet;
				}
				if (hand.leftGun != null) {
					GameObject newLeftGun = Instantiate(hand.leftGun);
					newLeftGun.transform.parent = leftGunBone;
					newLeftGun.transform.localPosition = Vector3.zero;
					newLeftGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
					LeftBarrel = hand.leftBarrel;
				}
				_animator.runtimeAnimatorController = hand.controller;
				_audioSource.clip = hand.sound;
				return;
				}
		}
	}
	
	private void Move() {
		var direction = GetMovementDirection();
		var shift = Speed * direction * Time.deltaTime;
		if (shift != Vector3.zero) {
			_animator.SetFloat("Speed", Speed);
		} else {
			_animator.SetFloat("Speed", 0);
		}
		_rigidbody.MovePosition(_rigidbody.position + shift);
		if (direction != Vector3.zero) {
			transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, direction,
				ROTATION_SPEED * Time.deltaTime, 0.0f));
		}
	}

	private Vector3 GetMovementDirection() {
		Vector3 direction = Vector3.zero;
		if (Input.GetKey(KeyCode.J)) {
			direction.z--;
		}

		if (Input.GetKey(KeyCode.H)) {
			direction.x--;
			if (direction.z != 0) {
				return direction.normalized;
			}
		}

		if (Input.GetKey(KeyCode.K)) {
			direction.z++;
			if (direction.x != 0) {
				return direction.normalized;
			}
		}

		if (Input.GetKey(KeyCode.L)) {
			direction.x++;
			if (direction.z != 0) {
				return direction.normalized;
			}
		}

		return direction;
	}

	private void ChangeModeToInput() {
		isNormalMode = false;
		_animator.SetBool("Aiming", true);
		
		EventSystem.current.SetSelectedGameObject(InputLine.gameObject, null);
	}

	private void ChangeModeToNormal() {
		InputLine.text = "";
		isNormalMode = true;
		_animator.SetBool("Aiming", false);
		
		EventSystem.current.SetSelectedGameObject(gameObject, null);
	}

	private void AimAndShoot(string target) {
		Vector3 targetPosition = AimSystem.FindTarget(target);
		if (targetPosition == Vector3.zero) {
			_misses++;
			return;
		}

		Vector3 direction = targetPosition - transform.position;
		direction = new Vector3(direction.x, 0, direction.z);
		if (direction != Vector3.zero) {
			transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, direction,
				AIM_ROTATION_SPEED * Time.deltaTime, 0.0f));
		}

		_hit++;
		Shoot(targetPosition);
	}

	private void Shoot(Vector3 targetPosition) {
		_animator.SetTrigger("Attack");
		_audioSource.Play();
		if (shotGun) {
			float deviation = -5f;
			GameObject[] shells = new GameObject[9];
			for (int i = 0; i < 9; i++) {
				shells[i] = Instantiate(Bullet, RightBarrel.transform.position, RightBarrel.transform.rotation);
				shells[i].transform.RotateAround(shells[i].transform.position, Vector3.up, deviation);
				shells[i].GetComponent<Rigidbody>().AddForce(shells[i].transform.forward * SHOT_POWER);
				deviation += 1.25f;
			}
		} else {
			var shell = Instantiate(Bullet, RightBarrel.transform.position, transform.rotation);
			shell.GetComponent<Rigidbody>().AddForce((targetPosition - shell.transform.position) * SHOT_POWER);
		}
	}
	
	private bool DoubleJ() {
		if (Input.GetKeyDown(KeyCode.J) && firstButtonPressed) {
			if (Time.time - timeOfFirstButton < DOUBLECLICK_TIMING) {
				return true;
			} 
			
			reset = true;
		}

		if (Input.GetKeyDown(KeyCode.J) && !firstButtonPressed) {
			firstButtonPressed = true;
			timeOfFirstButton = Time.time;
		}

		if (reset) {
			firstButtonPressed = false;
			reset = false;
		}

		return false;
	}

	public void PickShotGun() {
		SetArsenal(arsenal[(int) Weapon.ShotGun].name);
		shotGun = true;
	}

	[System.Serializable]
	public struct Arsenal {
		public string name;
		public GameObject rightGun;
		public GameObject rightBarrel;
		public GameObject leftGun;
		public GameObject leftBarrel;
		public GameObject bullet;
		public AudioClip sound;
		public RuntimeAnimatorController controller;
	}

	private enum Weapon {
		Empty,
		Pistol,
		TwoPistols,
		Rifle,
		ShotGun
	};
}
