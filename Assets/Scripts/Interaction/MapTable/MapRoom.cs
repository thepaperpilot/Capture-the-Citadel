using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoom
{
    public enum RoomType
    {
        EMPTY,
        START,
        REST,
        SHOP,
        CHEST,
        EVENT,
        ENEMY,
        ELITE,
        BOSS
    }

    public List<MapRoom> adjacentRooms;
    public bool visited;
    public bool available;
    public RoomType content;

    public MapRoom()
    {
        visited = false;
        available = false;
        adjacentRooms = new List<MapRoom>();
        content = RoomType.EMPTY;
    }

    public void Visit()
    {
        visited = true;
        foreach(MapRoom adj in adjacentRooms)
        {
            adj.available = true;
        }
    }
}
