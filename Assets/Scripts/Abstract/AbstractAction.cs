using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class AbstractAction : ScriptableObject
{
    [AssetSelector(FlattenTreeView=true, DrawDropdownForListElements=false, ExcludeExistingValuesInList=true)]
    [Space, PropertyOrder(100), GUIColor(0, 1, 1)]
    public AbstractAction[] chainedEvents;
    public virtual IEnumerator Run() {
        yield return null;
    }
}
