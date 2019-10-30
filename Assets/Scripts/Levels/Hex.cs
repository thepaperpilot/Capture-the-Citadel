using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public List<Hex> neighbors;
    public GameObject occupant;

    bool visited;
    int playerDistance; //Distance to player in hexes, disregarding everything (used for deciding valid push directions)
    int meleeDistance; //Distance to the player based on available paths
    int sightDistance; //Distance to the nearest hex with sight to the player
}
