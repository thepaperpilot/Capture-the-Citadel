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
    public List<RelicData> relics = new List<RelicData>();

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
            return next > target;
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
