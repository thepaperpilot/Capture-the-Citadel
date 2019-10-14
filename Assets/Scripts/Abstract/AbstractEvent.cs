using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Generic Event")]
public class AbstractEvent : ScriptableObjectAction
{
    public enum OUTCOME_TYPE {
        CARD,
        HEALTH,
        GOLD,
        ACTION
    }

    [Serializable]
    public struct Option {
        [EnumToggleButtons]
        public OUTCOME_TYPE type;
        [Required]
        public string description;
        [ShowIf("type", OUTCOME_TYPE.HEALTH)]
        public int amount;
        [Space, ShowIf("type", OUTCOME_TYPE.CARD), InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        [AssetSelector(FlattenTreeView=true, DrawDropdownForListElements=false, IsUniqueList=false)]
        public AbstractCard[] cards;
        [AssetList, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        [Space, ShowIf("type", OUTCOME_TYPE.ACTION)]
        public ScriptableObjectAction action;

        public void Select()
        {
            switch (type) {
                case OUTCOME_TYPE.CARD:
                    ActionsManager.Instance.AddToTop(new AddCardsAction(cards));
                    break;
                case OUTCOME_TYPE.HEALTH:
                    ActionsManager.Instance.AddToTop(new HealAction(amount));
                    break;
                case OUTCOME_TYPE.GOLD:
                    ActionsManager.Instance.AddToTop(new GainGoldAction(amount));
                    break;
                case OUTCOME_TYPE.ACTION:
                    ActionsManager.Instance.AddToTop(action);
                    break;
            }
        }
    }

    public string title;
    public string description;
    [ListDrawerSettings(Expanded=true)]
    public Option[] options;

    public override IEnumerator Run()
    {
        yield return null;
    }
}
