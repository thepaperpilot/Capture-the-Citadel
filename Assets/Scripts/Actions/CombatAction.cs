using System;
using System.Collections;
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

    [HideInInspector]
    // Targets must be set before adding this action to the ActionsManager
    public CombatantController[] targets;

    public IEnumerator Run() {
        foreach (CombatantController controller in targets) {
            switch (type) {
                case TYPE.DAMAGE:
                    ActionsManager.Instance.AddToTop(new HealAction(controller, -amount));
                    break;
                case TYPE.DRAW:
                    ActionsManager.Instance.AddToTop(new DrawAction(amount));
                    break;
                case TYPE.MOVE:
                    // TODO move actions
                    break;
                case TYPE.STATUS:
                    controller.GetComponent<StatusController>().AddStatus(status, amount);
                    break;
            }
        }
        yield return null;
    }
}
