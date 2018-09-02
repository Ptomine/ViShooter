using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameRestarter : MonoBehaviour {

	public Text GotYou;
	public Text Accuracy;
	public Text MesasgeText;
	
	private PlayerController _playerController;
	
	void Start () {
		_playerController = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<PlayerController>();
	}
	
	void Update () {
		if (_playerController.isDead) {
			GotYou.enabled = true;
			var accuracy = new StringBuilder("accuracy: ");
			accuracy.Append(_playerController.Accuracy);
			accuracy.Append("%");
			Accuracy.enabled = true;
			Accuracy.text = accuracy.ToString().Normalize();
			MesasgeText.enabled = true;
			if (Input.GetKey(KeyCode.Escape)) {
				ChangeScene();
			}
		}	
	}

	private void ChangeScene() {
		StartCoroutine(LoadScene());
	}

	private IEnumerator LoadScene() {
		var loadingSceneOperation = SceneManager.LoadSceneAsync("Menu");
		while (!loadingSceneOperation.isDone) {
			yield return null;
		}
	}
}
