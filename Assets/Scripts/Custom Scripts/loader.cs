using UnityEngine;
using System.Collections;

public class loader : MonoBehaviour {

	public Texture2D logo;
	public GameObject progressTxt;
	private int isloading = 0;

	void OnGUI () {
		GUI.DrawTexture(new Rect(Screen.width/2-440, Screen.height/2-162, 881, 162), logo, ScaleMode.StretchToFill);
	}

	void Update () {
		if(isloading == 1) {
			progressTxt.GetComponent<GUIText>().text = "loading...";
			isloading++;
			Application.LoadLevel("mainScene");
		} else {
			isloading++;
		}
	}
	
}
