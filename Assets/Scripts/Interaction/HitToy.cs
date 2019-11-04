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
                if (other.GetComponentInParent<EnemyController>()) {
                    Trigger(other.gameObject);
                }
                break;
        }
    }
}
