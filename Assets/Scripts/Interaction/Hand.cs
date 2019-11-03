using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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
    Transform heldObjectParent = null;

    [SerializeField] Transform grabTarget;
    [SerializeField] Transform pinchTarget;
    [ShowIf("id", HandID.RIGHT)]
    [SerializeField] Transform cardHolder;
    [SerializeField] float grabRadius = 0.05f;
    [SerializeField] float pinchRadius = 0.02f;

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

    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(grabTarget.position, grabRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pinchTarget.position, pinchRadius);
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
                if (gripPressed || heldObject == null || heldObject.transform.parent != transform)
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
                if (!triggerPressed)
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
                else if (gripPressed) {
                    Transform();
                    ChangeToState(HandAnimPose.OPEN);
                }
                break;
        }
    }

    public void TriggerButtonChanged(bool pressed)
    {
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
                heldObjectParent = heldObject.transform.parent;
                collider.transform.SetParent(transform);
                heldObject.transform.localPosition = grabTarget.localPosition;
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
                heldObjectParent = collider.transform.parent;
                collider.transform.SetParent(cardHolder);
                collider.transform.localPosition = Vector3.zero;
                collider.transform.localRotation = Quaternion.identity;
                PlayerManager.Instance.RemoveCard(heldObject.GetComponent<CardController>());
            }
        }
    }

    void Transform() {
        CardController card = heldObject.GetComponent<CardController>();
        if (CombatManager.Instance.player.energy < card.card.energyCost) {
            // TODO "Failed" sound effect or something
            return;
        }
        CombatManager.Instance.player.energy -= card.card.energyCost;
        GameObject toy = Instantiate(card.card.toy, card.transform.position, Quaternion.identity);
        toy.GetComponentInChildren<Toy>().card = card.card;
        Destroy(heldObject);
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
            heldObject.transform.SetParent(heldObjectParent);
            if (heldObject.CompareTag("Card"))
                PlayerManager.Instance.AddCard(heldObject.GetComponent<CardController>());
            heldObject = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PointingZone"))
        {
            inPointArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PointingZone"))
        {
            inPointArea = false;
        }
    }

#if UNITY_EDITOR
    // Used by debug manager
    public void Grab(GameObject gameObject) {
        Release();
        heldObject = gameObject;
        heldObjectParent = heldObject.transform.parent;
        gameObject.transform.SetParent(transform);
        heldObject.transform.localPosition = grabTarget.localPosition;
        heldObject.transform.localRotation = Quaternion.identity;
        Rigidbody childRB = heldObject.GetComponent<Rigidbody>();
        if(childRB != null)
        {
            childRB.isKinematic = true;
        }
        ChangeToState(HandAnimPose.HOLDING);
    }
#endif
}
