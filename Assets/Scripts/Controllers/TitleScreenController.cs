using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    [SerializeField, AssetList, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    private AbstractLevel level;
    [SerializeField, Space, AssetList(AutoPopulate = true), InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    private AbstractClass[] classes;
    [SerializeField, AssetList(Path="Prefabs/Toys", CustomFilterMethod="FindToy")]
    [InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Foldout)]
    private GameObject toy;
    [SerializeField, ValueDropdown("FindScenes")]
    private string mapScene;

    void Start() {
        LevelManager.Instance.SetLevel(level);
        ClassSelectorController[] controllers =
            LevelManager.Instance.GetComponentsInChildren<ClassSelectorController>();
        Debug.Log("Controllers: " + controllers.Length + " Classes: " + classes.Length);
        for (int i = 0; i < controllers.Length && i < classes.Length; i++) {
            controllers[i].Setup(classes[i], this);
        }
        GameObject toyInstance = Instantiate(toy);
        PlayerManager.Instance.Grab(toyInstance);
    }

    public void SelectClass(AbstractClass selectedClass) {
        PlayerManager.Instance.SetClass(selectedClass);
        CardsManager.Instance.deck = new List<AbstractCard>(selectedClass.startingDeck.ToArray());
        RelicsManager.Instance.ResetRelics();
        RelicsManager.Instance.AddRelic(new RelicsManager.RelicData { relic = selectedClass.startingRelic });
        ChangeSceneAction sceneChange = new ChangeSceneAction(mapScene);
        ActionsManager.Instance.AddToBottom(sceneChange);
    }

#if UNITY_EDITOR
    private bool FindToy(GameObject obj) {
        return obj.GetComponentInChildren<Toy>() != null;
    }

    private IEnumerable FindScenes() {
        int sceneCount = SceneManager.sceneCountInBuildSettings;     
        ValueDropdownItem[] scenes = new ValueDropdownItem[sceneCount];
        for( int i = 0; i < sceneCount; i++ ) {
            string name = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            scenes[i] = new ValueDropdownItem(name, name);
        }
        return scenes;
    }
#endif
}
