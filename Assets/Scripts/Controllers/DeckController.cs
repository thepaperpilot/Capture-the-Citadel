using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    [BoxGroup("Components")]
    [SerializeField, ChildGameObjectsOnly]
    private Transform back;
    [BoxGroup("Components")]
    [SerializeField, ChildGameObjectsOnly]
    private Transform sides;

    [SerializeField, AssetsOnly, HideInPlayMode]
    private GameObject cardPrefab;
    [InfoBox("This should probably be a sibling. Definitely don't make it a child")]
    [SerializeField, SceneObjectsOnly, PropertySpace(0, 10), HideInPlayMode]
    private Transform hand;

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
    private float timeToRearrange = .25f;
    [InfoBox("This is how far away each card in the hand is from the deck")]
    [FoldoutGroup("Advanced"), SerializeField, OnValueChanged("RearrangeHand")]
    private float arcRadius = 4;
    [InfoBox("This is the maximum arc length of the hand")]
    [FoldoutGroup("Advanced"), SerializeField, OnValueChanged("RearrangeHand")]
    private float arcLength = 60;
    [InfoBox("This is the maximum distance between two cards, in degrees. The distance can be smaller as needed to fit all the cards within the arcLength")]
    [FoldoutGroup("Advanced"), SerializeField, OnValueChanged("RearrangeHand")]
    private float maxArcDistance = 60;

    [HideInEditorMode, ReadOnly]
    public int deckSize = 0;

    private Vector3 cardPosition;
    private List<CardController> cardsInHand = new List<CardController>();

    public IEnumerator SlideOut() {
        yield return MoveTo(transform, slidingDuration, transform.localPosition + Vector3.back * slidingDistance);
    }
    
    public IEnumerator SlideIn() {
        yield return MoveTo(transform, slidingDuration, transform.localPosition + Vector3.forward * slidingDistance);
    }

    public void SetDeckSize(int numCards) {
        deckSize = numCards;
        gameObject.SetActive(numCards != 0);
        back.localPosition = new Vector3(0, 0, .02f * numCards - .01f);
        sides.localScale = new Vector3(.714f, 1, .02f * numCards - .002f);
        sides.localPosition = new Vector3(0, 0, .01f * (numCards - 1));
        cardPosition = new Vector3(0, 0, .02f * numCards);
    }

    public IEnumerator Draw(AbstractCard card) {
        GameObject cardObject = Instantiate(cardPrefab, transform);
        cardObject.transform.localPosition = cardPosition;
        cardObject.transform.SetParent(hand);
        CardController controller = cardObject.GetComponent<CardController>();
        cardsInHand.Add(controller);
        controller.Setup(card);
        SetDeckSize(deckSize - 1);
        yield return RearrangeCards();
    }

    public void PickupCard(CardController card) {
        cardsInHand.Remove(card);
        StartCoroutine(RearrangeCards());
    }

    private IEnumerator RearrangeCards() {
        int numCards = cardsInHand.Count;
        float arcDistance = Mathf.Min(maxArcDistance, arcLength / numCards) * Mathf.Deg2Rad;

        IEnumerator[] coroutines = new IEnumerator[numCards];
        float angle = Mathf.PI / 2 - arcDistance * (numCards - 1) / 2;
        for (int i = 0; i < numCards; i++, angle += arcDistance) {
            Vector3 newPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * arcRadius;
            newPosition.z += .02f * i;
            Quaternion newRotation = Quaternion.LookRotation(-hand.right, newPosition);
            coroutines[i] = MoveTo(cardsInHand[i].transform, timeToRearrange, newPosition, newRotation);
            StartCoroutine(coroutines[i]);
        }
        
        while (!coroutines.Any(e => e == null))
            yield return null;
    }

    private IEnumerator MoveTo(Transform transform, float duration, Vector3 newPosition, Quaternion? newRotation = null) {
        float time = 0;
        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;
        if (newRotation == null)
            newRotation = startRotation;
        while (time < duration) {
            transform.localPosition = Vector3.Lerp(startPosition, newPosition, (time / duration));
            transform.localRotation = Quaternion.Lerp(startRotation, newRotation.Value, (time / duration));
            time += Time.deltaTime;
            yield return null;
        }
        // Ensure exact positioning at end of animation
        transform.localPosition = newPosition;
        transform.localRotation = newRotation.Value;
    }

#if UNITY_EDITOR
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
#endif
}
