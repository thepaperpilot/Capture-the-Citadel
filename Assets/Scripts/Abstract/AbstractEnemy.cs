using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName="Enemies/Generic Enemy")]
public class AbstractEnemy : ScriptableObject
{
    [Space, AssetList(Path="Prefabs/Enemies", CustomFilterMethod="FindEnemyControllers")]
    [InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Hidden)]
    public GameObject enemyPrefab;
    public int health;
    public int spawnPoint;

    public enum STRATEGY_TYPE {
        LOOP,
        RANDOM
    }

    public enum CONDITIONS {
        HALF_HEALTH,
        NUM_TURNS,
        TOO_FAR,
        TOO_CLOSE
    }

    public enum MOVE_DIRECTION {
        CLOSER,
        FARTHER
    }

    [Serializable]
    public struct Attack {
        [ListDrawerSettings(Expanded=true)]
        public CardAction[] actions;
        [EnumToggleButtons, ShowIf("ShowDirection")]
        public MOVE_DIRECTION moveDirection;

#if UNITY_EDITOR
        private bool ShowDirection() {
            return actions.Where(effect => effect.type == CombatAction.TYPE.MOVE).Count() > 0;
        }
#endif
    }

    [Serializable, BoxGroup]
    public struct Strategy {
        [HideLabel, EnumToggleButtons]
        public STRATEGY_TYPE type;
        public Attack[] moves;
    }

    [Serializable]
    public struct StrategyChange {
        public CONDITIONS condition;
        [HideIf("condition", CONDITIONS.HALF_HEALTH)]
        public int amount;

        public Strategy newStrategy;

        [ShowIf("@newStrategy.type", STRATEGY_TYPE.LOOP)]
        [LabelText("Return to normal after first loop")]
        public bool returnAfter;
    }

    public Strategy normalStrategy;

    [Space]
    public StrategyChange[] conditionalStrategyChanges;

#if UNITY_EDITOR
    private bool FindEnemyControllers(GameObject obj) {
        return obj.GetComponentInChildren<EnemyController>() != null;
    }
#endif
}
