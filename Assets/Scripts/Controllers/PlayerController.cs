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
        base.Awake();
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

    public override void LoseHealth(int amount)
    {
        health -= Mathf.Max(0, statusController.GetHealthLost(amount));
        UpdateHealthBar();

        if (health <= 0)
        {
            Die();
        }
    }

    public override void TakeDamage(int damage)
    {
        LoseHealth(Mathf.Max(0, statusController.GetDamageTaken(damage)));
    }

    public void UpdateHealthBar()
    {
        PlayerManager.Instance.healthBar.ChangeHealth(health);
    }

    private void Die()
    {
        PlayerManager.Instance.SetClass(null);
        PlayerManager.Instance.Reset();
        CardsManager.Instance.deck = new List<AbstractCard>();
        RelicsManager.Instance.relics = new List<RelicsManager.RelicData>();
        CombatManager.Instance.maxHealth = 1;
        SceneManager.LoadScene("Title");
    }
}
