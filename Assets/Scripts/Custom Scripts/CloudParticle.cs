/*
 * @className		CloudParticle
 * @project			TheFoehnThing
 * @lastModified	2014-07-14
 * @author 			Tim J. Peters <tim.peters@stud.h-da.de>
 */

using UnityEngine;
using System.Collections;

public class CloudParticle {
	#region Variables
	public float size = .8f;
	public float opacity = 0.1f;
	public Vector3 position;
	public float rotation;
	public bool visible = true;
	private Vector3 direction;
	private float maxOpacity = 0.5f;
	private float movementSpeed = 0.005f;
	private float increaseFactor = 0.008f;
	private float decreaseFactor = 0.006f;
	private float rotationSpeed;
	private bool hadPeak = false;
	#endregion

	public CloudParticle(Vector3 _position, Vector3 _direction) {
		position = _position;
		direction = _direction;
		rotation = Random.Range(0,359); // degree-1
		rotationSpeed = Random.Range(-.5f,.5f);
	}
	
	public void move() {
		if(visible)
		{
			// move and rotate cloud
			position += movementSpeed*direction;
			rotation += rotationSpeed;
			
			// fade in (opacity and size) until peak, than fade out
			if(hadPeak)
			{
				opacity -= decreaseFactor;
				size *= (1-(decreaseFactor/2));
				if(opacity <= 0) visible = false;
			} else {
				size *= (1+(increaseFactor/2));
				opacity += increaseFactor;
				if(opacity >= maxOpacity) hadPeak = true;
			}
		}
	}
}