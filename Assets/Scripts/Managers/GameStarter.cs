using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour {

    void Update() {
        if (Input.anyKey) {
            ChangeScene();
        }
    }
    
    public void ChangeScene() {
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene() {
        var loadingSceneOperation = SceneManager.LoadSceneAsync("Game");
        while (!loadingSceneOperation.isDone) {
            yield return null;
        }
    }
}
