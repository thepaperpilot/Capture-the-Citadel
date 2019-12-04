using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneActionsController : MonoBehaviour
{
    [InfoBox("These are the actions that are randomly chosen between when entering a new floor")]
    [SerializeField, AssetSelector(FlattenTreeView = true, ExcludeExistingValuesInList = true)]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    private ScriptableObjectAction[] randomActions = new ScriptableObjectAction[0];

    public void DoRandomAction()
    {
        ActionsManager.Instance.AddToBottom(randomActions[UnityEngine.Random.Range(0, randomActions.Length)]);
    }
}
