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
        DAMAGE
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

        [BoxGroup("Effect")]
        [EnumToggleButtons]
        [HideIf("@trigger == Triggers.DAMAGE_GIVEN || trigger == Triggers.DAMAGE_TAKEN")]
        public Effects effect;
        [BoxGroup("Effect")]
        [EnumToggleButtons]
        [ShowIf("@trigger == Triggers.DAMAGE_GIVEN || trigger == Triggers.DAMAGE_TAKEN")]
        public TargetedTriggerTargets targetedEffect;
        [BoxGroup("Effect")]
        [HideIf("effect", Effects.AFFECT_CARD)]
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

        public IEnumerator Run() {
            // TODO
            yield return null;
        }
    }
}
