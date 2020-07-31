using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : CombatantController
{
    [HideInInspector]
    private int energy;

    [HideInPlayMode]
    public GameObject model;

    public int Energy { get => energy; private set => energy = value; }

    protected override void Awake()
    {
        maxHealth = PlayerManager.Instance.maxHealth;
        health = PlayerManager.Instance.health;
        statusController = GetComponent<StatusController>();
        PlayerManager.Instance.healthBar.Init(maxHealth, "");
        PlayerManager.Instance.healthBar.ChangeValue(health);
    }

    public override void UpdateStatuses()
    {
        PlayerManager.Instance.healthBar.SetStatuses(statusController.GetStatuses());
    }

    [Button(ButtonSizes.Medium), HideInEditorMode]
    public void FillEnergy() {
        Energy = PlayerManager.Instance.energyPerTurn;
        PlayerManager.Instance.energyBar.ChangeValue(energy);
    }

    public void SpendEnergy(int amount) {
        if(amount > 0)
        {
            Energy -= amount;
            PlayerManager.Instance.energyBar.ChangeValue(energy);
        }
    }

    public void AddEnergy(int amount)
    {
        if(amount > 0)
        {
            Energy += amount;
            PlayerManager.Instance.energyBar.ChangeValue(energy);
        }
    }

    public override void Heal(int amount)
    {
        health += amount;
        health = Mathf.Min(health, maxHealth);
        PlayerManager.Instance.health = health;
        UpdateHealthBar();
    }

    public override void LoseHealth(int amount)
    {
        health -= Mathf.Max(0, statusController.GetHealthLost(amount, false));
        PlayerManager.Instance.health = health;
        UpdateHealthBar();

        if (health <= 0)
        {
            PlayerManager.Instance.Die();
        }
    }

    public override void TakeDamage(int damage)
    {
        LoseHealth(Mathf.Max(0, statusController.GetDamageTaken(damage, false)));
    }

    public void UpdateHealthBar()
    {
        PlayerManager.Instance.healthBar.ChangeValue(health);
    }

    
}
