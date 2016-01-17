/*
 * @className		AllWindParticles
 * @project			TheFoehnThing
 * @lastModified	2014-07-14
 * @author 			Tim J. Peters <tim.peters@stud.h-da.de>
 */

using UnityEngine;
using System.Collections;

[AddComponentMenu("Föhn/Add AllWindParticle to ParticleSystem")]
public class AllWindParticles : MonoBehaviour {
	#region PublicVariables
	public bool running = false;
	public int amountX = 12; // Amount of particles per row
	public int amountY = 2;  // Amount of particles per column
	[Range(0,100)]	public float distanceBetweenParticles = 1; 
	[Range(0.0001f,49)]	public float sizeOfParticles = 0.3f; // Visual size of particles
	[Range(0,49)]	public int trailLength = 36; // Length of trail (in particles)
	public int maxParticles = 10000; // Max amount of particles to be rendered
	public int maxMountainHeight = 4; // Important for realistic flow
	public int maxDistanceFromSource = 24; // Distance from source at which particles disappear
	public float speed = 0.1f;
	public float temperature = 18f;
	public float saturation = 80f; // Relative Luftfeuchte
	public Gradient colorArea = new Gradient (); // Color of particles depending on temperature
	public int pauseBetweenWaves = 2; // for ordered spawn mode
	public bool randomPositioning = true; // random/ordered spawn mode
	public Transform sourceObject; // object which act as source (position, rotation...)
	#endregion
	#region PrivateVariables
	private Vector3 position;
	private Quaternion direction;
	private int waveCounter, amountParticles = 0, indexParticles = 0;
	private WindParticle[] particles;

	private ParticleSystem.Particle[] points;
	private ParticleSystem.Particle[][] pointsArchiv;
	private int pointsArchivI = 0;
	#endregion

	void Start() {
		trailLength++; // add first particle to trail amount
		pointsArchiv = new ParticleSystem.Particle[trailLength][];

		waveCounter = pauseBetweenWaves; // starting immediatly (just for ordnered mode)

		//position = sourceObject.position+(direction * new Vector3(-distanceBetweenParticles*amountX/2,-distanceBetweenParticles*amountY/2,0));
		position = sourceObject.position;
		direction = sourceObject.rotation;

		particles = new WindParticle[maxParticles]; // array of windParticles
		colorArea.SetKeys (new GradientColorKey[]{new GradientColorKey(new Color(1f,0,0),1), new GradientColorKey(new Color(0.478431f,0.478431f,0.478431f),0.5f), new GradientColorKey(new Color(0,0.3372549f,1),0)},new GradientAlphaKey[]{new GradientAlphaKey(1,0)});

		// necessary on start?
		//create ();
		//renderNow ();
	}

	void Update() {
		position = sourceObject.position+(direction * new Vector3(distanceBetweenParticles/2-distanceBetweenParticles*amountX/2,-distanceBetweenParticles*amountY/2,0));
		direction = sourceObject.rotation;
		if(running) {
			create (); // create new particles
			for(int i = 0; i<indexParticles; i++) // move each existing particle
				if(particles[i].visible) particles[i].move();
		}
		//if(sourceTarget != null) sourceTarget.transform.Find("Cube").localScale = new Vector3((amountX*distanceBetweenParticles)/sourceTarget.transform.localScale.x,(amountY*distanceBetweenParticles)/sourceTarget.transform.localScale.z,0.01F); // resize spawn object to show at which area particle are getting spawned
		renderNow ();
	}

	private void create() {
		if (amountParticles + amountX * amountY > maxParticles) // avoid creating more particles than maxParticles 
			amountParticles = 0; // ...by setting index to 0 again
		if(waveCounter >= pauseBetweenWaves) {
			if (randomPositioning) { // random mode
				/* Random on discret steps
				int x = Random.Range(0,amountX);
				int y = Random.Range(0,amountY);
				addParticle(amountParticles, direction * new Vector3(x*distanceBetweenParticles,y*distanceBetweenParticles,0f)+position); */
				/* Random on whole area */
				float x = Random.Range(0,amountX*distanceBetweenParticles);
				float y = Random.Range(0,amountY*distanceBetweenParticles);
				addParticle(amountParticles, direction * new Vector3(x,y,0f)+position); 
				amountParticles += 1;
			} else { // ordered mode
				for(int x = 0;x<amountX;x++) 
					for(int y = 0;y<amountY;y++) {
						addParticle(amountParticles, direction * new Vector3(x*distanceBetweenParticles,y*distanceBetweenParticles,0f)+position);
						amountParticles += 1;
				}
			}
			waveCounter = 0;
		}
		waveCounter++;
		if (indexParticles < amountParticles)
			indexParticles = amountParticles;
	}

	private void addParticle(int id, Vector3 position) {
		particles[id] = new WindParticle(id, position, maxDistanceFromSource, direction, speed, maxMountainHeight, temperature, saturation, colorArea);
	}

	private void renderNow() {
		pointsArchiv [pointsArchivI] = new ParticleSystem.Particle[indexParticles];
		for (int i = 0, ii = 0; i<indexParticles; i++) { // add actual particles to trail array
			if(particles[i].visible && running) {
				pointsArchiv [pointsArchivI][ii].position = particles[i].position;
				pointsArchiv [pointsArchivI][ii].color = particles[i].color;
				pointsArchiv [pointsArchivI][ii].size = sizeOfParticles;
				ii++;
			}
		}


		int pointsLength = 0; // to be calculated: Amount of existing particles
		for (int i = 0; i<trailLength && pointsArchiv[i] != null; i++)
			pointsLength += pointsArchiv [i].Length;

		points = new ParticleSystem.Particle[pointsLength];
		int trailI = pointsArchivI, n = 0, pointsI = 0; // TrailI = index, n = counter (max = railLength), index = points gesamt
		while (n < trailLength && pointsArchiv[trailI] != null) { // For each trail part
			for (int ii = 0; ii<pointsArchiv[trailI].Length; ii++) { // For each particle in this trail part
				// Write stored particles in one array (for ParticleSystem)
				points [pointsI].position = pointsArchiv[trailI][ii].position; 
				Color actualColor = pointsArchiv[trailI][ii].color;
				//points [pointsI].color = (randomPositioning) ? new Color(actualColor.r,actualColor.g,actualColor.b,1f-(n*(1f/trailLength))) : actualColor;
				points[pointsI].color = new Color(actualColor.r, actualColor.g, actualColor.b, 1f-(n*(1f/trailLength)));
				points [pointsI].size = sizeOfParticles;
				pointsI++;
			}
			if(trailI-- <= 0) trailI = trailLength-1;
			n++;
		}
		particleSystem.SetParticles (points, points.Length);
		pointsArchivI = (pointsArchivI+1 >= trailLength) ? 0 : pointsArchivI+1;
	}

	 public void setTemperature (float newTemperature){
		 temperature = newTemperature;
		 reset ();
	 }

	 public void setSaturation (float newSaturation){
		 saturation = newSaturation;
		 reset ();
	 }
	
	public void stop() {
		running = false;
	}
	
	public void go() {
		reset();
		running = true;
	}

	public void toggle() {
		running = !running;
	}

	public void reset() {
		waveCounter = pauseBetweenWaves;
		particles = new WindParticle[maxParticles];
		pointsArchiv = new ParticleSystem.Particle[trailLength][];
		pointsArchivI = 0;
		amountParticles = 0;
		indexParticles = 0;

	}
}
