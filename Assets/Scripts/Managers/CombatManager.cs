using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    [HideInInspector]
    public PlayerController player;
    [HideInInspector]
    public CombatantController[] enemies;

    [HideInPlayMode, ChildGameObjectsOnly]
    [SerializeField] private Transform playerSpawnPoint;
    [Space, HideInPlayMode, ChildGameObjectsOnly]
    [SerializeField] private Transform[] enemySpawnPoints;
    [Space, HideInEditorMode]
    [SerializeField] private CombatantController currentTurn;

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
        this.combat = combat;
        combatants.Clear();
        player = FindObjectOfType<PlayerController>();
        CardsManager.Instance.controller = player;
        combatants.Add(player);
        player.playArea.SetPositionAndRotation(playerSpawnPoint.position, playerSpawnPoint.rotation);
        CardsManager.Instance.ResetDeck();
        enemies = new CombatantController[combat.enemies.Length];
        for (int i = 0; i < combat.enemies.Length; i++) {
            AbstractEnemy enemy = combat.enemies[i];
            if (enemy.spawnPoint >= enemySpawnPoints.Length) {
                Debug.LogError("Cannot spawn enemy in spawn point " + enemy.spawnPoint + " because only " + enemySpawnPoints.Length + " spawn points exist.");
            } else {
                EnemyController controller = Instantiate(enemy.enemyPrefab, enemySpawnPoints[enemy.spawnPoint]).GetComponent<EnemyController>();
                controller.SetEnemy(enemy);
                combatants.Add(controller);
                enemies[i] = controller;
            }
        }
        // TODO add actions for relics and start the player turn after that
        currentTurn = player;
        ActionsManager.Instance.AddToTop(new PlayerTurnAction(player));
    }

    [Button(ButtonSizes.Medium), HideInEditorMode]
    public void EndTurn() {
        int index = (combatants.IndexOf(currentTurn) + 1) % combatants.Count;
        currentTurn = combatants[index];
        if (currentTurn != player) {
            ActionsManager.Instance.AddToTop(new EnemyTurnAction(currentTurn as EnemyController));
            // Add this so that the enemy turn action can add more actions to top, and their turn will end once those finish
            ActionsManager.Instance.AddToBottom(new EndTurnAction());
        } else {
            ActionsManager.Instance.AddToTop(new PlayerTurnAction(player));
        }
    }

    public bool IsPlayerTurn() {
        return currentTurn == player;
    }
}
