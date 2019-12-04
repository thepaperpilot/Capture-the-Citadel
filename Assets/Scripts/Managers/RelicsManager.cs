using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class RelicsManager : MonoBehaviour
{
    public static RelicsManager Instance;

    [Serializable]
    public struct RelicData {
        [AssetList, InlineEditor]
        public AbstractRelic relic;
        [ShowIf("@relic.NeedsCard")]
        public AbstractCard card;
    }

    [HideInEditorMode]
    private List<RelicData> relics = new List<RelicData>();

    [AssetList(AutoPopulate=true)]
    public AbstractRelic[] allRelics;

    [BoxGroup("Relic weights")]
    public float common = 1;
    [BoxGroup("Relic weights")]
    public float uncommon = .5f;
    [BoxGroup("Relic weights")]
    public float rare = .1f;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void AddRelic(RelicData relicData)
    {
        relics.Add(relicData);
        List<RelicAction> triggeredActions = relicData.relic.actions.Where(t => t.trigger == RelicAction.Triggers.COLLECT).ToList();
        if(triggeredActions.Count > 0)
        {
            ActionsManager.Instance.AddToTop(triggeredActions.ToArray());
        }
    }

    public void ResetRelics()
    {
        relics = new List<RelicData>();
    }

    public List<RelicData> GetRelics()
    {
        return relics;
    }

    public void OnCombatStart() {
        TriggerRelics(RelicAction.Triggers.COMBAT_START);
    }

    public void OnCombatEnd()
    {
        TriggerRelics(RelicAction.Triggers.COMBAT_END);
    }

    public void OnTurnStart(int turn) {
        TriggerRelics(RelicAction.Triggers.TURN_START, turn);
    }

    public void OnTurnEnd(int turn)
    {
        TriggerRelics(RelicAction.Triggers.TURN_END, turn);
    }

    public void OnDamageTaken(int damage, CombatantController attacker) {
        TriggerRelics(RelicAction.Triggers.DAMAGE_TAKEN, damage, attacker);
    }

    public void OnDamageGiven(int damage, CombatantController victim) {
        TriggerRelics(RelicAction.Triggers.DAMAGE_GIVEN, damage, victim);
    }

    public void OnPlayCard(AbstractCard card)
    {
        TriggerRelics(RelicAction.Triggers.PLAY_CARD);
    }

    public void OnShuffle() {
        TriggerRelics(RelicAction.Triggers.SHUFFLE);
    }

    public void OnMonsterKilled()
    {
        TriggerRelics(RelicAction.Triggers.MONSTER_DEATH);
    }

    public void OnMovement(int amount)
    {
        TriggerRelics(RelicAction.Triggers.MOVEMENT);
    }

    public void GetNewRelic() {
        IEnumerable<AbstractRelic> validRelics = allRelics.Where(r => !relics.Any(rData => rData.relic == r));
        float total = validRelics.Sum(r =>
            r.rarity == AbstractRelic.Rarities.COMMON ? common :
            r.rarity == AbstractRelic.Rarities.UNCOMMON ? uncommon :
            r.rarity == AbstractRelic.Rarities.RARE ? rare : 0);

        float target = UnityEngine.Random.Range(0, total);
        float current = 0;
        AbstractRelic relic = validRelics.SkipWhile(r => {
            float next = current + (
                r.rarity == AbstractRelic.Rarities.COMMON ? common :
                r.rarity == AbstractRelic.Rarities.UNCOMMON ? uncommon :
                r.rarity == AbstractRelic.Rarities.RARE ? rare : 0);
            return target > next;
        }).FirstOrDefault();
        if (relic != null) {
            relics.Add(new RelicData() {
                relic = validRelics.ElementAt(UnityEngine.Random.Range(0, validRelics.Count() - 1))
            });
        } else {
            // TODO
        }
    }

    public void GetNewRelic(AbstractRelic.Rarities rarity) {
        IEnumerable<AbstractRelic> validRelics = allRelics.Where(r => r.rarity == rarity && !relics.Any(rData => rData.relic == r));
        if (validRelics.Count() > 0) {
            relics.Add(new RelicData() {
                relic = validRelics.ElementAt(UnityEngine.Random.Range(0, validRelics.Count() - 1))
            });
        } else {
            // TODO
        }
    }

    // TODO refactor to make this nicer lmao
    private void TriggerRelics(RelicAction.Triggers trigger, int data = 0, CombatantController enemy = null) {
        List<RelicAction> actions = new List<RelicAction>();
        foreach (RelicData relicData in relics) {
            List<RelicAction> triggeredActions = relicData.relic.actions.Where(t => t.trigger == trigger).ToList();
            actions.AddRange(triggeredActions);
            foreach (RelicAction action in triggeredActions) {
                if (action.trigger == RelicAction.Triggers.DAMAGE_GIVEN || action.trigger == RelicAction.Triggers.DAMAGE_TAKEN)
                {
                    switch (action.targetedEffect)
                    {
                        case RelicAction.TargetedTriggerTargets.ALL_ENEMIES:
                            action.targets = CombatManager.Instance.enemies;
                            break;
                        case RelicAction.TargetedTriggerTargets.PLAYER:
                            action.targets = new CombatantController[] { CombatManager.Instance.player };
                            break;
                        case RelicAction.TargetedTriggerTargets.RANDOM_ENEMY:
                            action.targets = new CombatantController[] {
                                CombatManager.Instance.enemies[UnityEngine.Random.Range(0, CombatManager.Instance.enemies.Length - 1)]
                            };
                            break;
                        case RelicAction.TargetedTriggerTargets.ENEMY:
                            action.targets = new CombatantController[] { enemy };
                            break;
                    }
                }
                else
                {
                    switch (action.target)
                    {
                        case RelicAction.Targets.ALL_ENEMIES:
                            action.targets = CombatManager.Instance.enemies;
                            break;
                        case RelicAction.Targets.PLAYER:
                            action.targets = new CombatantController[] { CombatManager.Instance.player };
                            break;
                        case RelicAction.Targets.RANDOM_ENEMY:
                            action.targets = new CombatantController[] {
                                CombatManager.Instance.enemies[UnityEngine.Random.Range(0, CombatManager.Instance.enemies.Length - 1)]
                            };
                            break;
                    }
                }
            }
        }
        if(actions.ToArray().Length > 0)
        {
            ActionsManager.Instance.AddToTop(actions.ToArray());
        }
    }
}
