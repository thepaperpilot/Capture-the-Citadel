using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class SceneSelectorController : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly]
    private TextMeshPro sign;

    private string scene;
    private IScreenSelector controller;

    void Update()
    {
        transform.localRotation = Quaternion.LookRotation(transform.position - PlayerManager.Instance.GetHeadsetPos());
    }

    public void Setup(string scene, IScreenSelector controller) {
        sign.text = scene;
        this.scene = scene;
        this.controller = controller;
    }

    public void Select() {
        controller.SelectScene(scene);
    }
}
