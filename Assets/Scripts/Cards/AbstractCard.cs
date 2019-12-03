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
        CLASS,
        STARTER,
        COMMON,
        UNCOMMON,
        RARE
    }

    public enum ClassColor
    {
        COLORLESS,
        RED,
        GREEN
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
    [HideInInlineEditors, OnValueChanged("DrawPreview")]
    public bool exhaust;
    [HideInInlineEditors, EnumToggleButtons, OnValueChanged("DrawPreview")]
    public Rarities rarity;
    [HideInInlineEditors, EnumToggleButtons, OnValueChanged("DrawPreview")]
    public ClassColor color;
    [HideInInlineEditors, Space]
    public CardAction[] actions;
    [SerializeField, AssetList(Path="Prefabs/Toys", CustomFilterMethod="FindToy")]
    [InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Foldout)]
    public GameObject toy;
#if UNITY_EDITOR
    [PropertySpace(20), HideLabel, OnInspectorGUI("CheckPreview")]
    [InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Hidden)]
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
        nextPreview = UnityEditor.PrefabUtility.SaveAsPrefabAsset(temp, "Assets/Editor/Previews/Card Preview (" + name + ").prefab");
        UnityEditor.PrefabUtility.UnloadPrefabContents(temp);
    }

    private bool FindToy(GameObject obj) {
        return obj.GetComponentInChildren<Toy>() != null;
    }
#endif

    public void Play(GameObject hit) {
        List<AbstractAction> modifiedActions = new List<AbstractAction>();
        foreach (CardAction action in actions) {
            action.actor = CombatManager.Instance.player;
            switch (action.target) {
                case CardAction.Targets.ALL_ENEMIES:
                    action.targets = CombatManager.Instance.enemies;
                    break;
                case CardAction.Targets.ENEMY:
                    action.targets = new CombatantController[] { hit.GetComponent<EnemyController>() };
                    break;
                case CardAction.Targets.PLAYER:
                    action.targets = new CombatantController[] { CombatManager.Instance.player };
                    break;
            }
            if(action.type == CardAction.TYPE.ATTACK)
            {
                CardAction attackAction = new CardAction();
                attackAction.actor = action.actor;
                attackAction.amount = action.actor.statusController.GetDamageDealt(action.amount);
                attackAction.type = CombatAction.TYPE.ATTACK;
                attackAction.targets = action.targets;
                modifiedActions.Add(attackAction);
            }
            if (action.type == CardAction.TYPE.MOVE) {
                action.destination = hit.GetComponent<Hex>();
                modifiedActions.Add(action);
            }
            else if(action.type == CardAction.TYPE.STATUS)
            {
                if (action.target == CardAction.Targets.ENEMY)
                {
                    if (action.targets[0].tile.inSight)
                    {
                        modifiedActions.Add(action);
                    }
                }
                else
                {
                    modifiedActions.Add(action);
                }
            }
            else
            {
                modifiedActions.Add(action);
            }
            
        }
        ActionsManager.Instance.AddToTop(modifiedActions.ToArray());
    }
}
