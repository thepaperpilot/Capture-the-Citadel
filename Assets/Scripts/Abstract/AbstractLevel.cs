using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Generic Level")]
public class AbstractLevel : SerializedScriptableObject
{
    public enum LevelHex
    {
        EMPTY = 0,
        WALL,
        FLOOR
    }

    new public string name = "untitled";

    [HideInInlineEditors, Range(1, 12), OnValueChanged("ResizeTable")]
    public int rows = 8;
    [HideInInlineEditors, Range(1, 12), OnValueChanged("ResizeTable")]
    public int columns = 8;

    [HideInInlineEditors]
    public bool flipPlayerStartDirection;

    [BoxGroup("Level Geometry")]
    [TableMatrix(HorizontalTitle = "X axis", VerticalTitle = "Y axis")]
    [ShowInInspector]public LevelHex[,] layout;

    [BoxGroup("Objects")]
    [TableMatrix(HorizontalTitle = "X axis", VerticalTitle = "Y axis")]
    [ShowInInspector]public GameObject[,] content;


#if UNITY_EDITOR
    [OnInspectorGUI("CheckPreview"), ShowInInspector, HideLabel, InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Hidden), Space]
    private GameObject preview;
    private GameObject nextPreview;

    private void CheckPreview()
    {
        if (nextPreview != null)
        {
            preview = nextPreview;
            nextPreview = null;
        }
        else if (preview == null)
            DrawPreview();
    }

    [HideInInlineEditors, Button("Regenerate Preview")]
    private void DrawPreview()
    {
        if (name == "") return;
        GameObject temp = UnityEditor.PrefabUtility.LoadPrefabContents("Assets/Prefabs/Level/Level.prefab");
        temp.GetComponent<LevelController>().Setup(this);
        preview = null;
        nextPreview = UnityEditor.PrefabUtility.SaveAsPrefabAsset(temp, "Assets/Editor/Previews/" + name + " Level (Preview).prefab");
        UnityEditor.PrefabUtility.UnloadPrefabContents(temp);
    }

    [HideInInlineEditors, Button("Reset")]
    private void Reset()
    {
        layout = new LevelHex[columns, rows];
        content = new GameObject[columns, rows];
    }

    private void ResizeTable()
    {
        LevelHex[,] newLayout = new LevelHex[columns, rows];
        GameObject[,] newContent = new GameObject[columns, rows];
        int maxRows = layout.GetLength(1);
        int maxColumns = layout.GetLength(0);
        for (int row = 0; row < maxRows; row++)
        {
            for(int col = 0; col < maxColumns; col++)
            {
                try
                {
                    newLayout[col, row] = layout[col, row];
                    newContent[col, row] = content[col, row];
                }
                catch { }
            }
        }

        layout = newLayout;
        content = newContent;
    }
#endif
}
