using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTableController : MonoBehaviour
{
    float spacingHorizontal = 0.24f;
    float spacingVertical;

    public GameObject tileFab;
    public GameObject pawnFab;
    public Transform tileRoot;

    GameObject previousRoomTile;
    // Start is called before the first frame update
    void Start()
    {
        MapRoom[][] map = DungeonMapManager.Instance.GetMap();
        MapRoom previousRoom = DungeonMapManager.Instance.activeRoom;
        previousRoom.Visit();
        Setup(map, previousRoom);
    }

    public void Setup(MapRoom[][] map, MapRoom previous)
    {
        spacingVertical = Mathf.Cos(30 * Mathf.Deg2Rad) * spacingHorizontal;
        int yOffset = 3;
        for (int row = 0; row < 7; row++)
        {
            int y = yOffset - row;
            int rowWidth = 7 - Mathf.Abs(y);
            float zPos = y * spacingVertical;
            float xPos = ((rowWidth - 1) / 2.0f) * -spacingHorizontal;
            for (int col = 0; col < rowWidth; col++)
            {
                GameObject tile = Instantiate(tileFab, tileRoot);
                MapTableTile tileScript = tile.GetComponent<MapTableTile>();
                tileScript.room = map[row][col];
                if (tileScript.room == previous)
                    previousRoomTile = tileScript.gameObject;
                tile.transform.localPosition = new Vector3(xPos, 0, zPos);
                tile.transform.localRotation = Quaternion.Euler(new Vector3(0, 30, 0));
                tileScript.Setup();
                xPos += spacingHorizontal;
            }
        }
        MapPawnController pawn = Instantiate(pawnFab).GetComponent<MapPawnController>();
        pawn.controller = this;
        ResetPawn(pawn);
    }

    public void Select(MapTableTile tile, MapPawnController pawn)
    {
        if(tile == null)
        {
            ResetPawn(pawn);
        }
        else
        {
            if(tile.room.content == MapRoom.RoomType.EMPTY || tile.room.visited || !tile.room.available)
            {
                ResetPawn(pawn);
            }
            else
            {
                pawn.locked = true;
                MovePawn(pawn, tile.transform);
                Destroy(FindObjectOfType<MapScreenController>().toyInstance);
                Debug.Log(tile.room.content);
                DungeonMapManager.Instance.LaunchRoom(tile.room);
            }
        }
    }

    void ResetPawn(MapPawnController pawn)
    {
        MovePawn(pawn, previousRoomTile.transform);
    }

    void MovePawn(MapPawnController pawn, Transform destination)
    {
        pawn.transform.SetParent(destination);
        pawn.transform.localPosition = Vector3.zero;
        pawn.transform.rotation = Quaternion.identity;
        pawn.locked = false;
    }

    
}
