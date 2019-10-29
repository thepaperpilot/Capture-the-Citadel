using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName="Relic/Generic Relic")]
public class AbstractRelic : ScriptableObject
{
    public enum Rarities {
        STARTER,
        COMMON,
        UNCOMMON,
        RARE,
        SHOP,
        BOSS
    }

    public enum Triggers {
        COMBAT_START,
        TURN_START,
        DAMAGE_TAKEN,
        DAMAGE_GIVEN,
        SHUFFLE,
        DROP_ZONE
    }

    public enum Effects {
        ADD_STATUS,
        AFFECT_CARD,
        DAMAGE,
        DRAW
    }

    public enum Targets {
        PLAYER,
        ALL_ENEMIES,
        RANDOM_ENEMY
    }

    public enum TargetedTriggerTargets {
        PLAYER,
        ALL_ENEMIES,
        RANDOM_ENEMY,
        ENEMY
    }

    [Space, HorizontalGroup("Split", 50)]
    [HideLabel, PreviewField(50)]
    public Sprite image;
    [Space, VerticalGroup("Split/Properties")]
    new public string name;
    [VerticalGroup("Split/Properties")]
    public Rarities rarity;

    [Space]
    public RelicAction[] triggers;

    public bool NeedsCard {
        get {
            return triggers.Any(t => t.effect == Effects.AFFECT_CARD);
        }
    }

    [Serializable]
    public class RelicAction : AbstractAction {
        [EnumToggleButtons]
        [BoxGroup("Trigger")]
        public Triggers trigger;
        [BoxGroup("Trigger")]
        [ShowIf("trigger", Triggers.TURN_START)]
        public bool everyTurn;
        [BoxGroup("Trigger"), MinMaxSlider(0, 11)]
        [InfoBox("Turn it up to 11 to indicate their is no upper limit")]
        [ShowIf("@(trigger == Triggers.TURN_START && !everyTurn) || trigger == Triggers.DAMAGE_GIVEN || trigger == Triggers.DAMAGE_TAKEN")]
        public Vector2Int range;
        [BoxGroup("Trigger")]
        [ShowIf("trigger", Triggers.DROP_ZONE), PreviewField(50)]
        public Sprite sprite;
        [BoxGroup("Trigger")]
        [ShowIf("trigger", Triggers.DROP_ZONE)]
        public Color color;

        [BoxGroup("Effect")]
        public Effects effect;
        [BoxGroup("Effect")]
        [HideIf("@effect == Effects.AFFECT_CARD || effect == Effects.DRAW")]
        [ShowIf("@trigger == Triggers.DAMAGE_GIVEN || trigger == Triggers.DAMAGE_TAKEN")]
        public TargetedTriggerTargets targetedEffect;
        [BoxGroup("Effect")]
        [HideIf("@effect == Effects.AFFECT_CARD || effect == Effects.DRAW")]
        [HideIf("@trigger == Triggers.DAMAGE_GIVEN || trigger == Triggers.DAMAGE_TAKEN")]
        public Targets target;
        [BoxGroup("Effect")]
        [ShowIf("effect", Effects.ADD_STATUS), InlineEditor(InlineEditorObjectFieldModes.Foldout), AssetList]
        public AbstractStatus status;
        [BoxGroup("Effect")]
        [HideIf("effect", Effects.AFFECT_CARD)]
        public int amount;

        [HideInInspector]
        public RelicsManager.RelicData relicData;
        [HideInInspector]
        public int data;
        [HideInInspector]
        public AbstractCard discard;
        [HideInInspector]
        // Targets must be set before adding this action to the ActionsManager
        public CombatantController[] targets;

        public IEnumerator Run() {
            // Make sure all conditions are met
            if ((trigger == Triggers.TURN_START && !everyTurn) ||
                trigger == Triggers.DAMAGE_GIVEN ||
                trigger == Triggers.DAMAGE_TAKEN) {
                if (data < range.x || data > range.y)
                    yield break;
            }

            // Handle effects that only apply to the player
            if (effect == Effects.AFFECT_CARD) {
                // TODO
                yield break;
            } else if (effect == Effects.DRAW) {
                ActionsManager.Instance.AddToTop(new DrawAction(amount));
                yield break;
            }

            // Handle effects that can target others
            foreach (CombatantController controller in targets) {
                switch (effect) {
                    case Effects.ADD_STATUS:
                        controller.GetComponent<StatusController>().AddStatus(status, amount);
                        break;
                    case Effects.DAMAGE:
                        if (controller != CombatManager.Instance.player)
                            RelicsManager.Instance.OnDamageGiven(amount, controller);
                        ActionsManager.Instance.AddToTop(new HealAction(controller, -amount));
                        break;
                }
            }

            yield return null;
        }
    }
}
