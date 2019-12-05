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
        [EnumToggleButtons, ShowIf("ShowMajorDebuff")]
        public bool debuffIsMajor;

#if UNITY_EDITOR
        private bool ShowDirection() {
            return actions.Where(effect => effect.type == CombatAction.TYPE.MOVE).Count() > 0;
        }

        private bool ShowMajorDebuff()
        {
            return actions.Where(effect => effect.type == CombatAction.TYPE.STATUS && effect.target == CardAction.Targets.PLAYER).Count() > 0;
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
    public class StrategyChange {
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
        healthBar = Instantiate(healthBarFab, healthBarPos).GetComponent<EnemyReadoutUI>();
        healthBar.transform.localPosition = Vector3.zero;
        healthBar.Init(maxHealth, displayName);
        healthBar.UpdateLoS(tile.inSight);

        currentStrategy = normalStrategy;
        foreach (StrategyChange change in conditionalStrategyChanges)
        {
            if (currentStrategy.Equals(change.newStrategy))
                continue;

            if (change.condition == Conditions.NUM_TURNS && turn == change.number && !change.used)
            {
                ChangeStrategy(change);
            }
        }
        if (currentStrategy.type == StrategyTypes.LOOP) {
            nextMove = currentStrategy.moves[0];
        } else {
            nextMove = currentStrategy.moves[UnityEngine.Random.Range(0, currentStrategy.moves.Length)];
        }

        

        UpdateIntent();
    }

    public void SetInSight(bool inSight)
    {
        if (healthBar)
        {
            healthBar.UpdateLoS(inSight);
        }
    }

    void UpdateIntent()
    {
        bool debuffIncluded = false;
        bool majorDebuffIncluded = false;
        bool allBuffIncluded = false;
        bool selfBuffIncluded = false;
        List<EnemyReadoutUI.Intent> intents = new List<EnemyReadoutUI.Intent>();

        foreach(CardAction action in nextMove.actions)
        {
            if(action.type == CombatAction.TYPE.ATTACK)
            {
                if (action.ranged)
                {
                    intents.Add(new EnemyReadoutUI.Intent(IntentIcon.Intent.ATTACK_RANGED, statusController.GetDamageDealt(action.amount, true)));
                }
                else if(action.amount < IntentIcon.MajorDamageThreshold)
                {
                    intents.Add(new EnemyReadoutUI.Intent(IntentIcon.Intent.ATTACK, statusController.GetDamageDealt(action.amount, true)));
                }
                else
                {
                    intents.Add(new EnemyReadoutUI.Intent(IntentIcon.Intent.ATTACK_MAJOR, statusController.GetDamageDealt(action.amount, true)));
                }
            }
            else if (action.type == CombatAction.TYPE.MOVE)
            {
                intents.Add(new EnemyReadoutUI.Intent(IntentIcon.Intent.MOVE, statusController.GetMovement(action.amount, true)));
            }
            else if(action.type == CombatAction.TYPE.STATUS)
            {
                if(nextMove.debuffIsMajor && !majorDebuffIncluded && action.target == CardAction.Targets.PLAYER)
                {
                    intents.Add(new EnemyReadoutUI.Intent(IntentIcon.Intent.DEBUFF_MAJOR));
                    majorDebuffIncluded = true;
                }
                else if(!nextMove.debuffIsMajor && !debuffIncluded && action.target == CardAction.Targets.PLAYER)
                {
                    intents.Add(new EnemyReadoutUI.Intent(IntentIcon.Intent.DEBUFF));
                    debuffIncluded = true;
                }
                else if(!selfBuffIncluded && action.target == CardAction.Targets.ENEMY)
                {
                    intents.Add(new EnemyReadoutUI.Intent(IntentIcon.Intent.BUFF));
                    selfBuffIncluded = true;
                }
                else if (!allBuffIncluded && action.target == CardAction.Targets.ALL_ENEMIES)
                {
                    intents.Add(new EnemyReadoutUI.Intent(IntentIcon.Intent.BUFF_MAJOR));
                    allBuffIncluded = true;
                }
            }
            else
            {
                intents.Add(new EnemyReadoutUI.Intent(IntentIcon.Intent.OTHER));
            }
        }

        healthBar.SetIntents(intents);
    }

    public override void UpdateStatuses()
    {
        healthBar.SetStatuses(statusController.GetStatuses());
        UpdateIntent();
    }

    public void PlanTurn()
    {
        // Change back to normal strategy if applicable
        // and update next move
        if (currentStrategy.Equals(currentStrategyChange.newStrategy) && currentStrategyChange.returnAfter && nextMove.Equals(currentStrategy.moves.Last()))
        {
            Debug.Log("Returning to normal strategy");
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
            Debug.Log("output" + System.Array.IndexOf(currentStrategy.moves, nextMove));
            Debug.Log(index);
            nextMove = currentStrategy.moves[index];
        }
        else
        {
            nextMove = currentStrategy.moves[UnityEngine.Random.Range(0, currentStrategy.moves.Length)];
        }
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
        statusController.OnTurnStart();
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
                int remainingMovement = statusController.GetMovement(action.amount, false);
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
                            if(other.pathDistance < currentHex.pathDistance && other.occupant == null)
                            {
                                bestHex = other;
                                bestScore = other.pathDistance;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log(tile);
                        bestScore = bestHex.sightDistance;
                        foreach (Hex other in currentHex.neighbors)
                        {
                            if (other.sightDistance < currentHex.sightDistance && other.occupant == null)
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

                        currentHex = bestHex;
                        remainingMovement--;
                    }
                }
                if (moved)
                {
                    modifiedActions.Add(new BakeNavigationAction(BakeNavigationAction.BakeType.ENEMY_CHANGED));
                }
            }
            else if (action.type == CardAction.TYPE.ATTACK)
            {
                CardAction attackAction = new CardAction();
                attackAction.ranged = action.ranged;
                attackAction.actor = this;
                attackAction.amount = statusController.GetDamageDealt(action.amount, false);
                attackAction.type = CombatAction.TYPE.ATTACK;
                attackAction.targets = action.targets;
                modifiedActions.Add(attackAction);
            }
            else if (action.type == CardAction.TYPE.STATUS)
            {
                if(action.target == CardAction.Targets.PLAYER)
                {
                    if (tile.inSight)
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

    public void EndTurn()
    {
        statusController.OnTurnEnd();
        healthBar.ClearIntents();
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
            Debug.Log("got changed");
            PlanTurn();
        }
    }

    public override void Heal(int amount)
    {
        health += amount;
        health = Mathf.Min(health, maxHealth);
        healthBar.ChangeValue(health);
    }

    public override void LoseHealth(int amount)
    {
        health -= Mathf.Max(0, statusController.GetHealthLost(amount, false));
        healthBar.ChangeValue(health);

        if (health <= 0)
        {
            CombatManager.Instance.KillEnemy(this);
        }
        else
        {
            CheckStrategyChangeFromDamage();
        }
    }

    public override void TakeDamage(int damage)
    {
        LoseHealth(Mathf.Max(0, statusController.GetDamageTaken(damage, false)));
    }
}
