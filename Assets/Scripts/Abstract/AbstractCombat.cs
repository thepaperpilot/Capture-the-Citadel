using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Combats/Generic Combat")]
public class AbstractCombat : ScriptableObject
{
    [AssetSelector(IsUniqueList=false, Paths="Assets/Enemies", FlattenTreeView=true), InlineEditor, Space]
    public AbstractEnemy[] enemies;
    [MinMaxSlider(0, 200, true)]
    public Vector2Int goldRange = new Vector2Int(50, 75);
    public bool hasRelic = false;
    public bool hasRareRelic = false;
}
