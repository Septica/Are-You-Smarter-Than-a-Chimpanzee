using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour {
	private GameController gameController;

	private int n;
	public int N {
		get {
			return n;
		}
		set {
			n = value;
			GetComponentInChildren<Text>().text = n.ToString();
		}
	}

	void Start() {
		gameController = FindObjectOfType<GameController>();
	}

	public void Setup(int n, Vector2 position, float size) {
		RectTransform rectTransform = GetComponent<RectTransform>();

		N = n;
		rectTransform.anchoredPosition = position;
		rectTransform.sizeDelta = new Vector2(size, size);

		GetComponentInChildren<Text>().enabled = true;
	}

	public void HandleClick() {
		gameController.ButtonClicked(gameObject);
	}
}
