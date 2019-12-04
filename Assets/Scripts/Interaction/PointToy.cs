using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRTK.Prefabs.Pointers;
using Zinnia.Action;
using Zinnia.Rule;
using static Zinnia.Pointer.ObjectPointer;

[RequireComponent(typeof(BooleanAction))]
public class PointToy : Toy, IRule
{
    [System.Flags]
    public enum PointTargets {
        None = 0,
        Hex = 1 << 1,
        Class = 1 << 2,
        Scene = 1 << 3,
        Shop = 1 << 4,
        Enemy = 1 << 5,
        Campfire = 1 << 6,
    }

    [SerializeField]
    private PointTargets target;

    private List<Hex> availableHexes = new List<Hex>();
    private GameObject active = null;

    void Start()
    {
        GetComponent<BooleanAction>().Receive(true);
        if ((target & PointTargets.Hex) != PointTargets.None) {
            SetupHexes();
        }

        StartCoroutine(DelaySetup());
    }

    private IEnumerator DelaySetup() {
        yield return new WaitForEndOfFrame();
        PointerFacade facade = GetComponentInChildren<PointerFacade>();
        facade.SelectionAction = GetComponentInParent<Hand>().trigger;
        facade.Configuration.ConfigureSelectionAction();
    }

    protected override void Destroy(int delay, bool skipParticles) {
        if ((target & PointTargets.Hex) != PointTargets.None) {
            foreach (Hex hex in availableHexes) {
                hex.Unhighlight();
            }
        }
        base.Destroy(delay, skipParticles);
    }

    public void OnEnter(EventData data) {
        GameObject gObject = data.CollisionData.collider.gameObject;
        active = null;
        if (gObject == null) return;
        
        if ((target & PointTargets.Class) != PointTargets.None) {
            ClassSelectorController controller = gObject.GetComponentInParent<ClassSelectorController>();
            if (controller != null) {
                active = controller.gameObject;
                active.transform.localScale = Vector3.one * 1.3f;
                return;
            }
        }
        if ((target & PointTargets.Hex) != PointTargets.None) {
            Hex hex = gObject.GetComponentInParent<Hex>();
            if (hex != null && availableHexes.Contains(hex)) {
                hex.Activate();
                active = hex.gameObject;
                return;
            }
        }
        if ((target & PointTargets.Scene) != PointTargets.None) {
            SceneSelectorController controller = gObject.GetComponentInParent<SceneSelectorController>();
            if (controller != null) {
                active = controller.gameObject;
                active.transform.localScale = Vector3.one * 1.3f;
                return;
            }
        }
        if ((target & PointTargets.Shop) != PointTargets.None) {
            CardBuyerController controller = gObject.GetComponentInParent<CardBuyerController>();
            if (controller != null) {
                active = controller.gameObject;
                active.transform.localScale = Vector3.one * 1.3f;
                return;
            }
            CardRewardController rewardController = gObject.GetComponentInParent<CardRewardController>();
            if (rewardController != null) {
                active = rewardController.gameObject;
                active.transform.localScale = Vector3.one * 1.3f;
                return;
            }
        }
        if((target & PointTargets.Enemy) != PointTargets.None)
        {
            EnemyController controller = gObject.GetComponent<EnemyController>();
            if(controller != null)
            {
                if (controller.tile.inSight)
                {
                    active = controller.gameObject;
                }
                return;
            }
        }
        if ((target & PointTargets.Campfire) != PointTargets.None)
        {
            CampfireSelectorController controller = gObject.GetComponentInParent<CampfireSelectorController>();
            if (controller != null)
            {
                active = controller.gameObject;
                active.transform.localScale = Vector3.one * 1.3f;
                return;
            }
        }
    }

    public void OnExit() {
        if (active != null) {
            Hex hex = active.GetComponent<Hex>();
            if (hex)
                hex.Deactivate();
            else
                active.transform.localScale = Vector3.one;
            active = null;
        }
    }

    public void Select() {
        if (active != null)
        {
            if (card != null) {
                Trigger(active, true);
            } else {
                ClassSelectorController classSelectorController = active.GetComponent<ClassSelectorController>();
                if (classSelectorController != null) {
                    classSelectorController.Select();
                    Destroy(0, true);
                    return;
                }
                Hex hex = active.GetComponent<Hex>();
                if (hex) {
                    PlayerController controller= LevelManager.Instance.controller.playerHex.occupant.GetComponentInChildren<PlayerController>();
                    controller.transform.SetParent(hex.transform);
                    controller.transform.localPosition = Vector3.zero;
                    controller.tile.occupant = null;
                    hex.occupant = controller.gameObject;
                    controller.tile = hex;
                    PlayerManager.Instance.MovePlayer(controller.transform.position, controller.transform.rotation);
                    LevelManager.Instance.controller.playerHex = hex;
                    ResetHexes();
                    return;
                }
                SceneSelectorController sceneSelectorController = active.GetComponent<SceneSelectorController>();
                if (sceneSelectorController) {
                    sceneSelectorController.Select();
                    Destroy(0, true);
                    return;
                }
                CardBuyerController cardBuyerController = active.GetComponent<CardBuyerController>();
                if (cardBuyerController) {
                    cardBuyerController.Buy();
                    return;
                }
                CardRewardController cardRewardController = active.GetComponent<CardRewardController>();
                if (cardRewardController) {
                    cardRewardController.Choose();
                    return;
                }
                CampfireSelectorController campfireSelectorController = active.GetComponent<CampfireSelectorController>();
                if (campfireSelectorController)
                {
                    campfireSelectorController.Select();
                    Destroy(0, true);
                    return;
                }
            }
        }
    }

    public bool Accepts(object target)
    {
        return active != null;
    }

    private void ResetHexes() {
        foreach (Hex hex in availableHexes) {
            hex.Unhighlight();
        }
        availableHexes = new List<Hex>();
        SetupHexes();
    }

    private void SetupHexes() {
        int range = 100;
        Hex tile;
        if (target == PointTargets.Hex) {
            CombatAction action = card.actions.Where(a => a.type == CardAction.TYPE.MOVE).First();
            tile = CombatManager.Instance.player.tile;
            range = CombatManager.Instance.player.statusController.GetMovement(action.amount);
        } else {
            tile = LevelManager.Instance.gameObject.GetComponentInChildren<PlayerController>().tile;
        }
        List<Hex> outer = new List<Hex>() { tile };
        for (int i = 0; i < range; i++) {
            List<Hex> next = outer.SelectMany(h1 => h1.neighbors.Where(h => !outer.Contains(h) && !availableHexes.Contains(h) && tile != h && h.occupant == null)).Distinct().ToList();
            outer = next;
            availableHexes.AddRange(next);
        }
        foreach (Hex hex in availableHexes) {
            hex.Highlight();
        }
    }

    public override bool CanBeDropped() {
        return (target & PointTargets.Scene) == PointTargets.None;
    }
}
