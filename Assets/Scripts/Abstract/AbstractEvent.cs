using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Generic Event")]
public class AbstractEvent : ScriptableObject
{
    public enum OUTCOME_TYPE {
        CARD,
        HEALTH,
        COMBAT
    }

    [Serializable]
    public struct Option {
        [EnumToggleButtons]
        public OUTCOME_TYPE type;
        [Required]
        public string description;
        [HideIf("type", OUTCOME_TYPE.COMBAT)]
        public int amount;
        [ShowIf("type", OUTCOME_TYPE.CARD), AssetList(Path="Cards/"), InlineEditor]
        public AbstractCard card;
        [ShowIf("type", OUTCOME_TYPE.COMBAT), AssetList(Path="Combats/"), InlineEditor]
        public AbstractCombat combat;
    }

    public string title;
    public string description;
    public Option[] options;
}
