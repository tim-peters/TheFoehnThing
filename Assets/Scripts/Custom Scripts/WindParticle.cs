/*
 * @className		WindParticle
 * @project			TheFoehnThing
 * @lastModified	2014-07-14
 * @author 			Tim J. Peters <tim.peters@stud.h-da.de>
 */

using UnityEngine;
using System.Collections;

public class WindParticle {
	#region PublicVariables
	public float scale = 0.000873476f; // Teilen durch scale macht unity-Einheit zu Meter (0.1 => 1 unity = 10 meter)
	public float groundHeight = 1.8f; // definiert die 0-Punkt-Höhe in Unity-Einheiten
	public bool visible;
	public Vector3 position;
	public float temperature; 
	public Color color; // actual color
	public float saturation; // specific saturation
	#endregion
	#region PrivateVariables
	private Gradient colorArea;
	private int maxDistance; // entspricht AllWindParticle.maxDistanceFromSource
	private float tempDiffProMeter = -0.0097644f; // Wert für trockenadiabatischen Temperaturgradienten
	private Vector3 actualDirection; // aktuelle, tatsächliche Richtung
	private Vector3 oldPosition;
	private Vector3 source;
	private Quaternion direction; // generelle Richtung
	private RaycastHit raycast;
	private RaycastHit otherRaycast;
	private float speed;
	private float condensingSaturation = 0;
	private int maxMountainHeight;

	private int id; // just for debugging / can be deleted after

	#endregion

	public WindParticle(int _id, Vector3 _position, int _maxDistance, Quaternion _direction, float _speed, int _maxMountainHeight, float _temperature, float _saturation, Gradient _colorArea) {
		id = _id;
		position = _position;
		source = _position;
		maxDistance = _maxDistance;
		direction = _direction;
		speed = _speed;
		maxMountainHeight = _maxMountainHeight;
		temperature = _temperature;
		saturation = getSFromFRel(_saturation,getPFromHeight((position.y-groundHeight)/scale),temperature); // saturation kommt als relative Luftfeuchte, daher die Umrechnung
		colorArea = _colorArea;
		visible = true;
	}
	public void move() {
		oldPosition = position;
		if (visible) {
			Vector3 direcForward = direction * Vector3.forward;
			Vector3 direcLeft = Vector3.Cross(direcForward,Vector3.up);
			if (Physics.Raycast (position, Vector3.down, out raycast)) { // Wenn Particle über Grund (Normalzustand)
				Vector3 tangent = Vector3.Cross (raycast.normal, direcLeft);
				if (raycast.distance <= maxMountainHeight) {
					float multiplier = (maxMountainHeight + 1 - raycast.distance) / maxMountainHeight; // calculate y-multiplier depending on height over ground and MaxMountainHigh (Range: 0 - ~1)
					actualDirection = new Vector3 (tangent.x * speed, tangent.y * speed * multiplier, tangent.z * speed);
				} else {
					actualDirection = direcForward * speed;
				}
			} else if (Physics.Raycast(position, Vector3.up, out otherRaycast)) { // Wenn Particle unter Grund (Fehlerkorrektur)
				// HACK: Da der MeshCollider nur auf Raycasts die ihn von oben treffen reagiert, wird dieser Bereich (solange die Landschaft aus Meshes besteht) nie ausgeführt
				Vector3 oppositeTangent = Vector3.Cross (otherRaycast.normal, -direcLeft);
				actualDirection = Vector3.up * otherRaycast.distance + new Vector3 (oppositeTangent.x * speed, oppositeTangent.y * speed, oppositeTangent.z * speed);
			} else { // Wenn kein Bezug
				actualDirection = direcForward * speed;
			}
			position += actualDirection;

			if(Vector3.Distance(position, source) > maxDistance)
				hide ();

			foehn (oldPosition, position); // Berechnung aller anderen Werte (außer Position)
		}


	}

	private void foehn(Vector3 oldPos, Vector3 newPos) {
		float height = (newPos.y-groundHeight)/scale;
		float heightDifference = (newPos.y - oldPos.y)/scale;

		temperature += tempDiffProMeter * heightDifference; // Trockenadiabatischer Temperaturgradient
		float pressure = getPFromHeight (height);

		if (getFRelFromS (saturation, pressure, temperature) > 100) // wenn relative Luftfeuchte > 100%
						condensation (pressure); 
		
		//float tempValue = (temperature > -10) ? (temperature < 45) ? map (temperature, -10,45,0,1) : 45 : -10;
		float tempValue = map (Mathf.Clamp (temperature, -10, 35), -10, 35, 0, 1);
		color = colorArea.Evaluate(tempValue);
	}

	private void condensation(float pressure) {
		float maxSaturation = getSFromFRel (100, pressure, temperature); // 100% rel. Luftfeuchte entspricht wieviel g/kg spez. Luftfeuchte?
		float saturationDiff = saturation - maxSaturation; // in Gramm
		condensingSaturation += saturationDiff;
		if(condensingSaturation >= .5f) { // Wenn bestimmte Kondensationsmenge erreicht
			// spawne eine Wolke
			GameObject.Find ("Cloud Particle System").GetComponent<AllCloudParticle>().create(position, new Vector3(actualDirection.x,actualDirection.y/2,actualDirection.z).normalized);
			condensingSaturation = 0;
		}
		warmingByCondensation(saturationDiff);
		saturation = maxSaturation;
	}

	private void warmingByCondensation (float saturationDiff) {
		float specificHeatCapacity = 1.0054f; // Spezifische Wärmekapazität von Luft in kJ/(kg K)
		float specificEnthalpyOfVaporization = 2.441176470588235f; // Spezifische Verdampfungsenthalpie von Wasser in kJ/g
		float verdampfungsEnergie = specificEnthalpyOfVaporization * saturationDiff; // in kJ
		float warming = specificHeatCapacity * verdampfungsEnergie; // in ° Kelvin
		temperature += warming;
	}
	
	public void hide() {
		visible = false;
	}

	#region HILFSFUNKTIONEN

	/* 
	 * Berechnet den Luftdruck in Abhängigkeit zur Höhe 
	 * @height = Höhe in Metern über Meeresspiegel 
	 */
	private float getPFromHeight(float height) {
		return 1013.15f - (height * 0.125f);
	}
	/* 
	 * Berechnet die spezifische Luftfeuchte (s) in g/kg [wasser/luft]
	 * @FRel = Relative Luftfeuchte in % 
	 * @P = Druck in hPa
	 * @T = Tempreatur in ° Celsius (NICHT KELVIN!!)
	 */
	private float getSFromFRel(float FRel, float P, float T) {
		float saettDampfdruck = 6.1078f * (float)System.Math.Exp(((17.08085f*T)/(T+234.175f)));
		float p1 = saettDampfdruck * FRel/100.0f;
		float eps=0.62161576815f;
		return 1000.0f*(p1*eps/(P-(1-eps)*p1));
	}
	/* Berechnet Relative Luftfeuchte in % (der maximal möglichen Feuchte bei dieser Temperatur & Druck
	 * @s = spezifische Luftfeuchte [g/kg]
	 * @P = Druck in hPa
	 * @T = Temperatur in ° Celsius (NICHT KELVIN!!)
	 */
	private float getFRelFromS(float s, float P, float T) {
		float eps = 0.62161576815f;
		float partialDruck = P*(s/1000.0f)/(eps+(1-eps)*(s/1000.0f));
		float seattDampfdruck = 6.1078f * (float)System.Math.Exp(((17.08085f*T)/(T+234.175f)));
		return 100f*partialDruck/seattDampfdruck;
	}
	/* Gleiches als Public Methode
	 * Holt sich seine Variablen von Attributen */
	public float getFRel() {
		return getFRelFromS (saturation, getPFromHeight ((position.y-groundHeight)/scale), temperature);
	}
	/* mapt den Wert @s vom Bereich @a1 - @a2 auf den Bereich @b1 - @b2 */
	private float map(float s, float a1, float a2, float b1, float b2)
	{
		return (s - a1) / (a2 - a1) * (b2 - b1) + b1;
	}
	#endregion
}
