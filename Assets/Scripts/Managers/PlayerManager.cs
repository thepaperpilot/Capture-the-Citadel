using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject playAreaAlias;
    [SerializeField] private GameObject headsetAlias;
    [SerializeField] private DeckController deckController;
    [SerializeField] private List<Transform> leftControllerSources;
    [SerializeField] private List<Transform> rightControllerSources;
    private Hand right;
    private Hand left;
    private Transform leftSource;
    private Transform rightSource;
    Rigidbody leftRB;
    Rigidbody rightRB;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(this);
        }

        right = rightHand.GetComponentInChildren<Hand>();
        left = leftHand.GetComponentInChildren<Hand>();
    }

    void Start()
    {
#if UNITY_EDITOR
        leftSource = leftControllerSources[0];
        rightSource = rightControllerSources[0];
#else
        leftSource = leftControllerSources[1];
        rightSource = rightControllerSources[1];
#endif
        leftRB = leftHand.GetComponent<Rigidbody>();
        rightRB = rightHand.GetComponent<Rigidbody>();
    }

    void Update()
    {
        leftHand.transform.position = leftSource.position;
        leftHand.transform.rotation = leftSource.rotation;
        rightHand.transform.position = rightSource.position;
        rightHand.transform.rotation = rightSource.rotation;

        if (right.state == Hand.HandAnimPose.CLOSED &&
            left.state == Hand.HandAnimPose.CLOSED &&
            CombatManager.Instance.IsPlayerTurn())
            ActionsManager.Instance.AddToBottom(new EndTurnAction());
    }

    private void FixedUpdate()
    {
        //Move Hands
        Vector3 leftDelta = leftSource.position - leftHand.transform.position;
        //leftRB.AddForce(leftDelta, ForceMode.Acceleration);
        leftHand.transform.position = leftSource.position;
        Vector3 rightDelta = rightSource.position - rightHand.transform.position;
        rightHand.transform.position = rightSource.position;
        //rightRB.AddForce(rightDelta, ForceMode.Acceleration);
    }

    public void MovePlayer(Vector3 position, Quaternion rotation) {
        playAreaAlias.transform.SetPositionAndRotation(position, rotation);
    }

    public void SetupDropzones() {
        deckController.SetupDropzones();
    }

    public IEnumerator StartTurn() {
        deckController.SetDeckSize(CardsManager.Instance.drawPile.Count);
        yield return deckController.SlideOut();
    }

    public IEnumerator EndTurn() {
        yield return deckController.SlideIn();
    }
    
    public IEnumerator Draw(AbstractCard[] cardsToDraw, bool setDeckSize = true) {
        for (int i = 0; i < cardsToDraw.Count(); i++) {
            StartCoroutine(deckController.Draw(cardsToDraw[i], setDeckSize));
            yield return new WaitForSeconds(deckController.timeBetweenDraws);
        }
        yield return new WaitForSeconds(deckController.timeToRearrange);
    }

    public void AddCard(CardController card) {
        deckController.DropCardInHand(card);
    }

    public void RemoveCard(CardController card) {
        deckController.PickupCard(card);
    }

#if UNITY_EDITOR
    // These are used by DebugManager
    public DeckController GetDeckController() {
        return deckController;
    }

    public void Grab(GameObject gameObject) {
        right.Grab(gameObject);
    }
#endif
}
