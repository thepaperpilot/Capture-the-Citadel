using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    [BoxGroup("Components")]
    [SerializeField, ChildGameObjectsOnly]
    private GameObject drawPileRoot;
    [BoxGroup("Components")]
    [SerializeField, ChildGameObjectsOnly]
    private Transform back;
    [BoxGroup("Components")]
    [SerializeField, ChildGameObjectsOnly]
    private Transform sides;
    [BoxGroup("Components")]
    [SerializeField, ChildGameObjectsOnly]
    private Transform hand;

    [SerializeField, AssetList(Path="Prefabs", CustomFilterMethod="FindCardControllers")]
    [InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Hidden)]
    [HideInPlayMode]
    private GameObject cardPrefab;
    [SerializeField, AssetList(Path="Prefabs", CustomFilterMethod="FindDropZoneControllers")]
    [InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Hidden)]
    [HideInPlayMode]
    private GameObject dropzonePrefab;

    [FoldoutGroup("Advanced"), SerializeField]
    [Title("Deck")]
    private float slidingDuration = .5f;
    [FoldoutGroup("Advanced"), SerializeField]
    private float slidingDistance = 1;

    [FoldoutGroup("Advanced"), SerializeField]
    [Title("Hand")]
    [InfoBox("When drawing multiple cards, this is how long to wait before starting each card's animation")]
    public float timeBetweenDraws = .25f;
    [InfoBox("This is how long it takes the cards to move whenever adding or removing a card to the hand")]
    [FoldoutGroup("Advanced"), SerializeField]
    public float timeToRearrange = .25f;
    [InfoBox("This is how far away each card in the hand is from the deck")]
    [FoldoutGroup("Advanced"), SerializeField, OnValueChanged("RearrangeHand")]
    private float arcRadius = 4;
    [InfoBox("This is the maximum arc length of the hand")]
    [FoldoutGroup("Advanced"), SerializeField, OnValueChanged("RearrangeHand")]
    private float arcLength = 60;
    [InfoBox("This is the maximum distance between two cards, in degrees. The distance can be smaller as needed to fit all the cards within the arcLength")]
    [FoldoutGroup("Advanced"), SerializeField, OnValueChanged("RearrangeHand")]
    private float maxArcDistance = 60;

    private float yOffset = 0.8f;

    [HideInEditorMode, ReadOnly]
    public int deckSize = 0;

    private Vector3 cardPosition;
    private List<CardController> cardsInHand = new List<CardController>();
    private List<DropZoneController> dropzones = new List<DropZoneController>();

    public void SetupDropzones() {
        foreach (DropZoneController controller in dropzones) {
            Debug.Log(controller.gameObject);
            Destroy(controller.gameObject);
        }
        dropzones.Clear();
        foreach (RelicsManager.RelicData rData in RelicsManager.Instance.GetRelics()) {
            foreach (RelicAction action in rData.relic.actions.Where(t => t.trigger == RelicAction.Triggers.DROP_ZONE)) {
                GameObject dropzone = Instantiate(dropzonePrefab, hand);
                DropZoneController controller = dropzone.GetComponent<DropZoneController>();
                controller.Setup(action);
                dropzones.Add(controller);
            }
        }
    }

    public IEnumerator SlideOut() {
        yield return MoveTo(transform, slidingDuration, transform.localPosition + Vector3.back * slidingDistance);
        yield return RearrangeCards();
    }
    
    public IEnumerator SlideIn() {        
        int numCards = cardsInHand.Count;
        int numDropzones = dropzones.Count;
        if (numCards > 0 || numDropzones > 0) {
            IEnumerator[] coroutines = new IEnumerator[numCards + numDropzones];
            for (int i = 0; i < numCards; i++) {
                Vector3 newPosition = new Vector3(0, 0, 0);
                Quaternion newRotation = Quaternion.identity;
                coroutines[i] = MoveTo(cardsInHand[i].transform, timeToRearrange, newPosition, newRotation);
                StartCoroutine(coroutines[i]);
            }
            for (int i = 0; i < numDropzones; i++) {
                Vector3 newPosition = new Vector3(0, 0, 0);
                Quaternion newRotation = Quaternion.identity;
                coroutines[numCards + i] = MoveTo(dropzones[i].transform, timeToRearrange, newPosition, newRotation);
                StartCoroutine(coroutines[numCards + i]);
            }
            
            yield return new WaitForSeconds(timeToRearrange);
        }

        yield return MoveTo(transform, slidingDuration, transform.localPosition + Vector3.forward * slidingDistance);
    }

    // TODO add variable to set card width (both here and in CardController)
    public void SetDeckSize(int numCards) {
        deckSize = numCards;
        drawPileRoot.SetActive(numCards != 0);
        back.localPosition = new Vector3(0, 0, .02f * numCards - .01f);
        sides.localScale = new Vector3(.714f, 1, .02f * numCards - .002f);
        sides.localPosition = new Vector3(0, 0, .01f * (numCards - 1));
        cardPosition = new Vector3(0, 0, .02f * numCards);
    }

    public void Clear()
    {
        while(cardsInHand.Count > 0)
        {
            Destroy(cardsInHand[0].gameObject);
            cardsInHand.RemoveAt(0);
        }
    }

    public IEnumerator Draw(AbstractCard card, bool setDeckSize = true) {
        GameObject cardObject = Instantiate(cardPrefab, transform);
        cardObject.transform.localPosition = cardPosition;
        cardObject.transform.SetParent(hand);
        CardController controller = cardObject.GetComponent<CardController>();
        cardsInHand.Add(controller);
        controller.Setup(card);
        if (setDeckSize)
            SetDeckSize(deckSize - 1);
        yield return RearrangeCards();
    }

    public void PickupCard(CardController card) {
        cardsInHand.Remove(card);
        StartCoroutine(RearrangeCards());
    }

    public void DropCardInHand(CardController card) {
        card.transform.SetParent(hand);
        cardsInHand.Add(card);
        StartCoroutine(RearrangeCards());
    }

    private IEnumerator RearrangeCards() {
        int numCards = cardsInHand.Count;
        int numDropzones = dropzones.Count;
        if (numCards == 0 && numDropzones == 0)
            yield break;

        float cardArcDistance = Mathf.Min(maxArcDistance, arcLength / numCards) * Mathf.Deg2Rad;
        float dropzoneArcDistance = Mathf.Min(maxArcDistance, arcLength / numDropzones) * Mathf.Deg2Rad;

        IEnumerator[] coroutines = new IEnumerator[numCards + numDropzones];
        float angle = Mathf.PI / 2 - cardArcDistance * (numCards - 1) / 2;
        for (int i = 0; i < numCards; i++, angle += cardArcDistance) {
            Vector3 newPosition = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * arcRadius;
            newPosition.z += .02f * i;
            Quaternion newRotation = Quaternion.LookRotation(Vector3.forward, newPosition);
            Vector3 adjustedTarget = newPosition;
            adjustedTarget.x -= yOffset * Mathf.Sign(adjustedTarget.x);
            coroutines[i] = MoveTo(cardsInHand[i].transform, timeToRearrange, adjustedTarget, newRotation);
            StartCoroutine(coroutines[i]);
        }
        angle = Mathf.PI / 2 - dropzoneArcDistance * (numDropzones - 1) / 2;
        for (int i = 0; i < numDropzones; i++, angle += dropzoneArcDistance) {
            Vector3 newPosition = new Vector3(-Mathf.Sin(angle), Mathf.Cos(angle), 0) * arcRadius;
            newPosition.z += .02f * i;
            Quaternion newRotation = Quaternion.LookRotation(Vector3.forward, -newPosition);
            Vector3 adjustedTarget = newPosition;
            adjustedTarget.x -= yOffset * Mathf.Sign(adjustedTarget.x);
            coroutines[numCards + i] = MoveTo(dropzones[i].transform, timeToRearrange, adjustedTarget, newRotation);
            StartCoroutine(coroutines[numCards + i]);
        }

        yield return new WaitForSeconds(timeToRearrange);            
    }

    private IEnumerator MoveTo(Transform transform, float duration, Vector3 newPosition, Quaternion? newRotation = null) {
        float time = 0;
        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;
        if (newRotation == null)
            newRotation = startRotation;
        while (time < duration && transform != null) {
            if(transform.parent != hand)
            {
                yield break;
            }
            transform.localPosition = Vector3.Lerp(startPosition, newPosition, (time / duration));
            transform.localRotation = Quaternion.Lerp(startRotation, newRotation.Value, (time / duration));
            time += Time.deltaTime;
            yield return null;
        }
        // Ensure exact positioning at end of animation
        transform.localPosition = newPosition;
        transform.localRotation = newRotation.Value;
    }

    [PropertySpace, Button(ButtonSizes.Medium), HideInEditorMode]
    private void SlideDeckOut() {
        StartCoroutine(SlideOut());
    }

    [Button(ButtonSizes.Medium), HideInEditorMode]
    private void SlideDeckIn() {
        StartCoroutine(SlideIn());
    }

    private void RearrangeHand() {
        StartCoroutine(RearrangeCards());
    }

    private bool FindCardControllers(GameObject obj) {
        return obj.GetComponentInChildren<CardController>() != null;
    }

    private bool FindDropZoneControllers(GameObject obj) {
        return obj.GetComponentInChildren<DropZoneController>() != null;
    }

    public CardController GetCardController(int index) {
        if (index >= cardsInHand.Count)
            return null;

        return cardsInHand[index];
    }
}
