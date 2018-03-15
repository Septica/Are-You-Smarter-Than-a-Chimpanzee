using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSettings : MonoBehaviour {
	public void OnClick(GameObject settingsPanel) {
		settingsPanel.SetActive(!settingsPanel.activeSelf);
	}
}
