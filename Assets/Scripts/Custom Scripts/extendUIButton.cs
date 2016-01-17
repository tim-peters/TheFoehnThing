/*
 * @className		extendUIButton
 * @project			TheFoehnThing
 * @lastModified	2014-07-14
 * @author 			Kai Zwier <kai.p.zwier@stud.h-da.de>
 */

using UnityEngine;
using System.Collections;

public class extendUIButton : MonoBehaviour {
	
	private ChangePiece changePiece;

	public GameObject wrapper;
	public GameObject slider;
	public GameObject label;
	
	private string newText;
	System.Threading.Timer timer;

	// 0 = closed; 1 = open;
	private int state = 0;

	void Start(){
		changePiece = GameObject.Find ("Main Camera").GetComponent<ChangePiece> ();

		if (label != null) state = 1;
	}

	/*
		Slider Animations (temperature/humidity)
		- toggle visibility, position and scale based on current state
	*/
	public void animateSlider () {
		if (state == 0) {
			wrapper.GetComponent<TweenScale>().Toggle();
			wrapper.GetComponent<TweenPosition>().Toggle();
			wrapper.GetComponent<TweenAlpha>().Toggle();
		} else {
			wrapper.GetComponent<TweenScale>().Toggle();
			wrapper.GetComponent<TweenPosition>().Toggle();
			wrapper.GetComponent<TweenAlpha>().Toggle();
		}
		state = 1 - state;
	}

	/*
		Mountain Selection
		- updating button label
		- pass variable to our piece handler -> starting animations
	*/
	public void nextMountain(){
		if (state < 2) {
			state++;
		} else {
			state = 0;		
		}

		switch (state) {
		case 0:
			newText = "S";
			break;
		case 1:
			newText = "M";
			break;
		case 2:
			newText = "L";
			break;
		}
		label.GetComponent<UILabel> ().text = newText;
		changePiece.changePieceTo (state);
	}
}
