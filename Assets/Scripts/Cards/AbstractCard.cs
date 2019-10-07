using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Generic Card")]
public class AbstractCard : ScriptableObject
{
    public enum EFFECT_TYPE {
        DAMAGE,
        HEAL,
        DRAW,
        STATUS
    }

    public enum EFFECT_TARGET {
        SELF,
        ENEMY
    }

    [Serializable]
    public struct Effect {
        public EFFECT_TYPE type;
        public EFFECT_TARGET target;
        public int amount;
        public AbstractStatus status;
    }

    new public string name;
    public string description;
    public GameObject cardPrefab;
    public int energyCost;
    public Effect[] effects;

    public void Play() {
        ActionsManager.Instance.AddToBottom(new PlayCardAction(this));
    }

    public class PlayCardAction : AbstractAction {
        private AbstractCard card;

        public PlayCardAction(AbstractCard card) {
            this.card = card;
        }

        public override IEnumerator Run() {
            yield return null;
        }
    }
}
