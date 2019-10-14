using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName="Enemies/Generic Enemy")]
public class AbstractEnemy : ScriptableObject
{
    [Space, AssetList(Path="/Assets/Prefabs/Enemies")]
    public GameObject enemyPrefab;
    public int health;
    public int spawnPoint;

    public enum STRATEGY_TYPE {
        LOOP,
        RANDOM
    }

    public enum CONDITIONS {
        HALF_HEALTH
    }

    [Serializable]
    public struct Attack {
        [ListDrawerSettings(Expanded=true)]
        public AbstractCard.Effect[] effects;
    }

    [Serializable]
    public struct StrategyChange {
        [EnumToggleButtons]
        public CONDITIONS condition;

        [BoxGroup("New Strategy")]
        [HideLabel, EnumToggleButtons]
        public STRATEGY_TYPE strategyType;
        [BoxGroup("New Strategy")]
        public Attack[] moves;

        [ShowIf("strategyType", STRATEGY_TYPE.LOOP)]
        [LabelText("Return to normal strategy after first loop")]
        public bool returnAfter;
    }

    [BoxGroup("Normal Strategy")]
    [HideLabel, EnumToggleButtons]
    public STRATEGY_TYPE type;
    [BoxGroup("Normal Strategy")]
    public Attack[] moves;

    [Space]
    public StrategyChange[] conditionalStrategyChanges;
}
