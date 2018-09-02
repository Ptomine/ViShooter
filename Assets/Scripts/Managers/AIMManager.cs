using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIMManager : MonoBehaviour {

	private Dictionary<int, List<string>> _names;
	public Dictionary<string, GameObject> Targets;

	private const int MIN_LENGTH = 3;
	private const int MAX_LENGTH = 11;
	
	void Start () {
		_names = buildDictionary();
		Targets = new Dictionary<string, GameObject>();
	}
	
	private Dictionary<int, List<string>> buildDictionary() {
		TextAsset textAsset = (TextAsset) Resources.Load("English");
		string[] dictionary = textAsset.text.Split("\n"[0]);
		
		Dictionary<int, List<string>> words = new Dictionary<int, List<string>>();
		for (int len = MIN_LENGTH; len < MAX_LENGTH; len++) {
			words[len] = new List<string>();
		}

		foreach (string word in dictionary) {
			int length = word.Length;
			if (length < MAX_LENGTH && length >= MIN_LENGTH) {
				List<string> list = words[length];
				list.Add(word);
			}
		}

		return words;
	}

	public string NewTarget(GameObject target, int difficulty) {
		bool isUnique = false;
		var words = _names[Random.Range(MIN_LENGTH, MIN_LENGTH + difficulty >= MAX_LENGTH ? MAX_LENGTH : MIN_LENGTH + difficulty)];
		string targetName = "";
		while (!isUnique) {
			targetName = words[Random.Range(0, words.Count)];
			if (!Targets.ContainsKey(targetName)) {
				isUnique = true;
			}	
		}
		Targets.Add(targetName, target);
		return targetName;
	}

	public Vector3 FindTarget(string target) {
		if (Targets.ContainsKey(target)) {
			return Targets[target].transform.position;
		}
		return Vector3.zero;
	}
}
