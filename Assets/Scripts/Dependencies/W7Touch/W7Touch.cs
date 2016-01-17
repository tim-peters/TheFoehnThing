using UnityEngine;
using System.Collections;

public class W7Touch {

    private uint id;
    private Vector2 position;
    private bool updated;

    private Vector2 lastPosition;
    private Vector2 deltaPosition;

    private float lastTime;
    private float deltaTime;
    private float deleteTime;

    private TouchPhase phase;


    public uint Id {
        get { return id; }
    }

    public Vector2 DeltaPosition {
        get { return deltaPosition; }
    }

    public float DeltaTime {
        get { return deltaTime; }
    }

    public TouchPhase Phase {
        get { return phase; }
        set { phase = value; }
    }

    public Vector2 Position {
        get { return position; }
    }


    public W7Touch(uint touchId, Vector2 touchPosition) {
        this.id = touchId;
        UpdateTouch(touchPosition);
        phase = TouchPhase.Began;
        updated = true;
    }

    public void UpdateTouch(Vector2 touchPosition) {
        position = touchPosition;
        position.Scale(new Vector2(1f / (float)Screen.currentResolution.width, 1f / (float)Screen.currentResolution.height));
        position.y = 1 - position.y;
        position.Scale(new Vector2((float)Screen.width, (float)Screen.height));
        phase = TouchPhase.Moved;
        updated = true;
    }

    public void EndTouch() {
        phase = TouchPhase.Ended;
        updated = true;
    }

    public void Update() {
        deltaPosition = position - lastPosition;
        deltaTime = Time.time - lastTime;

        if (phase == TouchPhase.Moved && deltaPosition.sqrMagnitude == 0) {
            phase = TouchPhase.Stationary;
        }

        lastPosition = position;
        lastTime = Time.time;

        if (!updated) {
            deleteTime += Time.deltaTime;
            if (deleteTime > 0.5f) {
                deleteTime = 0;
                phase = TouchPhase.Canceled;
            }
        } else {
            deleteTime = 0;
        }
        updated = false;
    }
}