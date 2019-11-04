using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeNavigationAction : AbstractAction
{
    public enum BakeType
    {
        PLAYER_CHANGED,
        ENEMY_CHANGED,
        OBSTACLE_CHANGED
    }

    BakeType type;

    public BakeNavigationAction(BakeType type)
    {
        this.type = type;
    }

    public IEnumerator Run()
    {
        LevelController controller = LevelManager.Instance.controller;
        switch (type)
        {
            case BakeType.PLAYER_CHANGED:
                controller.BakeLevelFromPlayerMovement();
                break;
            case BakeType.ENEMY_CHANGED:
                controller.BakeLevelFromEnemyMovement();
                break;
            case BakeType.OBSTACLE_CHANGED:
                controller.BakeLevelFromObstacleChange();
                break;
        }
        yield return null;
    }
}
