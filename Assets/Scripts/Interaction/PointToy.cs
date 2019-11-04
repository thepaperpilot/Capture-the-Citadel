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
        HEX
    }

    [SerializeField]
    private PointTargets target;

    private List<Hex> availableHexes;
    private Hex active = null;
    private bool firstUpdate = true;

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

    /* 
        PointerFacade facade = GetComponentInChildren<PointerFacade>();
        facade.SelectionAction = GetComponentInParent<Hand>().trigger;
        facade.Configuration.ConfigureSelectionAction();
        */
    }

    void Update() {
        if (firstUpdate) {
            PointerFacade facade = GetComponentInChildren<PointerFacade>();
            facade.SelectionAction = GetComponentInParent<Hand>().trigger;
            facade.Configuration.ConfigureSelectionAction();
            firstUpdate = false;
        }
    }

    void OnDestroy() {
        if (target == PointTargets.HEX) {
            foreach (Hex hex in availableHexes) {
                hex.Unhighlight();
            }
        }
    }

    public void OnEnter(EventData data) {
        GameObject gObject = data.CollisionData.collider.gameObject;
        if (gObject != null && gObject.CompareTag("Hex") && availableHexes.Contains(gObject.GetComponentInParent<Hex>())) {
            active = gObject.GetComponentInParent<Hex>();
            active.Activate();
        } else active = null;
    }

    public void OnExit() {
        if (active != null) {
            active.Deactivate();
            active = null;
        }
    }

    public void Select() {
        if (active != null) {
            Debug.Log("Worked!");
            Trigger(active.gameObject);
        }
    }

    public bool Accepts(object target)
    {
        return active != null;
    }
}
