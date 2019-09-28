using System.Collections.Generic;
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
    private Rigidbody myRB;
    [SerializeField] private GameObject playAreaAlias;
    [SerializeField] private GameObject headsetAlias;
    [SerializeField] private List<Transform> rightControllerSources;
    [SerializeField] private float deadzone;
	[SerializeField] private float speed = 10;

    private void Awake() {
		state = STATE.STATIONARY;
        myRB = playAreaAlias.GetComponent<Rigidbody>();
	}

	private void Update() {
        Debug.Log(state);
		if(state == STATE.STATIONARY)
        {
            myRB.velocity = Vector3.zero;
        }
        else
        {
            float multiplier;
            switch (state)
            {
                case STATE.FORWARD:
                    multiplier = 1;
                    break;
                case STATE.FORWARD2X:
                    multiplier = 2;
                    break;
                case STATE.REVERSE:
                    multiplier = -1;
                    break;
                case STATE.REVERSE2X:
                    multiplier = -2;
                    break;
                default:
                    multiplier = 1;
                    break;
            }
            Vector3 handPos = Vector3.zero;
            foreach(Transform tf in rightControllerSources)
            {
                if(tf.position == Vector3.zero)
                {
                    continue;
                }
                handPos = tf.position;
            }
            Vector3 controllerOffset = Vector3.ProjectOnPlane(handPos - headsetAlias.transform.position, Vector3.up);
            Debug.Log(controllerOffset.magnitude);
            if(controllerOffset.magnitude > deadzone)
            {
                myRB.velocity = multiplier * speed * controllerOffset;
            }
            else
            {
                myRB.velocity = Vector3.zero;
            }
        }
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
