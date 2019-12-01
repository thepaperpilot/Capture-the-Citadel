using Sirenix.OdinInspector;
using UnityEngine;

public class CardRewardController : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly]
    private CardController card;

    private AbstractCard abstractCard;
    private RewardsScreenController controller;

    void Update()
    {
        transform.localRotation = Quaternion.LookRotation(transform.position - PlayerManager.Instance.GetHeadsetPos());
    }

    public void Setup(AbstractCard abstractCard, RewardsScreenController controller) {
        card.Setup(abstractCard, false);
        this.abstractCard = abstractCard;
        this.controller = controller;
    }

    public void Choose() {
        controller.Choose(abstractCard);
    }
}
