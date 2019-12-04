using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampfireScreenController : MonoBehaviour
{
    public enum CampfireAction
    {
        REST,
        UPGRADE
    }

    [SerializeField, AssetList, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    private AbstractLevel level;
    [SerializeField, AssetList(Path = "Prefabs/Toys", CustomFilterMethod = "FindToy")]
    [InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Foldout)]
    private GameObject toy;

    void Start()
    {
        CampfireAction[] actions = new CampfireAction[] { CampfireAction.REST, CampfireAction.UPGRADE };
        LevelManager.Instance.SetLevel(level);
        CampfireSelectorController[] controllers =
            LevelManager.Instance.GetComponentsInChildren<CampfireSelectorController>();
        for (int i = 0; i < controllers.Length && i < actions.Length; i++) {
            controllers[i].Setup(actions[i], this);
        }
        GameObject toyInstance = Instantiate(toy);
        PlayerManager.Instance.Grab(toyInstance);
    }

    public void SelectAction(CampfireAction action)
    {
        switch (action)
        {
            case CampfireAction.REST:
                PlayerManager.Instance.NonCombatHeal(GetHealAmount());
                break;
            case CampfireAction.UPGRADE:
                //do nothing
                break;
        }
        ChangeSceneAction sceneChange = new ChangeSceneAction("Map");
        ActionsManager.Instance.AddToBottom(sceneChange);
    }

    public int GetHealAmount()
    {
        return Mathf.CeilToInt(PlayerManager.Instance.maxHealth / 3.0f);
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
