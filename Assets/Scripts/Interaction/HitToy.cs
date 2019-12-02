using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitToy : Toy
{
    public enum CollisionTargets {
        ENEMY
    }

    [SerializeField]
    CollisionTargets target;

    private void OnTriggerEnter(Collider other) {
        switch (target) {
            case CollisionTargets.ENEMY:
                EnemyController controller = other.GetComponentInParent<EnemyController>();
                if (controller != null) {
                    if(controller.tile.pathDistance == 1)
                    {
                        Trigger(other.gameObject);
                    }
                    
                }
                break;
        }
    }
}
