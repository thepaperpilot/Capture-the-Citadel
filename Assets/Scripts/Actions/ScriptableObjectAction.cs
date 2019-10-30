using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class ScriptableObjectAction : ScriptableObject, AbstractAction
{
    [Space, AssetList, InlineEditor(InlineEditorObjectFieldModes.Foldout), PropertyOrder(100)]
    public AbstractLevel level;

    public virtual IEnumerator Run() {
        while (ActionsManager.Instance.transform.childCount > 0)
            Destroy(ActionsManager.Instance.transform.GetChild(0));

        GameObject levelGObject = Instantiate(ActionsManager.Instance.levelPrefab, ActionsManager.Instance.transform);
        levelGObject.GetComponent<LevelController>().Setup(level);
        // TODO put player in player spawn
        yield return null;
    }
}
