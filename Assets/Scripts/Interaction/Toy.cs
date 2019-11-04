using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Toy : MonoBehaviour
{
    public enum CollisionTargets {
        ENEMY
    }

    [SerializeField]
    private CollisionTargets target;
    [SerializeField, SceneObjectsOnly]
    private GameObject toyRoot;
    [SerializeField, SceneObjectsOnly]
    private ParticleSystem particles;
    [SerializeField, SceneObjectsOnly]
    new private MeshRenderer renderer;
    [SerializeField]
    private int delayDespawn = 2;
    private bool triggered = false;
    
    [HideInInspector]
    public AbstractCard card;

    private void OnTriggerEnter(Collider other) {
        if (triggered) return;

        switch (target) {
            case CollisionTargets.ENEMY:
                if (other.GetComponentInParent<EnemyController>()) {
                    triggered = true;
                    card.Play(other.gameObject);
                    Destroy(delayDespawn);
                }
                break;
        }
    }

    public void Destroy(int delay) {
        StartCoroutine(DelayParticles(delay));
    }

    IEnumerator DelayParticles(int delay) {
        yield return new WaitForSeconds(delay);
        particles.gameObject.SetActive(false);
        particles.gameObject.SetActive(true);
        renderer.gameObject.SetActive(false);
        Destroy(toyRoot, 1.5f);
    }
}
