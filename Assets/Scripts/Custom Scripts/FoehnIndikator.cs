/*
 * @className		FoehnIndikator
 * @project			TheFoehnThing
 * @lastModified	2014-07-14
 * @author 			Tim J. Peters <tim.peters@stud.h-da.de>
 * 
 * Ermittelt die Temperatur-, Höhen- und Sättigungswerte für Indikator-Ebene
 * Bei Veränderung der Landschaft (Bergwechsel) etc. muss zuerst neu instanziert werden und investigateMap() ausgeführt werden.
 * Ab dann steht über getValues() die vorher über steps festgelegte Anzahl an Werten zur Verfügung.
 */

using UnityEngine;
using System.Collections;

public class FoehnIndikator {
	private int steps;
	private float[] stepMap;
	private Vector3 position;
	private Quaternion direction;
	private float maxDistance;
	private float startTemperature;
	private float startSaturation;
	private float[] heights;
	private float[] temperatures;
	private float[] saturations;
	private WindParticle particle;
	
	public FoehnIndikator(	int _steps, Vector3 _position, Quaternion _direction, float _maxDistance, float _startTemperature, float _startSaturation) {
		steps = _steps;
		stepMap = new float[steps];
		position = _position;
		direction = _direction;
		maxDistance = _maxDistance;
		startTemperature = _startTemperature;
		startSaturation = _startSaturation;
		heights = new float[steps];
		temperatures = new float[steps];
		saturations = new float[steps];
		for (int n = 0; n<steps; n++) {
			stepMap [n] = Mathf.Lerp (0f, maxDistance, (1f + n) / steps);
		}
		investigateMap ();
	}
	
	public void investigateMap() { // Werte für aktuelle Landschaft in abhängigkeit zu Werten ermitteln und speichern
		Gradient colorArea = new Gradient ();
		particle = new WindParticle(0, position, 99999, direction, 0.1f, 999, startTemperature, startSaturation, colorArea); // create WindParticle
		
		temperatures[0] = startTemperature;
		saturations[0] = startSaturation;
		heights[0] = (position.y-particle.groundHeight)/particle.scale;
		
		int n = 0;
		while (n+1 < steps) {
			particle.move();
			if(Vector3.Distance(new Vector3(particle.position.x,0,particle.position.z),new Vector3(position.x,0,position.z)) >= stepMap[n]) { // if actual position is nearest to calculated distance for actual step...
				temperatures[n+1] = particle.temperature;
				saturations[n+1] = particle.getFRel();
				heights[n+1] = (particle.position.y-particle.groundHeight)/particle.scale;
				n++;
			}
		}
	}
	
	public float[] getValues(float stepValue) { // Werte aus vorher erstelltem Array ausgeben
		int step = Mathf.RoundToInt(stepValue*(steps-1));
		return new float[]{heights[step],temperatures[step],saturations[step]};
	}
}
