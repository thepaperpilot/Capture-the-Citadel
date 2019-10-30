using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyController : CombatantController
{    
    public int maxHealth;

    public enum StrategyTypes {
        LOOP,
        RANDOM
    }

    public enum Conditions {
        HALF_HEALTH,
        NUM_TURNS,
        TOO_FAR,
        TOO_CLOSE
    }

    public enum MoveDirections {
        CLOSER,
        FARTHER
    }

    [Serializable]
    public struct Attack {
        [ListDrawerSettings(Expanded=true)]
        public CardAction[] actions;
        [EnumToggleButtons, ShowIf("ShowDirection")]
        public MoveDirections moveDirection;

#if UNITY_EDITOR
        private bool ShowDirection() {
            return actions.Where(effect => effect.type == CombatAction.TYPE.MOVE).Count() > 0;
        }
#endif
    }

    [Serializable, BoxGroup]
    public struct Strategy {
        [HideLabel, EnumToggleButtons]
        public StrategyTypes type;
        public Attack[] moves;
    }

    [Serializable]
    public struct StrategyChange {
        public Conditions condition;
        [HideIf("condition", Conditions.HALF_HEALTH)]
        public int amount;

        public Strategy newStrategy;

        [ShowIf("@newStrategy.type", StrategyTypes.LOOP)]
        [LabelText("Return to normal after first loop")]
        public bool returnAfter;
    }

    public Strategy normalStrategy;

    [Space]
    public StrategyChange[] conditionalStrategyChanges;
    
    [SerializeField, HideInEditorMode]
    private int turn = 0;
    [SerializeField, HideInEditorMode]
    private Strategy currentStrategy;
    [SerializeField, HideInEditorMode]
    private StrategyChange currentStrategyChange;
    [SerializeField, HideInEditorMode]
    private Attack nextMove;

    public void PlayTurn() {
        turn++;
        
        // Check if our current strategy must change
        foreach (StrategyChange change in conditionalStrategyChanges) {
            if (currentStrategy.Equals(change.newStrategy)) continue;
            switch (change.condition) {
                case Conditions.HALF_HEALTH:
                    if (health < maxHealth * .5f)
                        ChangeStrategy(change);
                    break;
                case Conditions.NUM_TURNS:
                    if (turn == change.amount)
                        ChangeStrategy(change);
                    break;
                case Conditions.TOO_CLOSE:
                    // TODO positioning system
                    break;
                case Conditions.TOO_FAR:
                    // TODO positioning system
                    break;
            }
        }

        // Perform our actions
        foreach (CardAction action in nextMove.actions) {
            switch (action.target) {
                case CardAction.Targets.ALL_ENEMIES:
                    action.targets = CombatManager.Instance.enemies;
                    break;
                case CardAction.Targets.ENEMY:
                    action.targets = new CombatantController[] { this };
                    break;
                case CardAction.Targets.PLAYER:
                    action.targets = new CombatantController[] { CombatManager.Instance.player };
                    break;
            }
            action.actor = this;
        }
        ActionsManager.Instance.AddToTop(nextMove.actions);

        // Change back to normal strategy if applicable
        // and update next move
        if (currentStrategy.Equals(currentStrategyChange.newStrategy) && currentStrategyChange.returnAfter && nextMove.Equals(currentStrategy.moves.Last())) {
            currentStrategy = normalStrategy;
            nextMove = currentStrategy.type == StrategyTypes.LOOP ? currentStrategy.moves[0] : currentStrategy.moves[UnityEngine.Random.Range(0, currentStrategy.moves.Length)];
        } else {
            if (currentStrategy.type == StrategyTypes.LOOP) {
                int index = System.Array.IndexOf(currentStrategy.moves, nextMove) % currentStrategy.moves.Length;
                nextMove = currentStrategy.moves[index + 1];
            } else {
                nextMove = currentStrategy.moves[UnityEngine.Random.Range(0, currentStrategy.moves.Length)];
            }
        }
    }

    private void ChangeStrategy(StrategyChange change) {
        currentStrategyChange = change;
        currentStrategy = change.newStrategy;
        nextMove = currentStrategy.moves[0];
    }
}
