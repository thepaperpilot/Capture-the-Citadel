using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitToy : Toy
{


    public enum CollisionTargets {
        ENEMY
    }

    bool used = false;

    [SerializeField]
    CollisionTargets target;

    private void OnTriggerEnter(Collider other) {
        if (!used)
        {
            switch (target)
            {
                case CollisionTargets.ENEMY:
                    EnemyController controller = other.GetComponentInParent<EnemyController>();
                    if (controller != null)
                    {
                        used = true;
                        if (controller.tile.pathDistance == 1)
                        {
                            Trigger(other.gameObject);
                        }

                    }
                    break;
            }
        }
        
    }
}
