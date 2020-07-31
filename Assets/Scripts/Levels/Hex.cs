using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public List<Hex> neighbors;
    public GameObject occupant = null;
    public List<Transform> sightTargets;

    [SerializeField]
    private LayerMask obstacleMask;
    [SerializeField, AssetsOnly]
    private GameObject highlightPrefab;

    public bool visited;
    public bool inSight;
    public int playerDistance; //Distance to player in hexes, disregarding everything (used for deciding valid push directions)
    public int pathDistance; //Distance to the player based on available paths
    public int sightDistance; //Distance to the nearest hex with sight to the player

    public int row;
    public int col;
    [ChildGameObjectsOnly]
    public GameObject leftTorch;
    [ChildGameObjectsOnly]
    public GameObject rightTorch;

    private GameObject highlight;

    public void Init(int col, int row)
    {
        this.row = row;
        this.col = col;
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

    public void Highlight() {
        highlight = Instantiate(highlightPrefab, transform);
    }

    public void Unhighlight() {
        Destroy(highlight);
    }

    public void Activate() {
        if (highlight)
            highlight.transform.localScale = Vector3.one * 2;
    }

    public void Deactivate() {
        if (highlight)
            highlight.transform.localScale = Vector3.one;
    }

    public void SpawnLeftTorch() {
        leftTorch.SetActive(true);
    }

    public void SpawnRightTorch() {
        rightTorch.SetActive(true);
    }
}
