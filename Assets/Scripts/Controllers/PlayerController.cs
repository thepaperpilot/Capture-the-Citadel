using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController : CombatantController
{
    [HideInInspector]
    private int energy;

    [HideInPlayMode]
    public GameObject model;

    public int Energy { get => energy; private set => energy = value; }

    protected override void Awake()
    {
        health = maxHealth;
        PlayerManager.Instance.healthBar.Init(maxHealth, "");
    }

    public override void UpdateStatuses()
    {
        PlayerManager.Instance.healthBar.SetStatuses(statusController.GetStatuses());
    }

    [Button(ButtonSizes.Medium), HideInEditorMode]
    public void FillEnergy() {
        Energy = PlayerManager.Instance.maxEnergy;
        PlayerManager.Instance.energyBar.ChangeHealth(energy);
    }

    public void SpendEnergy(int amount) {
        Energy -= amount;
        PlayerManager.Instance.energyBar.ChangeHealth(energy);
    }

    public override void Heal(int amount)
    {
        health += amount;
        health = Mathf.Min(health, maxHealth);
        UpdateHealthBar();
    }

    public override void LoseHp(int amount)
    {
        health -= amount;
        //powers
        UpdateHealthBar();
    }

    public override void TakeDamage(int amount)
    {
        health -= amount;
        //block/shield
        //powers
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        PlayerManager.Instance.healthBar.ChangeHealth(health);
    }
}
