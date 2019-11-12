using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Classes/Generic Class")]
public class AbstractClass : ScriptableObject
{
    public int startingHealth;
    [Space, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [AssetSelector(FlattenTreeView=true, DrawDropdownForListElements=false, IsUniqueList=false)]
    public List<AbstractCard> startingDeck;
    [Space, AssetList, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    public AbstractRelic startingRelic;
    [Space, AssetList, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    public AbstractCard card;
}
