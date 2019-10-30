using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Combats/Generic Combat")]
public class AbstractCombat : ScriptableObjectAction
{
    public enum RelicRewards {
        NO_RELIC,
        RANDOM_RELIC,
        SET_RARITY,
        SET_RELIC
    }

    private IEnumerable Rarities = new ValueDropdownList<AbstractRelic.Rarities> {
        { "Common", AbstractRelic.Rarities.COMMON },
        { "Uncommon", AbstractRelic.Rarities.UNCOMMON },
        { "Rare", AbstractRelic.Rarities.RARE },
        { "Boss", AbstractRelic.Rarities.BOSS },
        { "Shop", AbstractRelic.Rarities.SHOP }
    };

    [BoxGroup("Rewards")]
    [MinMaxSlider(0, 200, true)]
    public Vector2Int goldRange = new Vector2Int(50, 75);
    [BoxGroup("Rewards")]
    public RelicRewards relicReward;
    [BoxGroup("Rewards"), ShowIf("relicReward", RelicRewards.SET_RARITY), ValueDropdown("Rarities")]
    public AbstractRelic.Rarities relicRarity;
    [BoxGroup("Rewards"), ShowIf("relicReward", RelicRewards.SET_RELIC), AssetList, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    public AbstractRelic relic;

    public override IEnumerator Run()
    {
        yield return base.Run();
        CombatManager.Instance.StartCombat(this);
        yield return null;
    }
}
