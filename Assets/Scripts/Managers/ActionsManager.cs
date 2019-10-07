using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActionsManager : MonoBehaviour
{
    public static ActionsManager Instance;

    private List<AbstractAction> actions = new List<AbstractAction>();
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
    }

    private void ResetActions(Scene scene, LoadSceneMode mode) {
        actions.RemoveRange(0, actions.Count);
        acting = false;
        StopAllCoroutines();
    }

    private void NextAction() {
        AbstractAction action = actions.First();
        actions.RemoveAt(0);
        acting = true;
        StartCoroutine(RunAction(action));
    }

    public void AddToTop(AbstractAction action) {
        actions.Prepend(action);
        if (!acting)
            NextAction();
    }

    public void AddToBottom(AbstractAction action) {
        actions.Append(action);
        if (!acting)
            NextAction();
    }

    private IEnumerator RunAction(AbstractAction action) {
        yield return StartCoroutine(action.Run());
        acting = false;
        if (actions.Count > 0)
            NextAction();
    }
}
