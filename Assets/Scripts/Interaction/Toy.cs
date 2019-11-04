using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Toy : MonoBehaviour
{
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

    protected void Trigger(GameObject gameObject = null, bool overrideDelay = false) {
        if (triggered) return;

        triggered = true;
        card.Play(gameObject);
        Destroy(overrideDelay ? 0 : delayDespawn);
    }

    public virtual void Destroy(int delay) {
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
