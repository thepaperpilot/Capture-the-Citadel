using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : CombatantController
{
    private AbstractEnemy enemy;
    
    public void SetEnemy(AbstractEnemy enemy) {
        this.enemy = enemy;
    }
}
