/*
 * @className		ChangePiece
 * @project			TheFoehnThing
 * @lastModified	2014-07-14
 * @author			Kai Zwier <kai.p.zwier@stud.h-da.de>
 * @author 			Tim J. Peters <tim.peters@stud.h-da.de>
 */

using UnityEngine;
using System.Collections;

public class ChangePiece : MonoBehaviour {
	#region publicVariables;
	private FoehnIndikator _foehnIndikator;
	public FoehnIndikator foehnIndikator {
		get { return _foehnIndikator;	}
		set {_foehnIndikator = value;}
	}

	private IndikatorLayer _indikatorLayer;
	public IndikatorLayer indikatorLayer {
		get { return _indikatorLayer;	}
		set {_indikatorLayer = value;}
	}

	private Vector3[] _rotationMiddlePositions = new Vector3[3] { // entspricht PiecePositions
		new Vector3(-5.526911f,0.3385153f,-41.14996f), // small/low
		new Vector3(-18.92917f,0.3385153f,-14.30336f), // medium
		new Vector3(2.08702f,0.3385153f,35.75725f) // big/high
	};
	public Vector3[] rotationMiddlePositions {
		get { return _rotationMiddlePositions; }
		set { _rotationMiddlePositions = value; }
	}

	private int _currentPiece = -1;
	public int currentPiece {
		get { return _currentPiece;	}
		set {_currentPiece = value;}
	}

	public int indikatorAccuracy = 100;
	public GameObject rotationMiddle; // to be draged
	public GameObject source; // to be draged
	public int degreeStep;

	#endregion
	#region privateVariables
	private AllWindParticles allWindParticles;
	private MountainHandler mountainHandler;
	private MainCameraHandler mainCameraHandler;
	private globalUIHandler ui;
	
	private Vector3[,] sourcePositions = new Vector3[16,3];
	private Vector3[,] indikatorSourcePositions = new Vector3[16, 3]; 
	#endregion
	
	void Start() {
		indikatorLayer = GameObject.Find ("IndikatorLayer").GetComponent<IndikatorLayer> ();
		mountainHandler = GameObject.Find ("Map").GetComponent<MountainHandler> ();
		mainCameraHandler = gameObject.GetComponent<MainCameraHandler> ();
		allWindParticles = GameObject.Find ("Main Camera/Wind Particle System").GetComponent<AllWindParticles> ();
		ui = GameObject.Find ("UI Root").GetComponent<globalUIHandler> ();
		
		// FIXME Besser wäre: Nochmal händisch auslesen und eintragen
		for (int n = 0; n<16; n++)
			for (int i = 0; i<3; i++) {
				sourcePositions[n,i] = rotationMiddlePositions[i]-(Quaternion.Euler (0,22.5f*n,0) * new Vector3(0f, -2f,12f));
			}
		indikatorSourcePositions = sourcePositions;

		// Initial: Mittleren Berg auswählen
		changePieceTo (1);
	}
	
	/*
		change piece function
		- comparison of current piece - new piece to prevent unneccesary animations
		- drop old piece
		- animation of cameras rotation middle
		- call new piece handler functions
	*/
	public void changePieceTo(int pieceId) {
		
		if(pieceId != currentPiece) {
			if(currentPiece >= 0) // Wenn ein Piece rausgehoben ist, lasse es absinken
				mountainHandler.dropPiece (currentPiece,0);
			
			mainCameraHandler.isSwitching = true;

			// Bewege die Camera zum neuen Stück
			iTween.MoveTo(rotationMiddle, iTween.Hash(
				"x", rotationMiddlePositions[pieceId].x, 
				"y", rotationMiddlePositions[pieceId].y, 
				"z", rotationMiddlePositions[pieceId].z, 
				"time", 2, 
				"easetype", iTween.EaseType.easeInOutQuad, 
				"delay", 1)
			              );
			mainCameraHandler.newPieceCamHandler (pieceId, "enableDragOrbit");

			handleNewPiece(pieceId);
		}
	}
	

	/*
		handle new piece
		- get current degree value
		- get position and rotation for the particle system
		- lift new piece
		- indication and fade in of foehn layer
	*/
	public void handleNewPiece(int pieceId = -1) {
		if(pieceId == -1) pieceId = currentPiece;
		degreeStep = mountainHandler.GetDegreeStep();
		source.transform.position = sourcePositions[degreeStep,pieceId];
		source.transform.rotation = Quaternion.Euler (0, (22.5f * degreeStep), 0);

		mountainHandler.liftPiece (pieceId, 2);
		currentPiece = pieceId;

		setNewFoehnIndikator();
		indikatorLayer.fadeIn (5);
	}

	public void setNewFoehnIndikator() {
		foehnIndikator = new FoehnIndikator(indikatorAccuracy,sourcePositions[degreeStep,currentPiece],Quaternion.Euler (0,(22.5f*degreeStep),0),allWindParticles.maxDistanceFromSource*0.999f,allWindParticles.temperature,allWindParticles.saturation);
		ui.checkInfowindowValues ();
	}

	public void moveFoehnIndikator() {
		indikatorLayer.SetNewPosition (new Vector3 (0, 3f, 0) + rotationMiddlePositions [currentPiece], Quaternion.Euler (0, 22.5f * degreeStep, 0));
	}
}
