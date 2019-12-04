using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class CardBuyerController : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly]
    private CardController card;
    [SerializeField, ChildGameObjectsOnly]
    private TextMeshPro costText;

    private AbstractCard abstractCard;
    private ShopScreenController controller;
    private int cost;

    void Update()
    {
        transform.localRotation = Quaternion.LookRotation(transform.position - PlayerManager.Instance.GetHeadsetPos());
    }

    public void Setup(AbstractCard abstractCard, ShopScreenController controller) {
        card.Setup(abstractCard, true);
        this.abstractCard = abstractCard;
        this.controller = controller;
        Vector2Int goldRange =
            abstractCard.rarity == AbstractCard.Rarities.COMMON ? controller.commonCost :
            abstractCard.rarity == AbstractCard.Rarities.UNCOMMON ? controller.uncommonCost :
            abstractCard.rarity == AbstractCard.Rarities.RARE ? controller.rareCost : Vector2Int.zero;
        costText.text = "" + (cost = Random.Range(goldRange.x, goldRange.y));
    }

    public void Buy() {
        if (controller.Buy(abstractCard, cost))
            Destroy(gameObject);
    }
}
