using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController : CombatantController
{
    [SerializeField] private int maxEnergy;
    [HideInInspector]
    // TODO way to see energy amount in-game
    public int energy;

    [HideInPlayMode]
    public GameObject model;

    [Button(ButtonSizes.Medium), HideInEditorMode]
    public void FillEnergy() {
        energy = maxEnergy;
    }
}
