using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    [HideInInspector]
    public PlayerController player;
    [HideInInspector]
    public CombatantController[] enemies;
    [HideInInspector]
    public int maxHealth;

    [HideInEditorMode]
    [SerializeField] private CombatantController currentTurn;
    private int turn;

    private AbstractCombat combat;

    [HideInEditorMode, ShowInInspector]
    private List<CombatantController> combatants = new List<CombatantController>();

    [SerializeField, ValueDropdown("FindScenes")]
    private string rewardsScreen;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void StartCombat(AbstractCombat combat) {
        combatants.Clear();
        player = LevelManager.Instance.GetComponentInChildren<PlayerController>();
        combatants.Add(player);
        //init player health bar based on current health
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
        if (currentTurn != player) {
            ((EnemyController)currentTurn).EndTurn();
        }
        int index = (combatants.IndexOf(currentTurn) + 1) % combatants.Count;
        currentTurn = combatants[index];
        turn++;
        if (currentTurn != player) {
            ActionsManager.Instance.AddToTop(new EnemyTurnAction(currentTurn as EnemyController));
            // Add this so that the enemy turn action can add more actions to top, and their turn will end once those finish
            ActionsManager.Instance.AddToBottom(new EndTurnAction());
        } else {
            foreach(CombatantController combatant in combatants)
            {
                if(combatant != player)
                {
                    ((EnemyController)combatant).PlanTurn();
                }
            }
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
        ActionsManager.Instance.AddToBottom(new ChangeSceneAction(rewardsScreen));
    }

    public bool IsPlayerTurn() {
        return currentTurn == player;
    }
#if UNITY_EDITOR
    private IEnumerable FindScenes() {
        int sceneCount = SceneManager.sceneCountInBuildSettings;     
        ValueDropdownItem[] scenes = new ValueDropdownItem[sceneCount];
        for( int i = 0; i < sceneCount; i++ ) {
            string name = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            scenes[i] = new ValueDropdownItem(name, name);
        }
        return scenes;
    }
#endif
}
