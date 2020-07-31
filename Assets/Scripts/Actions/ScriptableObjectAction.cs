using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class ScriptableObjectAction : ScriptableObject, AbstractAction
{
    [Space, AssetList, InlineEditor(InlineEditorObjectFieldModes.Foldout), PropertyOrder(100)]
    public AbstractLevel level;

    public virtual IEnumerator Run() {
        LevelManager.Instance.SetLevel(level);
        yield return null;
    }
}
