/*
 * @className		IndikatorLayer
 * @project			TheFoehnThing
 * @lastModified	2014-07-14
 * @author 			Tim J. Peters <tim.peters@stud.h-da.de>
 */

using UnityEngine;
using System.Collections;

public class IndikatorLayer : MonoBehaviour {
	private float pieceDepth = 24;
	private float positionValue = .5f;
	private Vector3 startPos, endPos;
	private ChangePiece changePiece;
	
	void Awake() {
		changePiece = Camera.main.GetComponent<ChangePiece> ();
	}
	
	public void SetNewPosition(Vector3 position, Quaternion direction) { // calculate start- & end layer positions
		startPos = position-(direction*new Vector3(0,0,pieceDepth/2));
		endPos = position+(direction*new Vector3(0,0,pieceDepth/2));
		transform.rotation = direction;
		SetNewPositionValue(positionValue);
	}
	
	public void SetNewPositionValue(float val) { // calculate actual layer position
		transform.position = Vector3.Lerp(startPos,endPos,val);
		positionValue = val;
	}
	
	public void fadeIn(float delay) {
		iTween.FadeTo (gameObject, iTween.Hash("duration", 0.7f, "alpha", 0.6f, "includechildren", true,"delay",delay, "onStart", "fireMoveFoehnIndikator"));
	}
	
	public void fadeOut(float delay) {
		iTween.FadeTo (gameObject, iTween.Hash("duration", 0.5f, "alpha", 0f, "includechildren", true,"delay",delay));
	}
	
	public void fireMoveFoehnIndikator() { // needed to pass iTween callback to another object
		changePiece.moveFoehnIndikator ();
	}
}
