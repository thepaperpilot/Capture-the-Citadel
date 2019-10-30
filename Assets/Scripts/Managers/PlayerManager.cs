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
    }

    void Start()
    {
        leftSource = leftControllerSources[0];
        rightSource = rightControllerSources[0];
        leftRB = leftHand.GetComponent<Rigidbody>();
        rightRB = rightHand.GetComponent<Rigidbody>();
    }

    void Update()
    {
        leftHand.transform.position = leftSource.position;
        leftHand.transform.rotation = leftSource.rotation;
        rightHand.transform.position = rightSource.position;
        rightHand.transform.rotation = rightSource.rotation;
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
    
    public IEnumerator Draw(AbstractCard[] cardsToDraw) {
        IEnumerator[] coroutines = new IEnumerator[cardsToDraw.Count()];
        for (int i = 0; i < cardsToDraw.Count(); i++) {
            coroutines[i] = deckController.Draw(cardsToDraw[i]);
            StartCoroutine(coroutines[i]);
            yield return new WaitForSeconds(deckController.timeBetweenDraws);
        }
        while (!coroutines.Any(e => e == null))
            yield return null;
    }
}
