/*
 * @className		extendUILabel
 * @project			TheFoehnThing
 * @lastModified	2014-07-14
 * @author 			Kai Zwier <kai.p.zwier@stud.h-da.de>
 */

using UnityEngine;
using System.Collections;

public class extendUILabel : MonoBehaviour {
	globalUIHandler ui;
	ChangePiece changePiece; 

	public float minTemp = -5f;
	public float maxTemp = 25f;
	public int sliderType = 1; // 1 = temperature; 2 = humidity/saturation; 3 = mountain; 4 = Indikator
	private GameObject[] infoTexte = new GameObject[4];
	private UILabel level;
	private UILabel headline;
	private string newText;
	private float[] newValues;

	void Start(){
		ui = GameObject.Find ("UI Root").GetComponent<globalUIHandler> ();
		changePiece = GameObject.Find ("Main Camera").GetComponent<ChangePiece> ();
		infoTexte[0] = GameObject.Find ("UI Root/UI Components/Infowindow/1_text");
		infoTexte[1] = GameObject.Find ("UI Root/UI Components/Infowindow/2_text");
		infoTexte[2] = GameObject.Find ("UI Root/UI Components/Infowindow/3_text");
		infoTexte[3] = GameObject.Find ("UI Root/UI Components/Infowindow/4_text");
		level = GameObject.Find ("UI Root/UI Components/Infowindow/level").GetComponent<UILabel>();
		headline = GameObject.Find ("UI Root/UI Components/Infowindow/title").GetComponent<UILabel>();
	}

	/*
		Temperature Slider OnChangeValue
		- remap of slider values
		- updating particles temperature
		- updating label
		- indicate new foehn layer
	*/
	public void SetCurrentTemperatur ()
	{
		if (UIProgressBar.current != null) {
			float newValue = Remap(UIProgressBar.current.value, 0f, 1f, minTemp, maxTemp );
			GameObject.Find("Main Camera/Wind Particle System").GetComponent<AllWindParticles>().setTemperature(newValue);
			gameObject.GetComponent<UILabel>().text = newValue.ToString("F1") + "°C";
			changePiece.setNewFoehnIndikator();
		}
	}

	/*
		Saturation Slider OnChangeValue
		- remap of slider values
		- updating particles saturation
		- updating label
		- indicate new foehn layer
	*/
	public void SetCurrentSaturation()
	{
		if (UIProgressBar.current != null) {
			float newValue = Remap(UIProgressBar.current.value, 0f, 1f, 0, 100 );
			GameObject.Find("Main Camera/Wind Particle System").GetComponent<AllWindParticles>().setSaturation(newValue);
			gameObject.GetComponent<UILabel>().text = newValue.ToString("F1") + "%";
			changePiece.setNewFoehnIndikator();
		}
	}	

	/*
		Compass Button OnClick
		- call start function of compass mode
	*/	
	public void setCompassMode()
	{
		CompassModel compassModel = GameObject.Find ("Compass").GetComponent<CompassModel> ();
		ChangePiece changePiece = GameObject.Find ("Main Camera").GetComponent<ChangePiece> ();

		compassModel.startCompassMode (changePiece.currentPiece);

	}

	/*
		Indikator Slider OnValueChange
		- getting slider value
		- detecting current step
		- update contents in the infowindow
	*/
	public void SetCurrentIndikatorStep() {
		if (UIProgressBar.current != null) {
			float newValue = (float)UIProgressBar.current.value;
			GameObject.Find ("IndikatorLayer").GetComponent<IndikatorLayer>().SetNewPositionValue(UIProgressBar.current.value);
			
			FoehnIndikator foehnIndikator = GameObject.Find("Main Camera").GetComponent<ChangePiece>().foehnIndikator;
			string newHeadline;
			int levelId = 1;
			if(newValue >= 0 && newValue <= 0.25f){
				infoTexte[0].SetActive(true);
				infoTexte[1].SetActive(false);
				infoTexte[2].SetActive(false);
				infoTexte[3].SetActive(false);
				newHeadline = "Entstehung von Föhn-Winden";
			} else if(newValue > 0.25f && newValue <= 0.5f){
				infoTexte[0].SetActive(false);
				infoTexte[1].SetActive(true);
				infoTexte[2].SetActive(false);
				infoTexte[3].SetActive(false);
				newHeadline = "Feuchtadiabatischer Aufstieg";
				levelId = 2;
			} else if(newValue > 0.5 && newValue <= 0.75){
				infoTexte[0].SetActive(false);
				infoTexte[1].SetActive(false);
				infoTexte[2].SetActive(true);
				infoTexte[3].SetActive(false);
				newHeadline = "Trockenadiabatischer Abstieg";
				levelId = 3;
			} else {
				infoTexte[0].SetActive(false);
				infoTexte[1].SetActive(false);
				infoTexte[2].SetActive(false);
				infoTexte[3].SetActive(true);
				newHeadline = "Föhn-Effekt";
				levelId = 4;
			}

			headline.text = newHeadline;
			level.text = "STUFE " + levelId;
		
			if(foehnIndikator != null){
				ui.setInfowindowValues(foehnIndikator.getValues(newValue));
			}
		}
	}
	
	public void updateParticles ()
	{
		if (UIProgressBar.current != null) {
			float temperature = Remap(UIProgressBar.current.value, 0f, 1f, minTemp, maxTemp );
			Debug.Log("setting new temperature and resetting");
		}
	}


	public float Remap (float value, float leftMin, float leftMax, float rightMin, float rightMax){
		// Figure out how 'wide' each range is
		float leftSpan = leftMax - leftMin;
		float rightSpan = rightMax - rightMin;
		
		// Convert the left range into a 0-1 range (float)
		float valueScaled = (value - leftMin) / leftSpan;
		
		// Convert the 0-1 range into a value in the right range.
		return rightMin + (valueScaled * rightSpan);
	}
}
