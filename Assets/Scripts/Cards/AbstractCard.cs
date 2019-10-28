using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Generic Card")]
public class AbstractCard : ScriptableObject
{
    public enum Rarities {
        COMMON,
        UNCOMMON,
        RARE
    }

    [Space, HorizontalGroup("Split", 100)]
    [HideInInlineEditors, HideLabel, PreviewField(100), OnValueChanged("DrawPreview")]
    public Sprite image;
    [Space, VerticalGroup("Split/Properties"), Delayed]
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
    public Rarities rarity;
    [HideInInlineEditors, Space]
    public CardAction[] actions;
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
        if (name == "") return;
        GameObject temp = UnityEditor.PrefabUtility.LoadPrefabContents("Assets/Prefabs/Card.prefab");
        temp.GetComponent<CardController>().Setup(this);
        preview = null;
        nextPreview = UnityEditor.PrefabUtility.SaveAsPrefabAsset(temp, "Assets/Editor/Previews/" + name + " Card (Preview).prefab");
        UnityEditor.PrefabUtility.UnloadPrefabContents(temp);
    }
#endif

    public void Play() {
        if (actions.Any(action => action.target == CardAction.Targets.ENEMY)) {
            // TODO enemy selection system
        } else {
            foreach (CardAction action in actions) {
                action.targets = action.target == CardAction.Targets.PLAYER ?
                    new CombatantController[] { CombatManager.Instance.player } :
                    CombatManager.Instance.enemies;
            }
            ActionsManager.Instance.AddToTop(actions);
        }
    }
}
