using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    private float slidingDuration = .5f;
    [SerializeField]
    private float slidingDistance = 1;

    [HideInInspector]
    public int deckSize = 0;

    public IEnumerator SlideOut() {
        yield return MoveTo(transform.localPosition + Vector3.back * slidingDistance);
    }
    
    public IEnumerator SlideIn() {
        yield return MoveTo(transform.localPosition + Vector3.forward * slidingDistance);
    }

    public void SetDeckSize(int numCards) {
        deckSize = numCards;
        gameObject.SetActive(numCards != 0);
        back.localPosition = new Vector3(0, 0, .02f * numCards - .01f);
        sides.localScale = new Vector3(.714f, 1, .02f * numCards - .002f);
        sides.localPosition = new Vector3(0, 0, .01f * (numCards - 1));
    }

    private IEnumerator MoveTo(Vector3 newPosition) {
        float time = 0;
        Vector3 startPosition = transform.localPosition;
        while (time < slidingDuration) {
            transform.localPosition = Vector3.Lerp(startPosition, newPosition, (time / slidingDuration));
            time += Time.deltaTime;
            yield return null;
        }
        // Ensure exact positioning at end of animation
        transform.localPosition = newPosition;
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
#endif
}
