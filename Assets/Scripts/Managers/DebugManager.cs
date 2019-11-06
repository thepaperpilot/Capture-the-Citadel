using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private readonly KeyCode[] nums = new KeyCode[] {
        KeyCode.F3,
        KeyCode.F4,
        KeyCode.F5,
        KeyCode.F6,
        KeyCode.F7,
        KeyCode.F8,
        KeyCode.F9,
        KeyCode.F10,
        KeyCode.F11,
        KeyCode.F12
    };

    void Update() {
        DeckController deck = PlayerManager.Instance.GetDeckController();
        for (int i = 0; i < nums.Length; i++) {
            if (Input.GetKeyDown(nums[i])) {
                CardController card = deck.GetCardController(i);
                if (card != null) {
                    deck.PickupCard(card);
                    Destroy(card.gameObject);
                    
                    GameObject toy = Instantiate(card.card.toy, card.transform.position, Quaternion.identity);
                    toy.GetComponentInChildren<Toy>().card = card.card;
                    PlayerManager.Instance.Grab(toy);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            ActionsManager.Instance.AddToBottom(new EndTurnAction());
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            CombatManager.Instance.player.FillEnergy();
        }
    }
}