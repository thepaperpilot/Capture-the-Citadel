using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zinnia.Action;

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

    public HandAnimPose state = HandAnimPose.OPEN;
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
    //[ShowIf("id", HandID.RIGHT)]
    [SerializeField] Transform cardHolder;
    [SerializeField] float grabRadius = 0.05f;
    [SerializeField] float pinchRadius = 0.04f;
    
    public BooleanAction trigger;
    public BooleanAction grip;

    public SkinnedMeshRenderer handRenderer;
    public Material noMat;
    public Material gauntletMat;
    public Material gloveMat;

    public LayerMask handLayer;

    public GameObject debugPinchRender;
    public bool showDebugPinch;

    void Awake() {
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        debugPinchRender.SetActive(false);
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
        inPointArea = CheckPointArea();
        if (showDebugPinch)
        {
            debugPinchRender.SetActive(inPointArea);
        }
        UpdateState();
    }

    public void SetTheme(AbstractCard.ClassColor color)
    {
        switch (color)
        {
            case AbstractCard.ClassColor.COLORLESS:
                handRenderer.material = noMat;
                break;
            case AbstractCard.ClassColor.RED:
                handRenderer.material = gauntletMat;
                break;
            case AbstractCard.ClassColor.GREEN:
                handRenderer.material = gloveMat;
                break;
        }
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
                    if (Release(true)) {
                        if (inPointArea)
                        {
                            ChangeToState(HandAnimPose.POINT);
                        }
                        else
                        {
                            ChangeToState(HandAnimPose.OPEN);
                        }
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
                        ChangeToState(HandAnimPose.HOLDING);
                    }
                }
                else if (gripPressed) {
                    PlayCard(heldObject);
                    //ChangeToState(HandAnimPose.OPEN);
                }
                break;
        }
    }

    public void TriggerButtonChanged(bool pressed)
    {
        triggerPressed = pressed;
        trigger.Receive(pressed);
    }

    public void GripButtonChanged(bool pressed)
    {
        gripPressed = pressed;
        grip.Receive(pressed);
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
                if(heldObject != null)
                {
                    Release();
                }
                heldObject = collider.gameObject;
                //heldObjectParent = collider.transform.parent;
                collider.transform.SetParent(cardHolder);
                collider.transform.localPosition = Vector3.zero;
                collider.transform.localRotation = Quaternion.identity;
                PlayerManager.Instance.GetDeckController().PickupCard(heldObject.GetComponent<CardController>());
                return;
            }
            else if (collider.CompareTag("Pawn"))
            {
                if (!collider.GetComponent<MapPawnController>().locked)
                {
                    if (heldObject != null)
                    {
                        Release();
                    }
                    heldObject = collider.gameObject;
                    collider.transform.SetParent(cardHolder);
                }
                return;
            }
        }
    }

    void PlayCard(GameObject cardObj) {
        if (cardObj == null)
            return;
        CardController card = cardObj.GetComponentInChildren<CardController>();
        if (card == null)
        {
            Destroy(cardObj);
            heldObject = null;
            return;
        } 
        if (CombatManager.Instance.player.Energy < card.card.energyCost) {
            // TODO "Failed" sound effect or something
            PlayerManager.Instance.AddCard(card);
            heldObject = null;
        }
        else
        {
            CombatManager.Instance.player.SpendEnergy(card.card.energyCost);
            Destroy(cardObj);
            GameObject toy = Instantiate(card.card.toy, card.transform.position, Quaternion.identity);
            toy.GetComponentInChildren<Toy>().card = card.card;
            heldObject = null;
            PlayerManager.Instance.Grab(toy);
        }
        
    }

    bool Release(bool checkToy = false)
    {
        if(heldObject != null)
        {
            if (heldObject.CompareTag("Card"))
            {
                if (inPointArea)
                {
                    Debug.Log("released in point");
                    PlayerManager.Instance.AddCard(heldObject.GetComponent<CardController>());
                    heldObject = null;
                }
                else
                {
                    PlayCard(heldObject);
                }
                return true;
            }
            else if (heldObject.CompareTag("Pawn"))
            {
                heldObject.transform.SetParent(null);
                MapPawnController pawn = heldObject.GetComponent<MapPawnController>();
                heldObject = null;
                pawn.Drop();
                return true;
            }
            else
            {
                Toy toy = heldObject.GetComponentInChildren<Toy>();
                if (toy != null)
                {
                    if (!checkToy || toy.CanBeDropped()) {
                        toy.Refund();
                        heldObject = null;
                        if (toy.card) {
                            CombatManager.Instance.player.SpendEnergy(-toy.card.energyCost);
                            StartCoroutine(PlayerManager.Instance.Draw(new AbstractCard[] { toy.card }));
                        }
                    } else return false;
                }
            }
            /*
            Rigidbody childRB = heldObject.GetComponent<Rigidbody>();
            if (childRB != null)
            {
                childRB.isKinematic = true;
            }
            heldObject.transform.SetParent(heldObjectParent);
            heldObject = null;
            */
        }
        return true;
    }

    private bool CheckPointArea()
    {
        Collider[] hits = Physics.OverlapSphere(cardHolder.position, 0.1f, handLayer, QueryTriggerInteraction.Collide);
        foreach(Collider hit in hits)
        {
            if (hit.CompareTag("PointingZone"))
            {
                return true;
            }
        }
        return false;
    }

    public void Grab(GameObject gameObject) {
        if(heldObject != null)
        {
            Release();
        }
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

    public bool GetTriggerPressed()
    {
        return triggerPressed;
    }

}
