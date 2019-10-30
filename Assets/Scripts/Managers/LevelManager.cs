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

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(this);
        }
    }

    public void SetLevel(AbstractLevel level) {
        while (transform.childCount > 0)
            Destroy(transform.GetChild(0));

        GameObject levelGObject = Instantiate(levelPrefab, transform);
        levelGObject.GetComponent<LevelController>().Setup(level);

        Transform playerTransform = levelGObject.GetComponentInChildren<PlayerController>().transform;
        PlayerManager.Instance.MovePlayer(playerTransform.position, playerTransform.rotation);
    }

#if UNITY_EDITOR
    private bool FindLevelControllers(GameObject obj) {
        return obj.GetComponentInChildren<LevelController>() != null;
    }
#endif
}
