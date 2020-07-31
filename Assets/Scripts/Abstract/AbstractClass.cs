using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Classes/Generic Class")]
public class AbstractClass : ScriptableObject
{
    public int startingHealth;
    public int startingGold = 99;
    public int baseEnergyPerTurn = 3;
    [Space, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [AssetSelector(FlattenTreeView=true, DrawDropdownForListElements=false, IsUniqueList=false)]
    public List<AbstractCard> startingDeck;
    [Space, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [AssetSelector(FlattenTreeView=true, DrawDropdownForListElements=false)]
    public List<AbstractCard> cardPool;
    [Space, AssetList, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    public AbstractRelic startingRelic;
    [Space, AssetList, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    public AbstractCard card;

    public AbstractCard.ClassColor color;
}
