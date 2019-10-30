using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(DeckController))]
public class PlayerController : CombatantController
{
    [SceneObjectsOnly]
    public Transform playArea;

    private DeckController deckController;

    private void Awake() {
        deckController = GetComponent<DeckController>();
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
