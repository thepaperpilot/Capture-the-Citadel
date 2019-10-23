using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public enum HandAnimPose
    {
        OPEN = 1,
        CLOSED,
        HOLDING,
        POINT,
        PINCH
    }

    public enum HandID
    {
        LEFT,
        RIGHT
    }

    HandAnimPose state = HandAnimPose.OPEN;
    Animator anim;
    [SerializeField] private HandID id;
    string triggerAxis;
    string gripAxis;

    bool triggerPressed;
    bool gripPressed;
    bool inPointArea;

    GameObject heldObject = null;

    [SerializeField] Transform grabTarget;
    [SerializeField] Transform pinchTarget;
    [SerializeField] float grabRadius = 0.2f;
    [SerializeField] float pinchRadius = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        if (id == HandID.RIGHT)
        {
            triggerAxis = "VRTK_Axis10_RightTrigger";
            gripAxis = "VRTK_Axis12_RightGrip";
        }
        else
        {
            triggerAxis = "VRTK_Axis9_LeftTrigger";
            gripAxis = "VRTK_Axis11_LeftGrip";
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        switch (state)
        {
            case HandAnimPose.OPEN:
                if (triggerPressed)
                {
                    if (Grab())
                    {
                        ChangeToState(HandAnimPose.HOLDING);
                    }
                    else
                    {
                        ChangeToState(HandAnimPose.CLOSED);
                    }
                }
                else if (inPointArea)
                {
                    ChangeToState(HandAnimPose.POINT);
                }
                break;
            case HandAnimPose.CLOSED:
                if (!triggerPressed)
                {
                    if (inPointArea)
                    {
                        ChangeToState(HandAnimPose.POINT);
                    }
                    else
                    {
                        ChangeToState(HandAnimPose.OPEN);
                    }
                }
                break;
            case HandAnimPose.HOLDING:
                if (gripPressed || heldObject == null)
                {
                    Release();
                    if (inPointArea)
                    {
                        ChangeToState(HandAnimPose.POINT);
                    }
                    else
                    {
                        ChangeToState(HandAnimPose.OPEN);
                    }
                }
                break;
            case HandAnimPose.POINT:
                if (triggerPressed)
                {
                    Pinch();
                    ChangeToState(HandAnimPose.PINCH);
                }
                else if (!inPointArea)
                {
                    ChangeToState(HandAnimPose.OPEN);
                }
                break;
            case HandAnimPose.PINCH:
                if (!triggerPressed || heldObject == null)
                {
                    Release();
                    if (inPointArea)
                    {
                        ChangeToState(HandAnimPose.POINT);
                    }
                    else
                    {
                        ChangeToState(HandAnimPose.OPEN);
                    }
                }
                break;
        }
    }

    public void TriggerButtonChanged(bool pressed)
    {
        Debug.Log("Pressed " + triggerAxis);
        triggerPressed = pressed;
    }

    public void GripButtonChanged(bool pressed)
    {
        gripPressed = pressed;
    }

    void ChangeToState(HandAnimPose toState)
    {
        state = toState;
        anim.SetInteger("state", (int)toState);
    }

    bool Grab()
    {
        Collider[] hit = Physics.OverlapSphere(grabTarget.position, grabRadius);

        foreach (Collider collider in hit)
        {
            if (collider.CompareTag("Grabbable"))
            {
                Release();
                heldObject = collider.gameObject;
                collider.transform.SetParent(transform);
                heldObject.transform.localPosition = Vector3.zero;
                heldObject.transform.localRotation = Quaternion.identity;
                Rigidbody childRB = heldObject.GetComponent<Rigidbody>();
                if(childRB != null)
                {
                    childRB.isKinematic = true;
                }
                return true;
            }
        }
        return false;
    }

    void Pinch()
    {
        Collider[] hit = Physics.OverlapSphere(pinchTarget.position, pinchRadius);

        foreach (Collider collider in hit)
        {
            if (collider.CompareTag("Card"))
            {
                Release();
                heldObject = collider.gameObject;
                collider.transform.SetParent(transform);
            }
        }
    }

    void Release()
    {
        if(heldObject != null)
        {
            Rigidbody childRB = heldObject.GetComponent<Rigidbody>();
            if (childRB != null)
            {
                childRB.isKinematic = true;
            }
            heldObject.transform.SetParent(null);
            heldObject = null;
        }
    }
}
