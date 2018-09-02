using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
	public float HealthPool = 100.0f;

	public Slider Slider;

	public Image Image;

	public Color FullHealthColor = Color.cyan;
	public Color ZeroHealthColor = Color.gray;

	private float CurrentHealth;

	private Animator _animator;
	
	private void OnEnable() {
		CurrentHealth = HealthPool;

		_animator = gameObject.GetComponent<Animator>();
		
		SetHealthUI();
	}

	public void TakeDamage(float damage) {
		CurrentHealth -= damage;
		
		SetHealthUI();
		
		if (CurrentHealth <= 0) {
			Death();
			return;
		}
		_animator.SetTrigger("Damage");	
		_animator.SetInteger("DamageID", Random.Range(0, 2));
	}

	private void SetHealthUI() {
		Slider.value = CurrentHealth;

		Image.color = Color.Lerp(ZeroHealthColor, FullHealthColor, CurrentHealth / HealthPool);
	}

	private void Death() {
		_animator.SetTrigger("Death");
		gameObject.GetComponent<PlayerController>().isDead = true;
	}

}
