using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Generic Card")]
public class AbstractCard : ScriptableObject
{
    public enum CARD_RARITY {
        COMMON,
        UNCOMMON,
        RARE
    }

    public enum EFFECT_TYPE {
        DAMAGE,
        HEAL,
        DRAW,
        STATUS
    }

    public enum EFFECT_TARGET {
        PLAYER,
        ENEMY,
        ALL_ENEMIES
    }

    [Serializable]
    public struct Effect {
        [EnumToggleButtons]
        public EFFECT_TYPE type;
        [EnumToggleButtons]
        public EFFECT_TARGET target;
        [ShowIf("type", EFFECT_TYPE.STATUS)]
        public AbstractStatus status;
        public int amount;
    }

    [Space, HorizontalGroup("Split", 100)]
    [HideInInlineEditors, HideLabel, PreviewField(100), OnValueChanged("DrawPreview")]
    public Material image;
    [Space, VerticalGroup("Split/Properties")]
    [HideInInlineEditors, InfoBox("Note: Text components aren't rendering in the preview, so you won't see the name or description :(")]
    // TODO if you can make a function run when the field loses focus, make it rename the scriptable object file as per https://answers.unity.com/questions/339997/change-file-name-in-a-scriptable-object.html
    new public string name;
    // TODO generate description based on effects
    [HideInInlineEditors, Multiline]
    [VerticalGroup("Split/Properties")]
    public string description;
    [HideInInlineEditors, Range(0, 5), OnValueChanged("DrawPreview")]
    public int energyCost;
    [HideInInlineEditors, EnumToggleButtons, OnValueChanged("DrawPreview")]
    public CARD_RARITY rarity;
    [HideInInlineEditors, Space]
    public Effect[] effects;
#if UNITY_EDITOR
    [OnInspectorGUI("CheckPreview"), ShowInInspector, HideLabel, InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Hidden), Space]
    private GameObject preview;
    private GameObject nextPreview;

    private void CheckPreview() {
        if (nextPreview != null) {
            preview = nextPreview;
            nextPreview = null;
        } else if (preview == null)
            DrawPreview();
    }
    
    [HideInInlineEditors, Button("Regenerate Preview")]
    private void DrawPreview() {
        GameObject temp = UnityEditor.PrefabUtility.LoadPrefabContents("Assets/Prefabs/Card.prefab");
        temp.GetComponent<CardController>().Setup(this);
        preview = null;
        nextPreview = UnityEditor.PrefabUtility.SaveAsPrefabAsset(temp, "Assets/Editor/Previews/" + name + " Card (Preview).prefab");
        UnityEditor.PrefabUtility.UnloadPrefabContents(temp);
    }
#endif

    public void Play() {
        ActionsManager.Instance.AddToBottom(new PlayCardAction(this));
    }

    public class PlayCardAction : AbstractAction {
        private AbstractCard card;

        public PlayCardAction(AbstractCard card) {
            this.card = card;
        }

        public override IEnumerator Run() {
            // TODO iterate through each effect
            yield return null;
        }
    }
}
