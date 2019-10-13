using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActionsManager : SerializedMonoBehaviour
{
    public static ActionsManager Instance;

    [InfoBox("These are the actions that are randomly chosen between when entering a new floor")]
    [SerializeField, AssetSelector(FlattenTreeView=true, ExcludeExistingValuesInList=true)]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    private AbstractAction[] randomActions = new AbstractAction[0];
    [SerializeField, HideInEditorMode]
    private List<AbstractAction> actions = new List<AbstractAction>();
    [SerializeField, HideInEditorMode]
    private bool acting = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(this);
        }
    }

    private void Start() {
        SceneManager.sceneLoaded += ResetActions;
        AddToBottom(GetRandomAction());
    }

    private void ResetActions(Scene scene, LoadSceneMode mode) {
        actions.RemoveRange(0, actions.Count);
        acting = false;
        StopAllCoroutines();
        AddToBottom(GetRandomAction());
    }

    private void NextAction() {
        AbstractAction action = actions.First();
        actions.RemoveAt(0);
        acting = true;
        StartCoroutine(RunAction(action));
    }

    public void AddToTop(AbstractAction action) {
        foreach (AbstractAction a in action.chainedEvents.Reverse())
        {
            actions.Prepend(a);
        }
        actions.Prepend(action);
        if (!acting)
            NextAction();
    }

    public void AddToBottom(AbstractAction action) {
        actions.Append(action);
        foreach (AbstractAction a in action.chainedEvents)
        {
            actions.Append(a);
        }
        if (!acting)
            NextAction();
    }

    public AbstractAction GetRandomAction() {
        return randomActions[Random.Range(0, randomActions.Length)];
    }

    private IEnumerator RunAction(AbstractAction action) {
        yield return StartCoroutine(action.Run());
        acting = false;
        if (actions.Count > 0)
            NextAction();
    }
}
