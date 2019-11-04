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

    [Button(ButtonSizes.Medium)]
    public void FillEnergy() {
        energy = maxEnergy;
    }
}
