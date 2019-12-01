using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapScreenController : MonoBehaviour, IScreenSelector
{
    [SerializeField, AssetList, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    private AbstractLevel level;
    [SerializeField, AssetList(Path="Prefabs/Toys", CustomFilterMethod="FindToy")]
    [InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Foldout)]
    private GameObject toy;
    [SerializeField, ValueDropdown("FindScenes")]
    private string[] scenes;

    void Start() {
        LevelManager.Instance.SetLevel(level);
        SceneSelectorController[] controllers =
            LevelManager.Instance.GetComponentsInChildren<SceneSelectorController>();
        for (int i = 0; i < controllers.Length && i < scenes.Length; i++) {
            controllers[i].Setup(scenes[i], this);
        }
        GameObject toyInstance = Instantiate(toy);
        PlayerManager.Instance.Grab(toyInstance);
    }

    public void SelectScene(string scene) {
        SceneManager.LoadScene(scene);
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
