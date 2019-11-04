using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : SerializedMonoBehaviour
{
    public static LevelManager Instance;

    [AssetList(Path="Prefabs", CustomFilterMethod="FindLevelControllers")]
    [InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Hidden)]
    [HideInPlayMode]
    public GameObject levelPrefab;
    public LevelController controller;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(this);
        }
    }

    public void SetLevel(AbstractLevel level) {
        ClearLevel();

        GameObject levelGObject = Instantiate(levelPrefab, transform);
        controller = levelGObject.GetComponent<LevelController>();
        controller.Setup(level);

        PlayerController playerController = levelGObject.GetComponentInChildren<PlayerController>();
        Transform playerTransform = playerController.transform;
        PlayerManager.Instance.MovePlayer(playerTransform.position, playerTransform.rotation);
        Destroy(playerController.model);
        foreach (EnemyController enemy in levelGObject.GetComponentsInChildren<EnemyController>())
            enemy.transform.LookAt(playerTransform);
    }

    public void ClearLevel()
    {
        while (transform.childCount > 0)
            Destroy(transform.GetChild(0));
    }

#if UNITY_EDITOR
    private bool FindLevelControllers(GameObject obj) {
        return obj.GetComponentInChildren<LevelController>() != null;
    }
#endif
}
