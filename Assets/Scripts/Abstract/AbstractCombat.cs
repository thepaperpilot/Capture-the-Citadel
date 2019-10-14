using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Combats/Generic Combat")]
public class AbstractCombat : AbstractAction
{
    [AssetSelector(IsUniqueList=false, Paths="Assets/Enemies", FlattenTreeView=true), InlineEditor(InlineEditorObjectFieldModes.Foldout), Space]
    public AbstractEnemy[] enemies;

    [BoxGroup("Rewards")]
    [MinMaxSlider(0, 200, true)]
    public Vector2Int goldRange = new Vector2Int(50, 75);
    [BoxGroup("Rewards")]
    public bool hasRelic = false;
    [BoxGroup("Rewards"), ShowIf("@hasRelic")]
    public bool hasRareRelic = false;

    public override IEnumerator Run()
    {
        CombatManager.Instance.StartCombat(this);
        yield return null;
    }
}
