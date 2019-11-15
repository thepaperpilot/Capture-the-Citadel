using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public enum HexDirection
    {
        EAST,
        NORTH_EAST,
        NORTH_WEST,
        WEST,
        SOUTH_WEST,
        SOUTH_EAST
    }

    static float gridSize = 1.75f;
    public Hex playerHex;
    List<Hex> allHexes;

    [SerializeField]
    private GameObject groundPlane;

    [FoldoutGroup("Prefabs", true)]
    [BoxGroup("Prefabs/Layout")]
    [SerializeField, AssetsOnly]
    private GameObject floorHexFab;
    [BoxGroup("Prefabs/Layout")]
    [SerializeField, AssetsOnly]
    private GameObject wallHexFab;

    public void Setup(AbstractLevel level)
    {
        groundPlane.transform.localScale = new Vector3(level.layout.GetLength(0)+1, 1, level.layout.GetLength(1)+1);
        Hex[,] hexes = new Hex[level.layout.GetLength(0), level.layout.GetLength(1)];
        allHexes = new List<Hex>();
        for(int row = 0; row < level.layout.GetLength(1); row++)
        {
            for(int col = 0; col < level.layout.GetLength(0); col++)
            {
                if(level.layout[col, row] != AbstractLevel.LevelHex.EMPTY)
                {
                    Vector3 location = transform.position + col * GetDirVector(HexDirection.EAST) + (row / 2) * GetDirVector(HexDirection.SOUTH_EAST) + Mathf.Ceil(row / 2f) * GetDirVector(HexDirection.SOUTH_WEST);
                    GameObject temp = Instantiate(floorHexFab, location, Quaternion.identity, transform);
                    Hex tempHex = temp.GetComponent<Hex>();
                    tempHex.Init(col, row);
                    hexes[col, row] = tempHex;
                    allHexes.Add(hexes[col, row]);
                    if(level.layout[col,row] == AbstractLevel.LevelHex.WALL)
                    {
                        tempHex.occupant = Instantiate(wallHexFab, temp.transform.position, Quaternion.identity, temp.transform);
                    }
                    if (level.content[col, row] != null)
                    {
                        GameObject occupant = Instantiate(level.content[col, row], temp.transform.position, Quaternion.identity, temp.transform);
                        tempHex.occupant = occupant;
                        if (occupant.CompareTag("PlayerSpawn"))
                        {
                            playerHex = tempHex;
                            occupant.GetComponent<CombatantController>().tile = tempHex;
                        }
                        else if (occupant.CompareTag("Enemy"))
                        {
                            occupant.GetComponent<CombatantController>().tile = tempHex;
                        }
                        
                    }
                }
            }
        }

        //Link Neighbors
        for (int row = 0; row < hexes.GetLength(1); row++)
        {
            int offset = row % 2 == 0 ? 1 : 0;
            for (int col = 0; col < hexes.GetLength(0); col++)
            {
                if(hexes[col,row] != null)
                {
                    
                    try
                    {
                        if(hexes[col + 1, row] != null)
                        {
                            hexes[col, row].neighbors.Add(hexes[col + 1, row]);
                        }
                    }
                    catch { }
                    try
                    {
                        if (hexes[col + offset, row - 1] != null)
                        {
                            hexes[col, row].neighbors.Add(hexes[col + offset, row - 1]);
                        }
                    }
                    catch { }
                    try
                    {
                        if (hexes[col - 1 + offset, row - 1] != null)
                        {
                            hexes[col, row].neighbors.Add(hexes[col - 1 + offset, row - 1]);
                        }
                    }
                    catch { }
                    try
                    {
                        if (hexes[col - 1, row] != null)
                        {
                            hexes[col, row].neighbors.Add(hexes[col - 1, row]);
                        }
                    }
                    catch { }
                    try
                    {
                        if (hexes[col - 1 + offset, row + 1] != null)
                        {
                            hexes[col, row].neighbors.Add(hexes[col - 1 + offset, row + 1]);
                        }
                    }
                    catch { }
                    try
                    {
                        if (hexes[col + offset, row + 1] != null)
                        {
                            hexes[col, row].neighbors.Add(hexes[col + offset, row + 1]);
                        }
                    }
                    catch { }
                }
            }
        }

        // Determine which hexes need torches        
        for(int row = 0; row < level.layout.GetLength(1); row++)
        {
            for(int col = 0; col < level.layout.GetLength(0); col++)
            {
                if(level.layout[col,row] == AbstractLevel.LevelHex.WALL)
                {
                    if (hexes.GetLength(0) > col + 1 && level.layout[col+1,row] == AbstractLevel.LevelHex.FLOOR &&
                        !hexes[col+1,row].neighbors.Exists(h => level.layout[h.col,h.row] == AbstractLevel.LevelHex.WALL && h != hexes[col,row])) {
                        hexes[col,row].SpawnRightTorch();
                    }

                    if (col > 0 && level.layout[col-1,row] == AbstractLevel.LevelHex.FLOOR &&
                        !hexes[col-1,row].neighbors.Exists(h => level.layout[h.col,h.row] == AbstractLevel.LevelHex.WALL && h != hexes[col,row])) {
                        hexes[col,row].SpawnLeftTorch();
                    }
                }
            }
        }

        BakeLevelFromPlayerMovement();
    }

    //Use this if the player moves or is moved
    public void BakeLevelFromPlayerMovement(Hex playerHex = null)
    {
        if (playerHex != null)
            this.playerHex = playerHex;
        UpdatePlayerDistances();
        UpdatePathDistances();
        UpdateLOSRaycasts();
        UpdateLOSDistances();
    }

    //Use this if an enemy moves/is moved or is created/destroyed mid round
    public void BakeLevelFromEnemyMovement()
    {
        UpdatePathDistances();
        UpdateLOSDistances();
    }

    //Use this if an obstacle is created, destroyed, or moved
    public void BakeLevelFromObstacleChange()
    {
        UpdateLOSRaycasts();
        UpdatePathDistances();
        UpdateLOSDistances();
    }

    private void UpdatePlayerDistances()
    {
        if (playerHex == null) return;
        List<Hex> queue = new List<Hex>();
        foreach(Hex hex in allHexes)
        {
            hex.visited = false;
            hex.playerDistance = int.MaxValue;
        }
        playerHex.playerDistance = 0;
        queue.Add(playerHex);
        playerHex.visited = true;
        while (queue.Count > 0)
        {
            queue.Sort(new Hex.PlayerDistanceAscending());
            Hex hex = queue[0];
            queue.RemoveAt(0);
            foreach(Hex other in hex.neighbors)
            {
                other.playerDistance = Mathf.Min(other.playerDistance, hex.playerDistance + 1);
                if (!other.visited)
                {
                    queue.Add(other);
                    other.visited = true;
                }
            }
        }
    }

    private void UpdatePathDistances()
    {
        if (playerHex == null) return;
        List<Hex> queue = new List<Hex>();
        foreach (Hex hex in allHexes)
        {
            hex.visited = false;
            hex.pathDistance = int.MaxValue;
        }
        playerHex.pathDistance = 0;
        queue.Add(playerHex);
        playerHex.visited = true;
        while(queue.Count > 0)
        {
            queue.Sort(new Hex.PathDistanceAscending());
            Hex hex = queue[0];
            queue.RemoveAt(0);
            foreach (Hex other in hex.neighbors)
            {
                other.pathDistance = Mathf.Min(other.pathDistance, hex.pathDistance + 1);
                if (!other.visited && other.occupant == null)
                {
                    queue.Add(other);
                    other.visited = true;
                }
            }
        }
    }

    private void UpdateLOSDistances()
    {
        List<Hex> queue = new List<Hex>();
        foreach (Hex hex in allHexes)
        {
            if (hex.inSight)
            {
                hex.sightDistance = 0;
                queue.Add(hex);
                hex.visited = true;
            }
            else
            {
                hex.sightDistance = int.MaxValue;
            }

        }
        while (queue.Count > 0)
        {
            queue.Sort(new Hex.SightDistanceAscending());
            Hex hex = queue[0];
            queue.RemoveAt(0);
            foreach (Hex other in hex.neighbors)
            {
                other.sightDistance = Mathf.Min(other.sightDistance, hex.sightDistance + 1);
                if (!other.visited && other.occupant == null)
                {
                    queue.Add(other);
                    other.visited = true;
                }
            }
        }
    }

    private void UpdateLOSRaycasts()
    {
        if (playerHex == null) return;
        foreach (Hex hex in allHexes)
        {
            if (playerHex.canSeeHexCorner(hex))
            {
                hex.inSight = true;
            }
            else
            {
                hex.inSight = false;
            }
        }
    }

    public static Vector3 GetDirVector(HexDirection dir)
    {
        Vector3 directionVec = Vector3.zero;
        switch (dir)
        {
            case HexDirection.EAST:
                directionVec = Vector3.right;
                break;
            case HexDirection.NORTH_EAST:
                directionVec = Mathf.Cos(60 * Mathf.Deg2Rad) * Vector3.right + Mathf.Sin(60 * Mathf.Deg2Rad) * Vector3.forward;
                break;
            case HexDirection.NORTH_WEST:
                directionVec = -Mathf.Cos(60 * Mathf.Deg2Rad) * Vector3.right + Mathf.Sin(60 * Mathf.Deg2Rad) * Vector3.forward;
                break;
            case HexDirection.WEST:
                directionVec = -Vector3.right;
                break;
            case HexDirection.SOUTH_WEST:
                directionVec = -Mathf.Cos(60 * Mathf.Deg2Rad) * Vector3.right + -Mathf.Sin(60 * Mathf.Deg2Rad) * Vector3.forward;
                break;
            case HexDirection.SOUTH_EAST:
                directionVec = Mathf.Cos(60 * Mathf.Deg2Rad) * Vector3.right + -Mathf.Sin(60 * Mathf.Deg2Rad) * Vector3.forward;
                break;
        }
        return gridSize * directionVec;
    }
}
