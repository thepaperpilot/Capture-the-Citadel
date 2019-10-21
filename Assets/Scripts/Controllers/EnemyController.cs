using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : CombatantController
{
    [SerializeField, InlineEditor, HideInEditorMode]
    private AbstractEnemy enemy;
    [SerializeField, HideInEditorMode]
    private int turn = 0;
    [SerializeField, HideInEditorMode]
    private int health = 0;
    [SerializeField, HideInEditorMode]
    private AbstractEnemy.Strategy strategy;
    private AbstractEnemy.StrategyChange strategyChange;
    [SerializeField, HideInEditorMode]
    private AbstractEnemy.Attack nextMove;
    
    public void SetEnemy(AbstractEnemy enemy) {
        this.enemy = enemy;
        turn = 0;
        health = enemy.health;
        strategy = enemy.normalStrategy;
        nextMove = strategy.moves[0];
    }

    public void PlayTurn() {
        turn++;
        
        // Check if our current strategy must change
        foreach (AbstractEnemy.StrategyChange change in enemy.conditionalStrategyChanges) {
            if (strategy.Equals(change.newStrategy)) continue;
            switch (change.condition) {
                case AbstractEnemy.CONDITIONS.HALF_HEALTH:
                    if (health < enemy.health * .5f)
                        ChangeStrategy(change);
                    break;
                case AbstractEnemy.CONDITIONS.NUM_TURNS:
                    if (turn == change.amount)
                        ChangeStrategy(change);
                    break;
                case AbstractEnemy.CONDITIONS.TOO_CLOSE:
                    // TODO positioning system
                    break;
                case AbstractEnemy.CONDITIONS.TOO_FAR:
                    // TODO positioning system
                    break;
            }
        }

        // Perform our actions
        foreach (CardAction action in nextMove.actions) {
            switch (action.target) {
                case CardAction.TARGET.ALL_ENEMIES:
                    action.targets = CombatManager.Instance.enemies;
                    break;
                case CardAction.TARGET.ENEMY:
                    action.targets = new CombatantController[] { this };
                    break;
                case CardAction.TARGET.PLAYER:
                    action.targets = new CombatantController[] { CombatManager.Instance.player };
                    break;
            }
        }
        ActionsManager.Instance.AddToTop(nextMove.actions);

        // Change back to normal strategy if applicable
        // and update next move
        if (strategy.Equals(strategyChange.newStrategy) && strategyChange.returnAfter && nextMove.Equals(strategy.moves.Last())) {
            strategy = enemy.normalStrategy;
            nextMove = strategy.type == AbstractEnemy.STRATEGY_TYPE.LOOP ? strategy.moves[0] : strategy.moves[Random.Range(0, strategy.moves.Length)];
        } else {
            if (strategy.type == AbstractEnemy.STRATEGY_TYPE.LOOP) {
                int index = System.Array.IndexOf(strategy.moves, nextMove) % strategy.moves.Length;
                nextMove = strategy.moves[index + 1];
            } else {
                nextMove = strategy.moves[Random.Range(0, strategy.moves.Length)];
            }
        }
    }

    private void ChangeStrategy(AbstractEnemy.StrategyChange change) {
        strategyChange = change;
        strategy = change.newStrategy;
        nextMove = strategy.moves[0];
    }
}
