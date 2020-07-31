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

    [Space, HorizontalGroup("Split", 50)]
    [Space, VerticalGroup("Split/Assets")]
    [HideLabel, PreviewField(50)]
    public Sprite image;
    [Space, VerticalGroup("Split/Assets")]
    [HideLabel, PreviewField(50)]
    [SerializeField]
    public GameObject model;
    [Space, VerticalGroup("Split/Properties")]
    new public string name;
    [Space, VerticalGroup("Split/Properties")]
    public string description;
    [VerticalGroup("Split/Properties")]
    public Rarities rarity;

    [Space]
    [SerializeField]
    public RelicAction[] actions;

    public bool NeedsCard {
        get {
            return actions.Any(t => t.effect == RelicAction.Effects.AFFECT_CARD);
        }
    }

    
}
