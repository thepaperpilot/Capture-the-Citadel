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
    public enum PointTargets {
        HEX,
        CLASS
    }

    [SerializeField]
    private PointTargets target;

    private List<Hex> availableHexes;
    private GameObject active = null;

    void Start()
    {
        GetComponent<BooleanAction>().Receive(true);
        if (target == PointTargets.HEX) {
            CombatAction action = card.actions.Where(a => a.type == CardAction.TYPE.MOVE).First();
            availableHexes = new List<Hex>();
            Hex tile = CombatManager.Instance.player.tile;
            List<Hex> outer = new List<Hex>() { tile };
            for (int i = 0; i < action.amount; i++) {
                List<Hex> next = outer.SelectMany(h1 => h1.neighbors.Where(h => !outer.Contains(h) && !availableHexes.Contains(h) && tile != h && h.occupant == null)).Distinct().ToList();
                outer = next;
                availableHexes.AddRange(next);
            }
            foreach (Hex hex in availableHexes) {
                hex.Highlight();
            }
        }

        StartCoroutine(DelaySetup());
    }

    private IEnumerator DelaySetup() {
        yield return new WaitForEndOfFrame();
        PointerFacade facade = GetComponentInChildren<PointerFacade>();
        facade.SelectionAction = GetComponentInParent<Hand>().trigger;
        facade.Configuration.ConfigureSelectionAction();
    }

    public override void Destroy(int delay) {
        if (target == PointTargets.HEX) {
            foreach (Hex hex in availableHexes) {
                hex.Unhighlight();
            }
        }
        base.Destroy(delay);
    }

    public void OnEnter(EventData data) {
        GameObject gObject = data.CollisionData.collider.gameObject;
        if (target == PointTargets.HEX) {
            if (gObject != null && gObject.CompareTag("Hex") && availableHexes.Contains(gObject.GetComponentInParent<Hex>())) {
                Hex hex = gObject.GetComponentInParent<Hex>();
                hex.Activate();
                active = hex.gameObject;
            } else active = null;
        } else if (target == PointTargets.CLASS) {
            if (gObject != null && gObject.CompareTag("Card")) {
                ClassSelectorController controller = gObject.GetComponentInParent<ClassSelectorController>();
                active = controller.gameObject;
                active.transform.localScale = Vector3.one * 1.3f;
            } else active = null;
        } else active = null;
    }

    public void OnExit() {
        if (active != null) {
            if (target == PointTargets.HEX)
                active.GetComponent<Hex>().Deactivate();
            else if (target == PointTargets.CLASS)
                active.transform.localScale = Vector3.one;
            active = null;
        }
    }

    public void Select() {
        if (active != null) {
            if (target == PointTargets.CLASS) {
                active.GetComponent<ClassSelectorController>().Select();
                Destroy(0);
            } else {
                Trigger(active, true);
            }
        }
    }

    public bool Accepts(object target)
    {
        return active != null;
    }
}
