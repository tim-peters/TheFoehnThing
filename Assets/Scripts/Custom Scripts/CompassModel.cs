/*
 * @className		CompassModel
 * @project			TheFoehnThing
 * @lastModified	2014-07-14
 * @author 			Tim J. Peters <tim.peters@stud.h-da.de>
 * @author			Kai Zwier <kai.p.zwier@stud.h-da.de>
 */

using UnityEngine;
using System.Collections;

public class CompassModel : MonoBehaviour {
	public bool active = false;
	private int _currentPiece;
	public int currentPiece{
		get { return _currentPiece; }
		set { _currentPiece = value; }
	}
	private MountainHandler mountainHandler;
	private globalUIHandler ui;
	private MainCameraHandler mainCameraHandler;
	private ChangePiece changePiece;

	void Start() {
		mountainHandler = GameObject.Find ("Map").GetComponent<MountainHandler> ();
		mainCameraHandler = Camera.main.GetComponent<MainCameraHandler> ();
		changePiece = Camera.main.GetComponent<ChangePiece> ();
		ui = GameObject.Find ("UI Root").GetComponent<globalUIHandler> ();
	}

	/*
		Starting Compass
		- updating active piece for positioning of the compass and camera
		- stop/prevent old animations
		- hide ui elements
		- fade in ui elements of compass
	*/
	public void startCompassMode(int pieceId) {
		active = true; 
		currentPiece = pieceId;
		mountainHandler.dropPiece (pieceId,0f);
		mainCameraHandler.startCompassView (pieceId);
		// Compass einlenden
		transform.position = changePiece.rotationMiddlePositions[pieceId] + new Vector3(0,mainCameraHandler.cameraHeightCompass,0) - new Vector3 (0, 36.5f, 0);
		iTween.Stop (gameObject);
		ui.fadeOut ();
		//iTween.FadeTo(gameObject, 1f, 1f);
		iTween.FadeTo (gameObject, iTween.Hash("duration", 0.5f, "delay", 1f, "alpha", 1f, "includechildren", true));
	}

	/*
		Confirm Compass Values/Close Compass Mode
		- stop/prevent old animations
		- fade out compass ui
		- fade in normal ui elements
		- passing chosen degreevalue to mountainhandler -> displays correct map
		- lift animation of new piece
		- animate camera to the chosen degreestep of the current piece
	*/
	public void confirmDegreeValue(int degreeStep) {
		active = false;
		// Compass ausblenden
		iTween.Stop (gameObject);
		iTween.FadeTo (gameObject, iTween.Hash("duration", 0.1f, "alpha", 0f, "includechildren", true));
		ui.setOrientation (degreeStep);
		ui.fadeIn ();
		mountainHandler.setLandscapeDegree (degreeStep);
		changePiece.handleNewPiece ();
		mainCameraHandler.newPieceCamHandler (currentPiece, "disableCompass");
	}
}
