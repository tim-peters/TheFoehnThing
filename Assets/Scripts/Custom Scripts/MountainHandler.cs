/*
 * @className		MountainHandler
 * @project			TheFoehnThing
 * @lastModified	2014-07-13
 * @author 			Tim J. Peters <tim.peters@stud.h-da.de>
 * @author			Kai Zwier <kai.p.zwier@stud.h-da.de>
 */

using UnityEngine;
using System.Collections;

public class MountainHandler : MonoBehaviour {
	# region variables
	public int degreeStep = 0;

	private Vector3[] _piecePositions = new Vector3 [3]{
		new Vector3 (-5.5269f,0,-41.14994f), 
		new Vector3 (-18.92916f,0,-14.30336f),
		new Vector3 (2.08702f,0,35.75724f)
	};
	public Vector3[] piecePositions{
		get { return _piecePositions; }
		set { _piecePositions = value; }
	}
	
	public GameObject[] centerMaps = new GameObject[16];
	public GameObject[] storePieceMaps = new GameObject[48];
	public GameObject[,] pieceMaps = new GameObject[16,3];
	public GameObject[] storePiecePartMaps = new GameObject[48];
	private GameObject[,,] piecePartMaps = new GameObject[16,3,2];
	private float animDuration;
	private ChangePiece changePiece;
	private AllWindParticles allWindParticles;
	#endregion
	
	void Awake() {
		int n = 0;
		for(int x = 0;x<16;x++)
		for(int i = 0;i<3;i++) {
			pieceMaps[x,i] = storePieceMaps[n];
			n++;
		}
		n = 0;
		for (int x=0; x<16; x++)
			for (int i=0; i<3; i++) {
				piecePartMaps[x,i,0] = storePiecePartMaps[n];
				n++;
				piecePartMaps[x,i,1] = storePiecePartMaps[n];
				if(n>=47) n=0; else n++;
			}

		allWindParticles = GameObject.Find ("Main Camera/Wind Particle System").GetComponent<AllWindParticles>();
		changePiece = Camera.main.GetComponent<ChangePiece> ();
	}
	

	/*
		setLandscapeDegree
		- deactivating of the olds landscape center
		- activating of the new landscape center
		- redefine global variable
	*/
	public void setLandscapeDegree(int newStep){
		centerMaps [degreeStep].SetActive (false);
		centerMaps [newStep].SetActive (true);
		degreeStep = newStep;
	}
	

	/*
		lift new piece
		- prevent old animations
		- fade to lighter texture
		- move down
	*/
	public void liftPiece(int pieceId, float delay){
		iTween.Stop (pieceMaps [degreeStep, pieceId], "MoveTo");
		//movePosition (pieceId, 3f, delay);
		fadeLight (piecePartMaps [degreeStep, pieceId,0]);
		fadeLight (piecePartMaps [degreeStep, pieceId,1]);
		iTween.MoveTo (pieceMaps [degreeStep, pieceId], iTween.Hash ("x", _piecePositions[pieceId].x, "y", _piecePositions[pieceId].y + 3f, "z", _piecePositions[pieceId].z, "time", 1f, "delay", delay,  "onComplete", "startParticles", "onCompleteTarget", gameObject, "easetype", iTween.EaseType.easeInQuad));
		
	}
	
	/*
		lift new piece
		- prevent old animations
		- stop windparticle animation
		- fade to darker texture
		- move piece down
	*/
	public void dropPiece(int pieceId, float delay){
		iTween.Stop (pieceMaps [degreeStep, pieceId], "MoveTo");
		allWindParticles.stop ();
		changePiece.indikatorLayer.fadeOut (0);
		fadeDark (piecePartMaps [degreeStep, pieceId,0]);
		fadeDark (piecePartMaps [degreeStep, pieceId,1]);
		iTween.MoveTo (pieceMaps [degreeStep, pieceId], iTween.Hash ("x", _piecePositions[pieceId].x, "y", _piecePositions[pieceId].y, "z", _piecePositions[pieceId].z, "time", 2f, "delay", delay));
		
	}
	
	// fade to lighter texture
	private void fadeLight(GameObject mapPiece) {
		iTween.ColorTo (mapPiece, iTween.Hash("color",new Color(1,1,1),"time", 0.8f,"delay",2.2f,"easetype", iTween.EaseType.easeInQuad));
	}

	// fade to darker texture
	private void fadeDark(GameObject mapPiece) {
		iTween.ColorTo (mapPiece, iTween.Hash("color",new Color(0.196f,0.196f,0.196f),"time", 2f));
	}

	private void updateMapColor(float val, GameObject mapPiece) {
		mapPiece.GetComponent<Renderer>().material.color = new Color(val,val,val);	
	}
	
	private void startParticles(){
		allWindParticles.go ();
	}
	
	public int GetDegreeStep() {
		return degreeStep;
	}
	
}
