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
    [HideInEditorMode]
    public LevelController controller;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void SetLevel(AbstractLevel level) {
        ClearLevel();

        GameObject levelGObject = Instantiate(levelPrefab, transform);
        levelGObject.transform.position = Vector3.zero;
        controller = levelGObject.GetComponent<LevelController>();
        controller.Setup(level);

        PlayerController playerController = levelGObject.GetComponentInChildren<PlayerController>();
        Transform playerTransform = playerController.transform;
        if (level.flipPlayerStartDirection)
            playerTransform.Rotate(Vector3.up, 180);
        PlayerManager.Instance.MovePlayer(playerTransform.position, playerTransform.rotation);
        Destroy(playerController.model);
        foreach (EnemyController enemy in levelGObject.GetComponentsInChildren<EnemyController>())
            enemy.transform.LookAt(playerTransform);
    }

    public void ClearLevel()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

#if UNITY_EDITOR
    private bool FindLevelControllers(GameObject obj) {
        return obj.GetComponentInChildren<LevelController>() != null;
    }
#endif
}
