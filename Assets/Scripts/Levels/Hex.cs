using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public List<Hex> neighbors;
    public GameObject occupant = null;
    public List<Transform> sightTargets;

    [SerializeField]
    private LayerMask obstacleMask;

    public bool visited;
    public bool inSight;
    public int playerDistance; //Distance to player in hexes, disregarding everything (used for deciding valid push directions)
    public int pathDistance; //Distance to the player based on available paths
    public int sightDistance; //Distance to the nearest hex with sight to the player

    public void Init()
    {
        sightTargets = new List<Transform>();
        for(int i = 0; i < 6; i++)
        {
            Transform temp = new GameObject().transform;
            temp.SetParent(transform);
            Vector3 dir = 0.55f * LevelController.GetDirVector((LevelController.HexDirection)i);
            temp.localPosition = new Vector3(dir.z, 0, dir.x);
            sightTargets.Add(temp);
        }
    }

    public bool canSeeHexCorner(Hex other)
    {
        foreach(Transform corner in sightTargets)
        {
            foreach(Transform otherCorner in other.sightTargets)
            {
                Vector3 toCorner = otherCorner.position - corner.position;
                if (!Physics.Raycast(new Ray(corner.position, toCorner), toCorner.magnitude, obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool canSeeHexCenter(Hex other)
    {
        Vector3 toCenter = other.transform.position - transform.position;
        if (!Physics.Raycast(new Ray(transform.position, toCenter), toCenter.magnitude, obstacleMask))
        {
            return true;
        }
        return false;
    }

    public class PlayerDistanceAscending : IComparer<Hex>
    {
        int IComparer<Hex>.Compare(Hex a, Hex b)
        {
            if (a.playerDistance > b.playerDistance)
                return 1;
            if (a.playerDistance < b.playerDistance)
                return -1;
            else
                return 0;
        }
    }

    public class PathDistanceAscending : IComparer<Hex>
    {
        int IComparer<Hex>.Compare(Hex a, Hex b)
        {
            if (a.pathDistance > b.pathDistance)
                return 1;
            if (a.pathDistance < b.pathDistance)
                return -1;
            else
                return 0;
        }
    }

    public class SightDistanceAscending : IComparer<Hex>
    {
        int IComparer<Hex>.Compare(Hex a, Hex b)
        {
            if (a.sightDistance > b.sightDistance)
                return 1;
            if (a.sightDistance < b.sightDistance)
                return -1;
            else
                return 0;
        }
    }
}


