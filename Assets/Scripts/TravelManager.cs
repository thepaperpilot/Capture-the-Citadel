using UnityEngine;

public class TravelManager : MonoBehaviour {
	enum STATE {
		STATIONARY,
		FORWARD,
		FORWARD2X,
		REVERSE,
		REVERSE2X
	}

	private STATE state;

	private void Awake() {
		state = STATE.STATIONARY;
	}

	private void Update() {
		Debug.Log(state);
	}

	// Bound to Right Circle Pad in OpenVR Controller
	// Bound to Button 2 in the Simulated Controller (right click button)
	public void TravelButtonChanged(bool activated) {
		state = activated ? STATE.FORWARD : STATE.STATIONARY;
	}

	// Bound to Right Trigger in OpenVR Controller
	// Bound to Button 3 in the Simulated Controller (middle mouse button)
	public void SprintButtonChanged(bool activated) {
		if (state == STATE.STATIONARY) return;
		if (activated) {
			state = state == STATE.REVERSE ? STATE.REVERSE2X : STATE.FORWARD2X;
		} else {
			state = state == STATE.REVERSE2X ? STATE.REVERSE : STATE.FORWARD;
		}
	}

	// Bound to Left Trigger in OpenVR Controller
	// Bound to Button 1 in the Simulated Controller (left mouse button)
	public void ReverseButtonChanged(bool activated) {
		if (state == STATE.STATIONARY) return;
		if (activated) {
			state = state == STATE.FORWARD ? STATE.REVERSE : STATE.REVERSE2X;
		} else {
			state = state == STATE.REVERSE ? STATE.FORWARD : STATE.FORWARD2X;
		}
	}
}
