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

    public void OnDamageTaken(int damage) {
        TriggerRelics(AbstractRelic.Triggers.DAMAGE_TAKEN, damage);
    }

    public void OnDamageGiven(int damage) {
        TriggerRelics(AbstractRelic.Triggers.DAMAGE_GIVEN, damage);
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

    private void TriggerRelics(AbstractRelic.Triggers trigger, int data = 0) {
        IEnumerable<AbstractRelic.RelicAction> actions = new List<AbstractRelic.RelicAction>();
        foreach (RelicData relicData in relics) {
            IEnumerable<AbstractRelic.RelicAction> triggers = relicData.relic.triggers.Where(t => t.trigger == trigger);
            actions.Concat(triggers);
            foreach (AbstractRelic.RelicAction action in triggers) {
                action.relicData = relicData;
                action.data = data;
            }
        }
        ActionsManager.Instance.AddToTop(actions.ToArray());
    }
}
