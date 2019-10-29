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

    public List<RelicData> relics = new List<RelicData>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(this);
        }
    }

    public void OnCombatStart() {
        TriggerRelics(AbstractRelic.Triggers.COMBAT_START);
    }

    public void OnTurnStart(int turn) {
        TriggerRelics(AbstractRelic.Triggers.COMBAT_START, turn);
    }

    public void OnDamageTaken(int damage, CombatantController attacker) {
        TriggerRelics(AbstractRelic.Triggers.DAMAGE_TAKEN, damage, attacker);
    }

    public void OnDamageGiven(int damage, CombatantController victim) {
        TriggerRelics(AbstractRelic.Triggers.DAMAGE_GIVEN, damage, victim);
    }

    public void OnShuffle() {
        TriggerRelics(AbstractRelic.Triggers.SHUFFLE);
    }

    public void GetNewRelic() {
        // TODO
    }

    public void GetNewRelic(AbstractRelic.Rarities rarity) {
        // TODO
    }

    // TODO refactor to make this nicer lmao
    private void TriggerRelics(AbstractRelic.Triggers trigger, int data = 0, CombatantController enemy = null) {
        IEnumerable<AbstractRelic.RelicAction> actions = new List<AbstractRelic.RelicAction>();
        foreach (RelicData relicData in relics) {
            IEnumerable<AbstractRelic.RelicAction> triggers = relicData.relic.triggers.Where(t => t.trigger == trigger);
            actions.Concat(triggers);
            foreach (AbstractRelic.RelicAction action in triggers) {
                action.relicData = relicData;
                action.data = data;
                if (action.trigger == AbstractRelic.Triggers.DAMAGE_GIVEN || action.trigger == AbstractRelic.Triggers.DAMAGE_TAKEN) {
                    switch (action.targetedEffect) {
                        case AbstractRelic.TargetedTriggerTargets.ALL_ENEMIES:
                            action.targets = CombatManager.Instance.enemies;
                            break;
                        case AbstractRelic.TargetedTriggerTargets.PLAYER:
                            action.targets = new CombatantController[] { CombatManager.Instance.player };
                            break;
                        case AbstractRelic.TargetedTriggerTargets.RANDOM_ENEMY:
                            action.targets = new CombatantController[] {
                                CombatManager.Instance.enemies[UnityEngine.Random.Range(0, CombatManager.Instance.enemies.Length - 1)]
                            };
                            break;
                        case AbstractRelic.TargetedTriggerTargets.ENEMY:
                            action.targets = new CombatantController[] { enemy };
                            break;
                    }
                } else {
                    switch (action.target) {
                        case AbstractRelic.Targets.ALL_ENEMIES:
                            action.targets = CombatManager.Instance.enemies;
                            break;
                        case AbstractRelic.Targets.PLAYER:
                            action.targets = new CombatantController[] { CombatManager.Instance.player };
                            break;
                        case AbstractRelic.Targets.RANDOM_ENEMY:
                            action.targets = new CombatantController[] {
                                CombatManager.Instance.enemies[UnityEngine.Random.Range(0, CombatManager.Instance.enemies.Length - 1)]
                            };
                            break;
                    }
                }
            }
        }
        ActionsManager.Instance.AddToTop(actions.ToArray());
    }
}
