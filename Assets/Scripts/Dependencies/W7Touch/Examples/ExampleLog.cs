using UnityEngine;
using System.Collections;

public class ExampleLog : MonoBehaviour {

    void OnHover(bool isOver) {
        Debug.Log("OnHover " + isOver);
    }

    void OnPress(bool pressed) {
        Debug.Log("OnPress " + pressed);
    }

    void OnClick() {
        Debug.Log("OnClick ");
    }

    void OnDoubleClick() {
        Debug.Log("OnDoubleClick ");
    }

    void OnSelect(bool selected) {
        Debug.Log("OnSelect " + selected);
    }

    void OnDrag(Vector2 delta) {
        Debug.Log("OnDrag " + delta);
    }

    void OnDrop(GameObject go) {
        Debug.Log("OnDrop " + go.name);
    }

    void OnInput(string text) {
        Debug.Log("OnInput " + text);
    }

    void OnSubmit() {
        Debug.Log("OnSubmit ");
    }

    void OnScroll(float delta) {
        Debug.Log("OnScroll " + delta);
    }
}
