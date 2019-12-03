using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class CombatAction : AbstractAction
{
    public enum TYPE {
        ATTACK,
        DRAW,
        MOVE,
        STATUS,
        LOSE_HP
    }

    [EnumToggleButtons]
    public TYPE type;
    //[ShowIf("type", TYPE.STATUS), ValueDropdown("GetStatusEffects")]
    [ShowIf("type", TYPE.STATUS)]
    public Status.Name status;
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
                case TYPE.ATTACK:
                    if (actor == CombatManager.Instance.player && CombatManager.Instance.enemies.Contains(controller))
                        RelicsManager.Instance.OnDamageGiven(amount, controller);
                    else if (CombatManager.Instance.enemies.Contains(actor) && controller == CombatManager.Instance.player)
                        RelicsManager.Instance.OnDamageTaken(amount, controller);

                    if (actor == CombatManager.Instance.player)
                    {
                        if ((ranged && controller.tile.inSight) || (!ranged && controller.tile.playerDistance == 1))
                        {
                            ActionsManager.Instance.AddToTop(new TakeDamageAction(actor, controller, amount));

                        }
                    }
                    else
                    {
                        actor.transform.LookAt(controller.tile.transform);
                        if ((ranged && actor.tile.inSight) || (!ranged && actor.tile.playerDistance == 1))
                        {
                            yield return new WaitForSeconds(1);
                            ActionsManager.Instance.AddToTop(new TakeDamageAction(actor, controller, amount));
                        }
                    }
                        
                    break;
                case TYPE.DRAW:
                    ActionsManager.Instance.AddToTop(new DrawAction(amount));
                    break;
                case TYPE.MOVE:
                    if (controller == CombatManager.Instance.player) {
                        controller.transform.SetParent(destination.transform);
                        controller.transform.localPosition = Vector3.zero;
                        controller.tile.occupant = null;
                        destination.occupant = controller.gameObject;
                        controller.tile = destination;
                        PlayerManager.Instance.MovePlayer(controller.transform.position, controller.transform.rotation);
                        LevelManager.Instance.controller.playerHex = destination;
                        controller.statusController.OnMove();
                        ActionsManager.Instance.AddToTop(new BakeNavigationAction(BakeNavigationAction.BakeType.PLAYER_CHANGED));
                    } else {
                        controller.transform.LookAt(destination.transform);
                        yield return new WaitForSeconds(1);
                        controller.transform.SetParent(destination.transform);
                        controller.transform.localPosition = Vector3.zero;
                        controller.tile.occupant = null;
                        destination.occupant = controller.gameObject;
                        controller.tile = destination;
                    }
                    break;
                case TYPE.STATUS:
                    controller.GetComponent<StatusController>().AddStatus(Status.FromName(status), amount);
                    break;
                case TYPE.LOSE_HP:
                    ActionsManager.Instance.AddToTop(new LoseHealthAction(controller, amount));
                    break;
            }
        }
        yield return null;
    }

#if UNITY_EDITOR
    private static IEnumerable GetStatusEffects() {
        string root = "Assets/Statuses/";
        return UnityEditor.AssetDatabase.FindAssets("t:AbstractStatus")
            .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
            .Where(x => x.StartsWith(root))
            .Select(x => new ValueDropdownItem<AbstractStatus>(
                x.Substring(root.Length, x.Length - root.Length - 6),
                UnityEditor.AssetDatabase.LoadAssetAtPath<AbstractStatus>(x)
            ));
    }
#endif
}
