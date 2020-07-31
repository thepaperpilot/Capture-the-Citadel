using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTableTile : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite combatTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite combatEliteTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite combatBossTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite eventTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite shopTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite treasureTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite restTex;

    [SerializeField, FoldoutGroup("Materials")]
    public Material unavailableMat;
    [SerializeField, FoldoutGroup("Materials")]
    public Material availableMat;
    [SerializeField, FoldoutGroup("Materials")]
    public Material visitedMat;
    [SerializeField, FoldoutGroup("Materials")]
    public Material emptyMat;

    public SpriteRenderer iconRenderer;
    public MeshRenderer hexRenderer;

    public MapRoom room;

    public void Setup()
    {
        if(room != null)
        {
            switch (room.content)
            {
                case MapRoom.RoomType.START:
                    iconRenderer.enabled = false;
                    hexRenderer.material = visitedMat;
                    return;
                case MapRoom.RoomType.EMPTY:
                    iconRenderer.enabled = false;
                    hexRenderer.material = emptyMat;
                    return;
                case MapRoom.RoomType.ENEMY:
                    iconRenderer.sprite = combatTex;
                    break;
                case MapRoom.RoomType.ELITE:
                    iconRenderer.sprite = combatEliteTex;
                    break;
                case MapRoom.RoomType.BOSS:
                    iconRenderer.sprite = combatBossTex;
                    break;
                case MapRoom.RoomType.EVENT:
                    iconRenderer.sprite = eventTex;
                    break;
                case MapRoom.RoomType.REST:
                    iconRenderer.sprite = restTex;
                    break;
                case MapRoom.RoomType.SHOP:
                    iconRenderer.sprite = shopTex;
                    break;
                case MapRoom.RoomType.CHEST:
                    iconRenderer.sprite = treasureTex;
                    break;
            }
            if (room.visited)
            {
                hexRenderer.material = visitedMat;
            }
            else if (room.available)
            {
                hexRenderer.material = availableMat;
            }
            else
            {
                hexRenderer.material = unavailableMat;
            }
        }
    }

#if UNITY_EDITOR
    [HideInInlineEditors, Button("Visit")]
    private void DebugVisit()
    {
        room.Visit();
        MapTableTile[] allTiles = FindObjectsOfType<MapTableTile>();
        foreach(MapTableTile tile in allTiles)
        {
            tile.Setup();
        }
    }

#endif
}
