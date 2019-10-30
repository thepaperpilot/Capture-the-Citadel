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
    Hex playerHex;

    [FoldoutGroup("Prefabs", true)]
    [BoxGroup("Prefabs/Hexes")]
    [SerializeField, AssetsOnly]
    private GameObject floorHex;
    [BoxGroup("Prefabs/Hexes")]
    [SerializeField, AssetsOnly]
    private GameObject wallHex;

    public void Setup(AbstractLevel level)
    {
        Hex[,] hexes = new Hex[level.layout.GetLength(0), level.layout.GetLength(1)];
        for(int row = 0; row < level.layout.GetLength(1); row++)
        {
            for(int col = 0; col < level.layout.GetLength(0); col++)
            {
                if(level.layout[col, row] != AbstractLevel.LevelHex.EMPTY)
                {
                    Vector3 location = transform.position + col * GetDirVector(HexDirection.EAST) + (row / 2) * GetDirVector(HexDirection.SOUTH_EAST) + Mathf.Ceil(row / 2f) * GetDirVector(HexDirection.SOUTH_WEST);
                    GameObject temp = Instantiate(floorHex, location, Quaternion.identity, transform);
                    hexes[col, row] = temp.GetComponent<Hex>();
                    if(level.layout[col,row] == AbstractLevel.LevelHex.WALL)
                    {
                        hexes[col, row].occupant = Instantiate(wallHex, temp.transform.position, Quaternion.identity, temp.transform);
                    }
                }
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
