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
	private int ismoving;

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
			ismoving = (Mathf.Abs(Input.GetAxis("Mouse X"))+Mathf.Abs(Input.GetAxis("Mouse Y")) > 0) ? 10 : ismoving-1;
			if (Input.GetMouseButton(0) && ismoving > 0) {
				GUI.color = new Color(1f,1f,1f,(1f/10)*ismoving);
				GUI.DrawTexture (new Rect (mouseInput.x-15, Screen.height-mouseInput.y-15, 30, 30), aTexture, ScaleMode.ScaleToFit);
			}
		}
	}
}


