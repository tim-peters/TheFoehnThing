/*
 * @className		AllCloudParticle
 * @project			TheFoehnThing
 * @lastModified	2014-07-14
 * @author 			Tim J. Peters <tim.peters@stud.h-da.de>
 */

using UnityEngine;
using System.Collections;

[AddComponentMenu("Föhn/Add AllCloudParticle to ParticleSystem")]
public class AllCloudParticle : MonoBehaviour {
	#region Variables
	public int maxAmountParticles = 1000;
	private int amountParticles = 0;
	private int indexParticles = 0;
	private CloudParticle[] particles;
	private ParticleSystem.Particle[] points;
	private AllWindParticles allWindParticles;
	#endregion

	void Awake () {
		allWindParticles = GameObject.Find ("Main Camera/Wind Particle System").GetComponent<AllWindParticles>();
		particles = new CloudParticle[maxAmountParticles]; // initialize particle array
	}

	void Update () {
		for(int i = 0; i<amountParticles; i++) // move each existing particle
			if(particles[i].visible) particles[i].move();
		if(amountParticles > 0) renderNow(); // and render
	}

	public void create(Vector3 position, Vector3 direction) {
		if(allWindParticles.running) {
			particles[indexParticles] = new CloudParticle(position,direction); // create Cloud
			if(indexParticles >= maxAmountParticles-1) indexParticles = 0; else indexParticles++; // if array is full, restart index
			if (amountParticles < indexParticles) amountParticles = indexParticles;
		}
	}

	private void renderNow() {
		int pointsLength = 0;
		for(int n = 0;n<amountParticles;n++)
			if(particles[n].visible) pointsLength++;

		points = new ParticleSystem.Particle[pointsLength]; // pass necessary values from cloud array to particle system (as points array) 
		for(int n = 0, i = 0;n<amountParticles;n++) {
			if(particles[n].visible)
			{
				points[i].position = particles[n].position;
				if(Physics.Linecast(points [i].position,Camera.main.transform.position))
					points[i].color = Color.Lerp(new Color(1f,1f,1f,particles[n].opacity),new Color(0,0,0,particles[n].opacity),0.6f);
				else
				points[i].color = new Color(1f,1f,1f,particles[n].opacity);
				points[i].size = particles[n].size;
				points[i].rotation = particles[n].rotation;
				i++;
			}
		}
		particleSystem.SetParticles (points, points.Length);
	}
}
