using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	// Game Settings
	public float PreferredButtonSize {get; set;}
	public float PreferredDuration {get; set;}
	public float PreferredLevel {get; set;}
	public bool PracticeMode {get; set;}
	public bool LevelUpMode {get; set;}

	// UI Element References
	public Transform mainPanel;
	public GameObject scorePanel;
	public GameObject timerPanel;
	public GameObject startButton;
	
	// Object Pooling Variables
	public SimpleObjectPool buttonObjectPool;
	private List<GameObject> buttonGameObjects = new List<GameObject>();

	// Game Variables
	private int score;
	private int Score {
		get {
			return score;
		}
		set {
			score = value;
			scorePanel.GetComponentInChildren<Text>().text = "Score: " + score;
		}
	}

	private bool timerRun;
	private float timeElapsed;
	private float TimeElapsed {
		get {
			return timeElapsed;
		}
		set {
			timeElapsed = value;
			timerPanel.GetComponentInChildren<Text>().text = timeElapsed.ToString("F2");
		}
	}

	private int currentNumber = 1;
	private int level = 0;
	

	void Start () {
		PreferredButtonSize = 64;
		PreferredDuration = 430;
		PreferredLevel = 3;
		PracticeMode = false;
		LevelUpMode = false;
	}

	void Update () {
		if (timerRun) {
			TimeElapsed += Time.deltaTime;
		}
	}

	// Shuffle an array of integer from 1 to n
	private int[] shuffle(int n) {
		int[] sequence = new int[n];
		for (int i = 0; i < n; i++) {
			sequence[i] = i + 1;
		}

		for (int i = 0; i < n; i++) {
			int j = Random.Range(i, n - 1);
			int tmp = sequence[i];
			sequence[i] = sequence[j];
			sequence[j] = tmp;
		}

		return sequence;
	}

	// Place n.Length buttons evenly within minA and maxB
	private void CreateButtons(int[] n, Vector2 minA, Vector2 maxB) {
		int length = n.Length;
		Vector2 difference = maxB - minA;

		if (length == 1) {
			Debug.Log("Button " + n[0].ToString() + " will be within " + minA.ToString() + " and " + maxB.ToString());

			float size;
			if (difference.y >= PreferredButtonSize && difference.x >= PreferredButtonSize) {
				size = PreferredButtonSize;
			} else {
				size = difference.y < difference.x ? difference.y : difference.x;
			}

			float offset = size / 2;

			GameObject buttonGameObject = buttonObjectPool.GetObject();
			buttonGameObjects.Add(buttonGameObject);
			buttonGameObject.transform.SetParent(mainPanel);
			buttonGameObject.GetComponent<Button>().Setup(
				n[0], 
				new Vector2(Random.Range(minA.x + offset, maxB.x - offset), Random.Range(minA.y + offset, maxB.y - offset)), 
				size
			);
		} else {
			Vector2 maxA = maxB, minB = minA;
			if (difference.y > difference.x) {
				maxA -= new Vector2(0, difference.y/2);
				minB += new Vector2(0, difference.y/2);
			} else {
				maxA -= new Vector2(difference.x/2, 0);
				minB += new Vector2(difference.x/2, 0);
			}

			int[] a = new int[length / 2];
			for (int i = 0; i < a.Length; i++) {
				a[i] = n[i];
			}
			int[] b = new int[(length + 1) / 2];
			for (int i = 0; i < b.Length; i++) {
				b[i] = n[i + a.Length]; 
			}

			CreateButtons(a, minA, maxA);
			CreateButtons(b, minB, maxB);
		}
	}

	public void StartLevel() {
		currentNumber = 1;

		Rect rect = mainPanel.GetComponent<RectTransform>().rect;
		CreateButtons(shuffle((LevelUpMode ? ++level : level = (int)PreferredLevel) + 4), rect.min, rect.max);

		if (PracticeMode) {
			TimeElapsed = 0;
			timerRun = true;
		} else {
			startButton.SetActive(false);
			StartCoroutine(HideNumbers(PreferredDuration));
		}
	}

	public void FinishLevel() {
		if (PracticeMode) {
			timerRun = false;
		}

		startButton.SetActive(true);
	}

	public void ButtonClicked(GameObject button) {
		if (currentNumber == button.GetComponent<Button>().N) {
			Score += currentNumber;
			if (currentNumber == level + 4) {
				FinishLevel();
			} else {
				currentNumber++;
			}

			buttonObjectPool.ReturnObject(button);
			buttonGameObjects.Remove(button);
		} else {
			FinishLevel();

			RemoveAllButtons();

			Score = 0;
			level = (int)PreferredLevel;
		}
	}

	public void StartButtonPointerUp() {
		StartCoroutine(HideNumbers(0));
		startButton.SetActive(false);
	}

	private void RemoveAllButtons() {
		while (buttonGameObjects.Count > 0) {
			buttonObjectPool.ReturnObject(buttonGameObjects[0]);
			buttonGameObjects.RemoveAt(0);
		}
	}

	private IEnumerator HideNumbers(float miliseconds) {
		yield return new WaitForSeconds(miliseconds/1000);
		
		foreach (Transform child in mainPanel) {
			child.gameObject.GetComponentInChildren<Text>().enabled = false;
		}
	}
}
