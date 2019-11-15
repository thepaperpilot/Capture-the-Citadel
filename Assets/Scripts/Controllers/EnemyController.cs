using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyController : CombatantController
{    
    public enum StrategyTypes {
        LOOP,
        RANDOM
    }

    public enum Conditions {
        HEALTH_BELOW_PERCENT,
        NUM_TURNS
    }

    public enum PathType {
        INTO_MELEE,
        INTO_RANGE
    }

    [Serializable]
    public struct TurnActions {
        [ListDrawerSettings(Expanded=true)]
        public CardAction[] actions;
        [EnumToggleButtons, ShowIf("ShowDirection")]
        public PathType pathType;

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
        public TurnActions[] moves;
    }

    [Serializable]
    public struct StrategyChange {
        public Conditions condition;
        [HideIf("condition", Conditions.HEALTH_BELOW_PERCENT)]
        [LabelText("Round Number")]
        public int number;
        [HideIf("condition", Conditions.NUM_TURNS)]
        [LabelText("0.0-1.0 Remaining health")]
        public float value;

        public Strategy newStrategy;

        [ShowIf("@newStrategy.type", StrategyTypes.LOOP)]
        [LabelText("Return to normal after first loop")]
        public bool returnAfter;

        public bool repeatable;
        [HideInEditorMode]
        public bool used;
    }

    public string displayName;

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
    private TurnActions nextMove;

    //Audio Visual Properties
    [SerializeField]
    Transform healthBarPos;
    private EnemyReadoutUI healthBar;
    public GameObject healthBarFab;
    

    void Start() {
        currentStrategy = normalStrategy;
        if (currentStrategy.type == StrategyTypes.LOOP) {
            nextMove = currentStrategy.moves[0];
        } else {
            nextMove = currentStrategy.moves[UnityEngine.Random.Range(0, currentStrategy.moves.Length)];
        }
        UpdateIntent();

        healthBar = Instantiate(healthBarFab, healthBarPos).GetComponent<EnemyReadoutUI>();
        healthBar.transform.localPosition = Vector3.zero;
        healthBar.Init(maxHealth, displayName);
    }

    void UpdateIntent()
    {
        //todo
    }

    void UpdateHealthBar()
    {

    }

    public void PlanTurn()
    {
        // Change back to normal strategy if applicable
        // and update next move
        if (currentStrategy.Equals(currentStrategyChange.newStrategy) && currentStrategyChange.returnAfter && nextMove.Equals(currentStrategy.moves.Last()))
        {
            currentStrategy = normalStrategy;
        }

        // Check if our current strategy must change
        foreach (StrategyChange change in conditionalStrategyChanges)
        {
            if (currentStrategy.Equals(change.newStrategy))
                continue;

            if (change.condition == Conditions.NUM_TURNS && turn == change.number && !change.used)
            {
                ChangeStrategy(change);
            }
        }

        if (currentStrategy.type == StrategyTypes.LOOP)
        {
            int index = (System.Array.IndexOf(currentStrategy.moves, nextMove) + 1) % currentStrategy.moves.Length;
            nextMove = currentStrategy.moves[index];
        }
        else
        {
            nextMove = currentStrategy.moves[UnityEngine.Random.Range(0, currentStrategy.moves.Length)];
        }
        Debug.Log("Intent");
        UpdateIntent();
    }

    void CheckStrategyChangeFromDamage()
    {
        foreach (StrategyChange change in conditionalStrategyChanges)
        {
            if (currentStrategy.Equals(change.newStrategy))
                continue;

            if (change.condition == Conditions.HEALTH_BELOW_PERCENT && health <= change.value * maxHealth && !change.used)
            {
                ChangeStrategy(change);
            }
        }
    }

    public void PlayTurn() {
        turn++;
        
        

        // Perform our actions
        List<AbstractAction> modifiedActions = new List<AbstractAction>();
        foreach (CardAction action in nextMove.actions) {
            action.actor = this;
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
            if(action.type == CombatAction.TYPE.MOVE)
            {
                bool moved = false;
                int remainingMovement = action.amount;
                Hex currentHex = tile;
                while(remainingMovement > 0)
                {
                    Hex bestHex = currentHex;
                    int bestScore;
                    if(nextMove.pathType == PathType.INTO_MELEE)
                    {
                        bestScore = bestHex.pathDistance;
                        foreach(Hex other in currentHex.neighbors)
                        {
                            if(other.pathDistance < currentHex.pathDistance)
                            {
                                bestHex = other;
                                bestScore = other.pathDistance;
                            }
                        }
                    }
                    else
                    {
                        bestScore = bestHex.sightDistance;
                        foreach (Hex other in currentHex.neighbors)
                        {
                            if (other.sightDistance < currentHex.sightDistance)
                            {
                                bestHex = other;
                                bestScore = other.sightDistance;
                            }
                        }
                    }

                    if(bestHex == currentHex)
                    {
                        remainingMovement = 0;
                    }
                    else
                    {
                        moved = true;
                        CardAction moveAction = new CardAction();
                        moveAction.actor = this;
                        moveAction.amount = 1;
                        moveAction.type = CombatAction.TYPE.MOVE;
                        moveAction.destination = bestHex;
                        moveAction.targets = new CombatantController[] { this };
                        modifiedActions.Add(moveAction);

                        //If the entered hex contains a trap, spring it

                        currentHex = bestHex;
                        remainingMovement--;
                    }
                }
                if (moved)
                {
                    modifiedActions.Add(new BakeNavigationAction(BakeNavigationAction.BakeType.ENEMY_CHANGED));
                }
            }
            else
            {
                modifiedActions.Add(action);
            }
            
        }
        ActionsManager.Instance.AddToTop(modifiedActions.ToArray());
    }

    private void ChangeStrategy(StrategyChange change) {
        if (!change.repeatable)
        {
            change.used = true;
        }
        currentStrategyChange = change;
        currentStrategy = change.newStrategy;
        if (CombatManager.Instance.IsPlayerTurn())
        {
            /*
            if (currentStrategy.type == StrategyTypes.LOOP)
            {
                nextMove = currentStrategy.moves[0];
            }
            else
            {
                nextMove = currentStrategy.moves[UnityEngine.Random.Range(0, currentStrategy.moves.Length)];
            }
            */
            PlanTurn();
        }
    }

    public override void Heal(int amount)
    {
        health += amount;
        health = Mathf.Min(health, maxHealth);
        UpdateHealthBar();
    }

    public override void LoseHp(int amount)
    {
        health -= amount;
        //powers
        UpdateHealthBar();
    }

    public override void TakeDamage(int amount)
    {
        health -= amount;
        //block/shield
        //powers
        UpdateHealthBar();
    }
}
