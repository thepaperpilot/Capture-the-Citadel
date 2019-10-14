using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    [HideInPlayMode]
    [SerializeField] private CombatantController player;
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
        combatants.Add(player);
        player.transform.SetPositionAndRotation(playerSpawnPoint.position, playerSpawnPoint.rotation);
        foreach (AbstractEnemy enemy in combat.enemies) {
            if (enemy.spawnPoint >= enemySpawnPoints.Length) {
                Debug.LogError("Cannot spawn enemy in spawn point " + enemy.spawnPoint + " because only " + enemySpawnPoints.Length + " spawn points exist.");
            } else {
                EnemyController controller = Instantiate(enemy.enemyPrefab, enemySpawnPoints[enemy.spawnPoint]).GetComponent<EnemyController>();
                controller.SetEnemy(enemy);
                combatants.Add(controller);
            }
        }
        // TODO add actions for relics and start the player turn after that
        currentTurn = player;
    }

    [Button(ButtonSizes.Medium), HideInEditorMode]
    public void EndTurn() {
        int index = (combatants.IndexOf(currentTurn) + 1) % combatants.Count;
        currentTurn = combatants[index];
        if (currentTurn != player) {
            ActionsManager.Instance.AddToTop(new EnemyTurnAction(currentTurn as EnemyController));
            // Add this so that the enemy turn action can add more actions to top, and their turn will end once those finish
            ActionsManager.Instance.AddToBottom(new EndTurnAction());
        }
    }
}
