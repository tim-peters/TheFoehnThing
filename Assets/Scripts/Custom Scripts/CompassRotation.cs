/*
 * @className		CompassRotation
 * @project			TheFoehnThing
 * @lastModified	2014-07-14
 * @author 			Tim J. Peters <tim.peters@stud.h-da.de>
 */

using UnityEngine;
using System.Collections;

public class CompassRotation : MonoBehaviour {

	private bool lerp = true; // continuous | discrete steps of movement
	private float lookSpeed = 3.0f; // rotation speed (to step)
	private Quaternion newRotation;

	void Start() {
		newRotation = transform.rotation;
	}

	void Update(){
		if(gameObject.transform.parent.GetComponent<CompassModel>().active) { // if in compass mode
		
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Plane plane = new Plane(transform.up, transform.position); // build an invisible plan around needle, with which the ray can collide
			float dist;
			plane.Raycast(ray, out dist);

			Quaternion oldRotation = transform.rotation;
			if (Input.GetMouseButton (0)) {
				if(Vector3.Distance(transform.position, ray.GetPoint (dist)) > 1.5) { // if mouse != on button
						// Get rotation of needle looking to mouse...
						transform.LookAt (ray.GetPoint (dist), Vector3.up);
						Vector3 rotation = transform.rotation.eulerAngles;
						float yAngleClamped = clampAngle (rotation.y);
						// ... and rotate to nearest step
						for (int n = 0; n<16; n++) {
								if (yAngleClamped < clampAngle ((n * 22.5f) + (22.5f / 2)) && yAngleClamped > clampAngle ((n * 22.5f) + (22.5f / 2))-22.5f) {
										newRotation = Quaternion.Euler (0, (n * 22.5f), 0);
										break;
								}

						}
				} else { // ..else handle click on button
					int degreeStep = (int)(newRotation.eulerAngles.y/22.5f);
					gameObject.transform.parent.gameObject.GetComponent<CompassModel>().confirmDegreeValue(degreeStep);
				}
			}

			if(lerp){
				transform.rotation = Quaternion.Slerp(oldRotation, newRotation, lookSpeed * Time.deltaTime);
			}
		}
	}
	private static float clampAngle(float angle) {
		if(angle < 0)
			angle += 360;
		if(angle > 360f)
			angle -= 360f;
		
		if(angle < 0 || angle > 360)
			return clampAngle(angle);
		else
			return angle;
	}
}