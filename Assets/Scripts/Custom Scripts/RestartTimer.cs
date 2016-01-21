using UnityEngine;
using System.Collections;

public class RestartTimer : MonoBehaviour {

	public UISlider tempSlider, satSlider, indikatorSlider;
	public UIButton mountainButton;

	private float timeUntilShowmode = 60f;
	private MainCameraHandler mainCameraHandler;
	private ChangePiece changePiece;
	private float lastInputTime;
	private float indikatorTime = 15f;
	private float[] indikatorSteps = new float[]{0.1f,0.4f,0.6f,0.9f};
	private int indikatorCounter = 0;

	void Start () {
		mainCameraHandler = gameObject.GetComponent<MainCameraHandler>();
		changePiece = gameObject.GetComponent<ChangePiece>();

		lastInputTime = Time.realtimeSinceStartup;
	}

	void LateUpdate () {
		if(Input.GetMouseButton(0) && !mainCameraHandler.showMode)
			lastInputTime = Time.realtimeSinceStartup;

		if(Time.realtimeSinceStartup-lastInputTime >= timeUntilShowmode)
			resetAndRestart();

		if(mainCameraHandler.showMode) {
			if(indikatorTime >= 15.0f) {
				changeIndikatorValue();
				indikatorTime = 0f;
			} else
				indikatorTime += Time.deltaTime;
		}
	}

	private void resetAndRestart() {
		tempSlider.value = 0.6f;
		satSlider.value = 0.6f;
		mountainButton.SendMessage("OnClick");
		mainCameraHandler.showMode = true;
		lastInputTime = Time.realtimeSinceStartup;
	}

	private void changeIndikatorValue() {
		Hashtable param = new Hashtable();
		param.Add("from", indikatorSlider.value);
		param.Add("to", indikatorSteps[indikatorCounter]);
		param.Add("time", 1.2f);
		param.Add("onupdate", "setIndikatorValue");
		param.Add ("easetype", iTween.EaseType.easeInOutCubic);
		iTween.ValueTo(gameObject, param);
		indikatorCounter = (indikatorCounter<indikatorSteps.Length-1) ? indikatorCounter+1 : 0;
	}

	private void setIndikatorValue(float val) {
		indikatorSlider.value = val;
	}
}
