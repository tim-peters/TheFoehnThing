/*
 * @className		globalUIHandler
 * @project			TheFoehnThing
 * @lastModified	2014-07-14
 * @author 			Kai Zwier <kai.p.zwier@stud.h-da.de>
 */

using UnityEngine;
using System.Collections;

public class globalUIHandler : MonoBehaviour {

	public GameObject[] uiElements = new GameObject[6];
	public UILabel[] infowindowLabels = new UILabel[3];
	public GameObject uiComponents;
	private UILabel myElement;
	private string[] orientationLabels = {"N", "NNE", "NE", "NEE", "E", "SEE", "SE", "SSE", "S", "SSW", "SW", "SWW", "W", "NWW", "NW", "NNW"};
	UISprite uiComponentSprite;

	void Start(){
		uiComponentSprite = GameObject.Find ("UI Components").GetComponent<UISprite> ();
		myElement = GameObject.Find ("UI Root/UI Components/Compass Container/Value").GetComponent<UILabel> ();
		redefineBoxColliders ();
	}
	
	public void fadeOut() {
		uiComponents.GetComponent<TweenColor> ().Play ();
	}
	
	public void fadeIn() {
		uiComponents.GetComponent<TweenColor> ().PlayReverse ();
	}

	public void setTemperature(float newTemperature){
		//uiElements[1].Find("Value").
	}

	public void setOrientation(int degreeStep){
		myElement.text = orientationLabels[degreeStep];
	}

	/*
		redefine sliders boxcollider
		- colliders are set to it's minimum by default
		- need to be bigger for preventing camera rotation while dragging the sliders
	*/
	public void redefineBoxColliders(){
		
		// temperature
		uiElements [0].transform.Find ("Slider Wrapper/Control - Colored Slider").GetComponent<BoxCollider> ().size = new Vector3 (170, 60, 0);
		uiElements [0].transform.Find ("Slider Wrapper/Control - Colored Slider").GetComponent<BoxCollider> ().center = new Vector3 (0, 1.4f, 0);
		
		// saturation
		uiElements [1].transform.Find ("Slider Wrapper/Control - Colored Slider").GetComponent<BoxCollider> ().size = new Vector3 (170, 60, 0);
		uiElements [1].transform.Find ("Slider Wrapper/Control - Colored Slider").GetComponent<BoxCollider> ().center = new Vector3 (0, 1.4f, 0);
		
		// indikator?
		uiElements [4].transform.Find ("Slider Wrapper/Control - Colored Slider").GetComponent<BoxCollider> ().size = new Vector3 (1980, 80, 0);
	}
	
	public void checkInfowindowValues(){
		FoehnIndikator foehnIndikator = GameObject.Find ("Main Camera").GetComponent<ChangePiece> ().foehnIndikator;
		if (foehnIndikator != null) {
			float newValue = (float)uiElements [4].transform.Find ("Slider Wrapper/Control - Colored Slider").GetComponent<UISlider>().value;
			//Debug.Log (Remap(uiElements [4].transform.Find ("Slider Wrapper/Control - Colored Slider").GetComponent<UISlider>().value, 0f, 1f, 1, 100 ).ToString("F0"));
			setInfowindowValues (foehnIndikator.getValues(newValue));
		}
	}

	public void setInfowindowValues(float[] newValues){
		infowindowLabels [0].text = newValues[0].ToString ("F0") + " m";
		infowindowLabels [1].text = newValues[1].ToString ("F1") + " °C";
		infowindowLabels [2].text = newValues[2].ToString ("F2") + "%";
		//return new float[]{heights[step],temperatures[step],saturations[step]};
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
