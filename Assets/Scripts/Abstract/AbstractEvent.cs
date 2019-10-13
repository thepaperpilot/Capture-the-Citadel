using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Generic Event")]
public class AbstractEvent : AbstractAction
{
    public enum OUTCOME_TYPE {
        CARD,
        HEALTH,
        DUMMY
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
        [AssetSelector(FlattenTreeView=true, DrawDropdownForListElements=false, ExcludeExistingValuesInList=true)]
        [Space, PropertyOrder(100), GUIColor(0, 1, 1)]
        public AbstractAction[] chainedEvents;

        public void Select()
        {
            if (type == OUTCOME_TYPE.CARD) {
                ActionsManager.Instance.AddToTop(new DrawAction(cards, chainedEvents));
            } else if (type == OUTCOME_TYPE.HEALTH) {
                ActionsManager.Instance.AddToTop(new HealAction(amount, chainedEvents));
            }
        }
    }

    public string title;
    public string description;
    public Option[] options;

    public override IEnumerator Run()
    {
        yield return null;
    }
}
