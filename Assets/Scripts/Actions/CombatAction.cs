using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class CombatAction : AbstractAction
{
    public enum TYPE {
        DAMAGE,
        DRAW,
        MOVE,
        STATUS
    }

    [EnumToggleButtons]
    public TYPE type;
    [ShowIf("type", TYPE.STATUS), InlineEditor(InlineEditorObjectFieldModes.Foldout), AssetList]
    public AbstractStatus status;
    public int amount;
    public bool ranged;

    [HideInInspector]
    // Targets must be set before adding this action to the ActionsManager
    public CombatantController[] targets;
    [HideInInspector]
    public CombatantController actor;
    [HideInInspector]
    public Hex destination;

    protected virtual int GetAmount() {
        return amount;
    }

    public IEnumerator Run() {
        foreach (CombatantController controller in targets) {
            switch (type) {
                case TYPE.DAMAGE:
                    if (actor == CombatManager.Instance.player && CombatManager.Instance.enemies.Contains(controller))
                        RelicsManager.Instance.OnDamageGiven(amount, controller);
                    else if (CombatManager.Instance.enemies.Contains(actor) && controller == CombatManager.Instance.player)
                        RelicsManager.Instance.OnDamageTaken(amount, controller);

                    ActionsManager.Instance.AddToTop(new HealAction(controller, -amount));
                    break;
                case TYPE.DRAW:
                    ActionsManager.Instance.AddToTop(new DrawAction(amount));
                    break;
                case TYPE.MOVE:
                    actor.transform.LookAt(destination.transform);
                    yield return new WaitForSeconds(1);
                    actor.transform.SetParent(destination.transform);
                    actor.transform.localPosition = Vector3.zero;
                    actor.tile.occupant = null;
                    destination.occupant = actor.gameObject;
                    actor.tile = destination;
                    break;
                case TYPE.STATUS:
                    controller.GetComponent<StatusController>().AddStatus(status, amount);
                    break;
            }
        }
        yield return null;
    }
}
