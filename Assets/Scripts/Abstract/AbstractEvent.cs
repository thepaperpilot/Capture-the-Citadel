using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Events/Generic Event")]
public class AbstractEvent : ScriptableObjectAction
{
    public enum OutcomeTypes {
        CARD,
        HEALTH,
        GOLD,
        ACTION
    }

    [Serializable]
    public struct Option {
        [EnumToggleButtons]
        public OutcomeTypes type;
        [Required]
        public string description;
        [InfoBox("This determines whether we should return to the map scene immediately after selecting this option")]
        public bool returnImmediately;
        [HideIf("type", OutcomeTypes.CARD)]
        [HideIf("type", OutcomeTypes.ACTION)]
        public int amount;
        [Space, ShowIf("type", OutcomeTypes.CARD), InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        [AssetSelector(FlattenTreeView=true, DrawDropdownForListElements=false, IsUniqueList=false)]
        public AbstractCard[] cards;
        [AssetList, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        [Space, ShowIf("type", OutcomeTypes.ACTION)]
        public ScriptableObjectAction action;

        public void Select()
        {
            switch (type) {
                case OutcomeTypes.CARD:
                    ActionsManager.Instance.AddToTop(new AddCardsAction(cards));
                    break;
                case OutcomeTypes.HEALTH:
                    ActionsManager.Instance.AddToTop(new HealAction(CombatManager.Instance.player, amount));
                    break;
                case OutcomeTypes.GOLD:
                    ActionsManager.Instance.AddToTop(new GainGoldAction(amount));
                    break;
                case OutcomeTypes.ACTION:
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
        yield return base.Run();

        OptionController[] controllers =
            LevelManager.Instance.GetComponentsInChildren<OptionController>();
        for (int i = 0; i < controllers.Length && i < options.Length; i++) {
            controllers[i].Setup(options[i], this);
        }

        EventInfoController eventInfoController =
            LevelManager.Instance.GetComponentInChildren<EventInfoController>();
        if (eventInfoController != null)
            eventInfoController.Setup(this);

        GameObject toyInstance = Instantiate(ActionsManager.Instance.eventToy);
        PlayerManager.Instance.Grab(toyInstance);
        yield return null;
    }

    public void Choose(Option option) {
        option.Select();
        if (option.returnImmediately)
            SceneManager.LoadScene("Map");
    }
}
