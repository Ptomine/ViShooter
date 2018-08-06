using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

[RequireComponent (typeof (Animator))]
public class StaticPlayer : MonoBehaviour {

	public Transform rightGunBone;
	public Transform leftGunBone;
	public Arsenal[] arsenal;
	
	private Animator _animator;

	void Awake() {
		_animator = GetComponent<Animator>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		if (arsenal.Length > 0)
			SetArsenal(arsenal[0].name);
	}

	public void SetArsenal(string name) {
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

				_animator.runtimeAnimatorController = hand.controller;
				return;
			}
		}
	}

	[System.Serializable]
	public struct Arsenal {
		public string name;
		public GameObject rightGun;
		public GameObject leftGun;
		public RuntimeAnimatorController controller;
	}

	private enum Weapon {
		Empty,
		Pistol,
		TwoPistols,
		Rifle
	};
}
