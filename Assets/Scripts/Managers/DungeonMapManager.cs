using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonMapManager : MonoBehaviour
{
    public static DungeonMapManager Instance;

    public MapRoom activeRoom;
    MapRoom[][] dungeonMap = null;

    //public bool implementedOnly = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public MapRoom[][] GetMap()
    {
        if(dungeonMap == null)
        {
            dungeonMap = GenerateMap();
        }
        return dungeonMap;
    }

    public void LaunchRoom(MapRoom room)
    {
        activeRoom = room;
        switch (room.content)
        {
            case MapRoom.RoomType.ENEMY:
                SelectScene("Combat");
                break;
            case MapRoom.RoomType.ELITE:
                //SelectScene("EliteCombat");
                break;
            case MapRoom.RoomType.BOSS:
                //SelectScene("BossCombat");
                break;
            case MapRoom.RoomType.EVENT:
                SelectScene("Event");
                break;
            case MapRoom.RoomType.CHEST:
                SelectScene("Treasure");
                break;
            case MapRoom.RoomType.REST:
                SelectScene("Rest");
                break;
            case MapRoom.RoomType.SHOP:
                SelectScene("Shop");
                break;

        }
    }

    public void SelectScene(string scene)
    {
        ChangeSceneAction sceneChange = new ChangeSceneAction(scene);
        ActionsManager.Instance.AddToBottom(sceneChange);
    }


    MapRoom[][] GenerateMap()
    {
        //Build room jagged array
        List<MapRoom> allRooms = new List<MapRoom>();
        MapRoom[][] map = new MapRoom[7][];
        int yOffset = 3;
        for (int row = 0; row < 7; row++)
        {
            int y = yOffset - row;
            int rowWidth = 7 - Mathf.Abs(y);
            map[row] = new MapRoom[rowWidth];
            for (int col = 0; col < rowWidth; col++)
            {
                map[row][col] = new MapRoom();
                allRooms.Add(map[row][col]);
            }
        }

        //Set adjacents
        for (int row = 0; row < 7; row++)
        {
            int aboveColOffset = row >= 4 ? 0 : -1;
            int belowColOffset = row >= 3 ? -1 : 0;
            for (int col = 0; col < map[row].Length; col++)
            {
                try
                {
                    map[row][col].adjacentRooms.Add(map[row - 1][col + aboveColOffset]);
                }
                catch { }
                try
                {
                    map[row][col].adjacentRooms.Add(map[row - 1][col + aboveColOffset + 1]);
                }
                catch { }
                try
                {
                    map[row][col].adjacentRooms.Add(map[row][col - 1]);
                }
                catch { }
                try
                {
                    map[row][col].adjacentRooms.Add(map[row][col + 1]);
                }
                catch { }
                try
                {
                    map[row][col].adjacentRooms.Add(map[row + 1][col + belowColOffset]);
                }
                catch { }
                try
                {
                    map[row][col].adjacentRooms.Add(map[row + 1][col + belowColOffset + 1]);
                }
                catch { }
            }
        }

        //Fill Rooms
        bool reject = false;
        MapRoom startingRoom = map[6][3];
        MapRoom bossRoom = map[0][0];
        int debugloops = 0;
        do
        {
            Debug.Log("Begin map generation");
            List<MapRoom> freeRooms = new List<MapRoom>(allRooms.ToArray());

            //Place Spawn Room
            startingRoom.content = MapRoom.RoomType.START;
            startingRoom.available = true;
            freeRooms.Remove(startingRoom);

            //Place Boss Room
            bossRoom.content = MapRoom.RoomType.BOSS;
            freeRooms.Remove(bossRoom);

            //Place the first chest
            List<MapRoom> chestCandidates = new List<MapRoom>();
            MapRoom[] chestNodes = new MapRoom[] { map[0][3], map[2][5] };
            foreach (MapRoom node in chestNodes)
            {
                chestCandidates.Add(node);
                foreach (MapRoom adj in node.adjacentRooms)
                {
                    if (!chestCandidates.Contains(adj))
                    {
                        chestCandidates.Add(adj);
                    }
                }

            }
            MapRoom chestRoom = chestCandidates[Random.Range(0, chestCandidates.Count)];
            chestRoom.content = MapRoom.RoomType.CHEST;
            freeRooms.Remove(chestRoom);

            //Place the second chest
            chestCandidates = new List<MapRoom>();
            chestNodes = new MapRoom[] { map[3][0], map[5][0] };
            foreach (MapRoom node in chestNodes)
            {
                chestCandidates.Add(node);
                foreach (MapRoom adj in node.adjacentRooms)
                {
                    if (!chestCandidates.Contains(adj))
                    {
                        chestCandidates.Add(adj);
                    }
                }

            }
            chestRoom = chestCandidates[Random.Range(0, chestCandidates.Count)];
            chestRoom.content = MapRoom.RoomType.CHEST;
            freeRooms.Remove(chestRoom);

            //Temporarily remove rooms too close to start & boss from the pool
            foreach (MapRoom adj in bossRoom.adjacentRooms)
            {
                freeRooms.Remove(adj);
            }
            foreach (MapRoom adj in startingRoom.adjacentRooms)
            {
                freeRooms.Remove(adj);
            }

            //place shops
            int shopCount = Random.Range(3, 5);
            List<MapRoom> shopCandidates = new List<MapRoom>(freeRooms.ToArray());
            for (int i = 0; i < shopCount; i++)
            {
                if (shopCandidates.Count == 0)
                    break;
                MapRoom shopRoom = shopCandidates[Random.Range(0, shopCandidates.Count)];
                shopRoom.content = MapRoom.RoomType.SHOP;
                freeRooms.Remove(shopRoom);
                shopCandidates.Remove(shopRoom);
                foreach (MapRoom adj in shopRoom.adjacentRooms)
                {
                    shopCandidates.Remove(adj);
                }
            }

            //Place elites
            int eliteCount = Random.Range(3, 5);
            List<MapRoom> eliteCandidates = new List<MapRoom>(freeRooms.ToArray());
            for (int i = 0; i < eliteCount; i++)
            {
                if (eliteCandidates.Count == 0)
                    break;
                MapRoom eliteRoom = eliteCandidates[Random.Range(0, eliteCandidates.Count)];
                eliteRoom.content = MapRoom.RoomType.ELITE;
                freeRooms.Remove(eliteRoom);
                eliteCandidates.Remove(eliteRoom);
                foreach (MapRoom adj in eliteRoom.adjacentRooms)
                {
                    eliteCandidates.Remove(adj);
                }
            }

            //place walls
            int wallCount = Random.Range(5, 8);
            for (int i = 0; i < wallCount; i++)
            {
                MapRoom wallRoom = freeRooms[Random.Range(0, freeRooms.Count)];
                wallRoom.content = MapRoom.RoomType.EMPTY;
                freeRooms.Remove(wallRoom);
            }

            //Add back in boss adjacent rooms
            foreach (MapRoom adj in bossRoom.adjacentRooms)
            {
                freeRooms.Add(adj);
            }
            //Guarantee one fire adjacent to boss
            List<MapRoom> fireCandidates = new List<MapRoom>(freeRooms.ToArray());
            MapRoom bossFireRoom = bossRoom.adjacentRooms[Random.Range(0, bossRoom.adjacentRooms.Count)];
            bossFireRoom.content = MapRoom.RoomType.REST;
            freeRooms.Remove(bossFireRoom);
            fireCandidates.Remove(bossFireRoom);
            foreach (MapRoom adj in bossFireRoom.adjacentRooms)
            {
                fireCandidates.Remove(adj);
            }
            //Place remaining fires
            int fireCount = Random.Range(4, 7) - 1;
            for (int i = 0; i < fireCount; i++)
            {
                if (fireCandidates.Count == 0)
                    break;
                MapRoom fireRoom = fireCandidates[Random.Range(0, fireCandidates.Count)];
                fireRoom.content = MapRoom.RoomType.REST;
                freeRooms.Remove(fireRoom);
                fireCandidates.Remove(fireRoom);
                foreach (MapRoom adj in fireRoom.adjacentRooms)
                {
                    fireCandidates.Remove(adj);
                }
            }

            //Place events
            int eventCount = freeRooms.Count / 2;
            for (int i = 0; i < eventCount; i++)
            {
                MapRoom eventRoom = freeRooms[Random.Range(0, freeRooms.Count)];
                eventRoom.content = MapRoom.RoomType.EVENT;
                freeRooms.Remove(eventRoom);
            }

            //Add back in start adjacent rooms
            foreach (MapRoom adj in startingRoom.adjacentRooms)
            {
                freeRooms.Add(adj);
            }

            //Place enemies
            while (freeRooms.Count > 0)
            {
                freeRooms[0].content = MapRoom.RoomType.ENEMY;
                freeRooms.RemoveAt(0);
            }

            //Reject map if the boss room is not reachable
            List<MapRoom> reachable = new List<MapRoom>() { startingRoom };
            List<MapRoom> queue = new List<MapRoom>() { startingRoom };
            while (queue.Count > 0)
            {
                MapRoom current = queue[0];
                queue.RemoveAt(0);
                foreach (MapRoom adj in current.adjacentRooms)
                {
                    if (adj.content != MapRoom.RoomType.EMPTY && !reachable.Contains(adj))
                    {
                        reachable.Add(adj);
                        queue.Add(adj);
                    }
                }
            }
            reject = false;
            List<MapRoom> unreachable = allRooms.Except(reachable).ToList();
            foreach (MapRoom room in unreachable)
            {
                if (room.content != MapRoom.RoomType.EMPTY)
                    reject = true;
            }
            if (reject)
            {
                Debug.Log("Map rejected!");
                if (debugloops > 10)
                    reject = !reachable.Contains(bossRoom);
                debugloops++;
            }
        } while (reject);
        Debug.Log("Map generation complete!");
        activeRoom = startingRoom;
        return map;
    }
}
