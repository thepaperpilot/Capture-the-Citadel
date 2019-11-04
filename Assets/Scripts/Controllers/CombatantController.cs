using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(StatusController))]
public class CombatantController : MonoBehaviour
{
    public int maxHealth;

    [HideInEditorMode]
    public int health = 0;
    [HideInInspector]
    public Hex tile;

    void Awake() {
        health = maxHealth;
    }
}
