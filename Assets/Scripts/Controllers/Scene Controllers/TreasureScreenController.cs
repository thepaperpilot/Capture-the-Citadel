using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TreasureScreenController : MonoBehaviour, IScreenSelector
{
    [SerializeField, AssetList, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    private AbstractLevel level;
    [SerializeField, AssetList(Path = "Prefabs/Toys", CustomFilterMethod = "FindToy")]
    [InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Foldout)]
    private GameObject toy;
    [SerializeField, ValueDropdown("FindScenes")]
    private string nextScene;
    [MinMaxSlider(0, 200, true)]
    public Vector2Int goldRange = new Vector2Int(50, 75);

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.Instance.SetLevel(level);

        ActionsManager.Instance.AddToTop(new GainGoldAction(Random.Range(goldRange.x, goldRange.y)));

        RelicPedestalController pedestal = FindObjectOfType<RelicPedestalController>();
        AbstractRelic rewardRelic = RelicsManager.Instance.GetNewRelic();
        if (rewardRelic == null)
        {
            Destroy(pedestal.gameObject);
        }
        else
        {
            pedestal.Init(rewardRelic);
        }

        SceneSelectorController sceneSelector =
            LevelManager.Instance.GetComponentInChildren<SceneSelectorController>();
        if (sceneSelector != null)
            sceneSelector.Setup(nextScene, this);

        GameObject toyInstance = Instantiate(toy);
        PlayerManager.Instance.Grab(toyInstance);
    }

    public void SelectScene(string scene)
    {
        ChangeSceneAction sceneChange = new ChangeSceneAction(scene);
        ActionsManager.Instance.AddToBottom(sceneChange);
    }
#if UNITY_EDITOR
    private bool FindToy(GameObject obj)
    {
        return obj.GetComponentInChildren<Toy>() != null;
    }

    private IEnumerable FindScenes()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        ValueDropdownItem[] scenes = new ValueDropdownItem[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            scenes[i] = new ValueDropdownItem(name, name);
        }
        return scenes;
    }
#endif
}
