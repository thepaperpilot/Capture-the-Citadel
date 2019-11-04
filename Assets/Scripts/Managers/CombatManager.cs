using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    [HideInInspector]
    public PlayerController player;
    [HideInInspector]
    public CombatantController[] enemies;

    [HideInEditorMode]
    [SerializeField] private CombatantController currentTurn;
    private int turn;

    private AbstractCombat combat;

    private List<CombatantController> combatants = new List<CombatantController>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(this);
        }
    }

    public void StartCombat(AbstractCombat combat) {
        combatants.Clear();
        player = LevelManager.Instance.GetComponentInChildren<PlayerController>();
        combatants.Add(player);
        enemies = LevelManager.Instance.GetComponentsInChildren<EnemyController>();
        combatants.AddRange(enemies);

        PlayerManager.Instance.SetupDropzones();
        CardsManager.Instance.ResetDeck();

        turn = 0;
        this.combat = combat;
        currentTurn = player;
        RelicsManager.Instance.OnCombatStart();
        ActionsManager.Instance.AddToTop(new PlayerTurnAction());
    }

    [Button(ButtonSizes.Medium), HideInEditorMode]
    public void EndTurn() {
        int index = (combatants.IndexOf(currentTurn) + 1) % combatants.Count;
        currentTurn = combatants[index];
        turn++;
        if (currentTurn != player) {
            ActionsManager.Instance.AddToTop(new EnemyTurnAction(currentTurn as EnemyController));
            // Add this so that the enemy turn action can add more actions to top, and their turn will end once those finish
            ActionsManager.Instance.AddToBottom(new EndTurnAction());
        } else {
            player.FillEnergy();
            RelicsManager.Instance.OnTurnStart(turn);
            ActionsManager.Instance.AddToBottom(new PlayerTurnAction());
        }
    }

    public void KillEnemy(CombatantController enemy) {
        combatants.Remove(enemy);
        if (combatants.Count == 1 && combatants[0] == player)
            EndCombat();
    }

    public void EndCombat() {
        ActionsManager.Instance.AddToTop(new GainGoldAction(Random.Range(combat.goldRange.x, combat.goldRange.y)));
        switch (combat.relicReward) {
            case AbstractCombat.RelicRewards.RANDOM_RELIC:
                RelicsManager.Instance.GetNewRelic();
                break;
            case AbstractCombat.RelicRewards.SET_RARITY:
                RelicsManager.Instance.GetNewRelic(combat.relicRarity);
                break;
            case AbstractCombat.RelicRewards.SET_RELIC:
                if (!RelicsManager.Instance.relics.Any(r => r.relic == combat.relic))
                    RelicsManager.Instance.relics.Add(new RelicsManager.RelicData() {
                        relic = combat.relic
                    });
                break;
        }
    }

    public bool IsPlayerTurn() {
        return currentTurn == player;
    }
}
