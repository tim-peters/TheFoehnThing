/*
 * @className		MainCameraHandler
 * @project			TheFoehnThing
 * @lastModified	2014-07-14
 * @author 			Tim J. Peters <tim.peters@stud.h-da.de>
 * @author			Kai Zwier <kai.p.zwier@stud.h-da.de>
 */

using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse drag Orbit with zoom")]
public class MainCameraHandler : MonoBehaviour
{
	#region publicVariables
	public Transform target;
	public float distance = 18.0f; // start distance to target
	public float xSpeed = 0.1f; // speed for horizontal rotation
	public float ySpeed = 1.0f; // speed for vertical rotation
	public float yMinLimit = 10f; // min vertical rotation (in degree) 
	public float yMaxLimit = 80f; // max vertical rotation (in degree)
	public float distanceMin = 10f;
	public float distanceMax = 25f;
	public float smoothTime = 1f; 
	public int uiInteraction = 0;	

	private float _rotationYAxis = 0.0f;
	public float rotationYAxis {
		get {
			return _rotationYAxis;
		}
		set {
			_rotationYAxis = value;
		}
	}

	private float _rotationXAxis = 0.0f;
	public float rotationXAxis {
		get {
			return _rotationXAxis;
		}
		set {
			_rotationXAxis = value;
		}
	}

	private float _cameraHeightCompass = 40f;
	public float cameraHeightCompass {
		get { return _cameraHeightCompass; }
		set { _cameraHeightCompass = value; }
	}

	private Vector3[] _cameraPositions;
	public Vector3[] cameraPositions {
		get { return _cameraPositions; }
		set { _cameraPositions = value; }
	}

	private bool _isSwitching = false;
	public bool isSwitching {
		get { return _isSwitching; }
		set { _isSwitching = value; }
	}

	private float _velocityX = 0.0f;
	public float velocityX {
		get { return _velocityX; }
		set { _velocityX = value; }
	}

	private float _velocityY = 0.0f;
	public float velocityY {
		get { return _velocityY; }
		set { _velocityY = value; }
	}

	private bool _showCompass = false;
	public bool showCompass {
		get { return _showCompass; }
		set { _showCompass = value; }
	}

	private bool _showMode = false;
	public bool showMode {
		get { return _showMode; }
		set { _showMode = value; }
	}

	#endregion
	#region privateVariables
	private MountainHandler mountainHandler;
	private CompassModel compassModel;
	private ChangePiece changePiece;
	private float offsetY = 5; // in °
	private float zoomInput;
	private Vector3 position;
	private Vector3 rotation;
	private bool deltaReset = true, 
				multiDeltaReset = true; 
	private Vector3 compassCameraDirectionEuler = new Vector3(90,0,0);
	private Vector3[,] leavingCameraPositions = new Vector3[16,3];
	private Vector3[] rotationMiddle;
	private Vector3 oldPosition,
				oldRotation;
	private Vector3 compassPosition = new Vector3(0f,47f,-8.7f);
	private Vector3 compassRotation = new Vector3(90f,0f,0f);
	private float showModeXSpeed = 0.03f;
	#endregion

	void Start()
	{
		mountainHandler = GameObject.Find ("Map").GetComponent<MountainHandler> ();
		compassModel = GameObject.Find ("Compass").GetComponent<CompassModel> ();
		changePiece = gameObject.GetComponent<ChangePiece> ();
		rotationMiddle = GameObject.Find ("Main Camera").GetComponent<ChangePiece> ().rotationMiddlePositions;
		
		for (int n = 0; n<16; n++)
				for (int i = 0; i<3; i++) // calculate camera positions on leaving the compass mode
					leavingCameraPositions [n, i] = rotationMiddle [i] + (Quaternion.Euler (0, 22.5f * n, 0) * new Vector3(19.696f,3.473f,0));// x = Distance * sin(80) / sin(90); y = Distance * sin(10) / sin(90)

		Vector3 angles = transform.eulerAngles;
		rotationYAxis = angles.y;
		rotationXAxis = angles.x;
		
		// Make the rigid body not change rotation
		if(rigidbody)
		{
			rigidbody.freezeRotation = true;
		}
	}
	
	void LateUpdate()
	{
		if (!showCompass) {
			if (!isSwitching)
			{
				if(!UICamera.hoveredObject){
				if(gameObject.GetComponent<W7TouchManager>().isWinTouchDevice) // Touch input handler
				{
					int touchCount = W7TouchManager.GetTouchCount();
					if(touchCount == 0) {
						deltaReset = true;
						multiDeltaReset = true;
					} else {
						showMode = false;
						if(!UICamera.hoveredObject) {
							if(deltaReset)
								deltaReset = false;
							else {
								W7Touch touch_1 = W7TouchManager.GetTouch(0);
								if(touchCount >= 2) { // swipe gesture recognition
									if(multiDeltaReset)
										multiDeltaReset = false;
									else {
										W7Touch touch_2 = W7TouchManager.GetTouch(1);
										float prevDistance = Vector2.Distance((touch_1.Position - touch_1.DeltaPosition), (touch_2.Position - touch_2.DeltaPosition));
										float curDistance = Vector2.Distance(touch_1.Position, touch_2.Position);
										zoomInput = (curDistance-prevDistance)*0.01f;
									}
								} else { // drag gesture recognition
									multiDeltaReset = true;
									zoomInput = 0;
									velocityX *= 0.5f;
									velocityY *= 0.5f;
									velocityX += xSpeed * touch_1.DeltaPosition.x * distance * 0.1f;
									velocityY += ySpeed * touch_1.DeltaPosition.y * 0.1f;
								}
							}
						}
					}
				}
				else {
					if (Input.GetMouseButton(0)) // Mouse input handler
					{
						velocityX *= 0.5f;
						velocityY *= 0.5f;
						velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.3f;
						velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.3f;

						showMode = false;
					}
					zoomInput = Input.GetAxis("Mouse ScrollWheel");
				}
				}

			// Add "drag force" to rotations
			if(!showMode) {
				rotationYAxis += velocityX;
				rotationXAxis -= velocityY;
			} else {
				rotationXAxis += showModeXSpeed;
				if(rotationXAxis >= 40 || rotationXAxis <= yMinLimit) showModeXSpeed *= -1;
				rotationYAxis -= 0.1f;
				zoomInput = -0.005f;
			}
			
			rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
			
			} 

			Quaternion rotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0); 
			
			distance = Mathf.Clamp(distance - zoomInput * 5, distanceMin, distanceMax);

			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + target.position;
			
			transform.rotation = rotation*Quaternion.Euler (0,offsetY,0);
			transform.position = position;
			
			// smooth movement
			velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
			velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
		}
	}

	public void startCompassView(int pieceId){
		iTween.Stop (gameObject);
		iTween.MoveTo(gameObject, iTween.Hash("x", rotationMiddle[pieceId].x, "y",rotationMiddle[pieceId].y+cameraHeightCompass, "z", rotationMiddle[pieceId].z, "time", 4f));
		iTween.RotateTo(gameObject, iTween.Hash("x", compassCameraDirectionEuler.x, "y", compassCameraDirectionEuler.y, "z", compassCameraDirectionEuler.z, "delay", 0f, "time", 3f));
		showCompass = true;
	}

	public void newPieceCamHandler(int pieceId, string callback){
		int degreeStep = mountainHandler.GetDegreeStep ();

		float newYAngle = customClampAngle(calculateYAngle(degreeStep));
		iTween.Stop (gameObject);
		if (callback == "disableCompass") {
			Vector3 move = leavingCameraPositions[degreeStep, changePiece.currentPiece];
			iTween.RotateTo(gameObject, iTween.Hash("x", compassCameraDirectionEuler.x, "y", calculateYAngle(degreeStep), "z", compassCameraDirectionEuler.z, "delay", 0f, "time", 2f));
			iTween.MoveTo(gameObject, iTween.Hash("x", move.x, "y", move.y, "z", move.z, "delay", 1f, "time", 4f, "onComplete", "disableCompass"));
			iTween.RotateTo(gameObject, iTween.Hash("x", 10f, "y", calculateYAngle(degreeStep)+offsetY, "z", 0, "delay", 1f, "time", 3f));
			rotationXAxis = 10f;
			rotationYAxis = calculateYAngle(degreeStep);
			distance = 20f;
		} else {
			Hashtable paramX = new Hashtable ();
			paramX.Add ("from", customClampAngle (rotationXAxis));
			paramX.Add ("to", 10f);
			paramX.Add ("time", 3.0f);
			paramX.Add ("onUpdate", "TweenRotationXAxis");
			paramX.Add ("onComplete", "enableDragOrbit");
			paramX.Add ("easetype", iTween.EaseType.easeInOutQuad);
			iTween.ValueTo (gameObject, paramX);

			Hashtable paramY = new Hashtable ();
			paramY.Add ("from", customClampAngle (rotationYAxis));
			paramY.Add ("to", newYAngle);
			paramY.Add ("time", 3.0f);
			paramY.Add ("onUpdate", "TweenRotationYAxis");
			paramY.Add ("onComplete", "enableDragOrbit");
			paramY.Add ("easetype", iTween.EaseType.easeInOutQuad);
			iTween.ValueTo (gameObject, paramY);

			Hashtable paramZ = new Hashtable ();
			paramZ.Add ("from", distance);
			paramZ.Add ("to", 20f);
			paramZ.Add ("time", 3.0f);
			paramZ.Add ("onUpdate", "TweenDistance");
			paramZ.Add ("onComplete", "enableDragOrbit");
			paramZ.Add ("easetype", iTween.EaseType.easeInOutQuad);
			iTween.ValueTo (gameObject, paramZ);
		}

	}

	private void TweenRotationXAxis(float val) { rotationXAxis = val; }

	private void TweenRotationYAxis(float val) { rotationYAxis = val; }

	private void TweenDistance(float val) { distance = val; }

	private void disableCompass() { showCompass = false; }

	private void enableDragOrbit() { isSwitching = false; }

	private void enableInteractionmode() { uiInteraction = 1; }

	private void disableInteractionmode() {	uiInteraction = 0; }

	private float calculateYAngle(int degreeStep) { return degreeStep * 22.5f - 90; }

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}

	private float customClampAngle(float angle) {
		if(angle < 0)
			angle += 360;
		if(angle > 360f)
			angle -= 360f;
		
		if(angle < 0 || angle > 360)
			return customClampAngle(angle);
		else
			return angle;
	}
}
	
