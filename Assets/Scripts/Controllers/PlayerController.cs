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

    [Button(ButtonSizes.Medium), HideInEditorMode]
    public void FillEnergy() {
        Energy = PlayerManager.Instance.maxEnergy;
        PlayerManager.Instance.energyBar.ChangeHealth(energy);
    }

    public void SpendEnergy(int amount) {
        Energy -= amount;
        PlayerManager.Instance.energyBar.ChangeHealth(energy);
    }
}
