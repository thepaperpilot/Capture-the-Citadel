using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName="Enemies/Generic Enemy")]
public class AbstractEnemy : ScriptableObject
{
    [AssetList(Path="Assets/Prefabs/Enemies"), InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Hidden)]
    public GameObject enemyPrefab;
    public int health;
}
