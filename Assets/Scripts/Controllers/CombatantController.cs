using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(StatusController))]
public abstract class CombatantController : MonoBehaviour
{
    [SerializeField, ShowInInspector]
    protected int maxHealth;

    [HideInEditorMode, ShowInInspector]
    protected int health = 0;
    [HideInInspector]
    public Hex tile;
    protected StatusController statusController;

    protected virtual void Awake() {
        health = maxHealth;
        statusController = GetComponent<StatusController>();
    }

    public abstract void Heal(int amount);

    public abstract void LoseHealth(int amount);

    public abstract void TakeDamage(int amount);

    public void ChangeMaxHealth(int amount)
    {
        if(amount > 0)
        {
            maxHealth += amount;
            health += amount;
        }
        else
        {
            maxHealth -= amount;
            health = Mathf.Min(health, maxHealth);
        }
    }

    public abstract void UpdateStatuses();

    public void SetMaxHealth(int newMax)
    {
        ChangeMaxHealth(newMax - maxHealth);
    }

    public int GetHealth()
    {
        return health;
    }
}
