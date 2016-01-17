/*
 * @className		DebugInputPosition
 * @project			TheFoehnThing
 * @lastModified	2014-07-14
 * @author 			Tim J. Peters <tim.peters@stud.h-da.de>
 *
 * Visualisiert die Touch- oder Mauseingabe (macht die sichtbar).
 */

using UnityEngine;
using System.Collections;

public class DebugInputPosition : MonoBehaviour {

	private Vector2 mouseInput;
	public Texture aTexture;
	private int id;
	private W7Touch ress;

	void Update () {
		if(gameObject.GetComponent<W7TouchManager>().isWinTouchDevice) {
			id = W7TouchManager.GetTouchCount ();
		} else 
			mouseInput = Input.mousePosition;
	}

	void OnGUI () {
		if(gameObject.GetComponent<W7TouchManager>().isWinTouchDevice) {
			for (int n=0; n<id; n++) {
				ress = W7TouchManager.GetTouch(n);
				GUI.DrawTexture (new Rect (ress.Position.x-20, Screen.height-ress.Position.y-20, 40, 40), aTexture, ScaleMode.ScaleToFit);
			}
		} else {
			if (Input.GetMouseButton(0))
				GUI.DrawTexture (new Rect (mouseInput.x-20, Screen.height-mouseInput.y-20, 40, 40), aTexture, ScaleMode.ScaleToFit);
		}
	}
}


