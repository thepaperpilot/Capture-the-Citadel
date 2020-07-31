using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class EventInfoController : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly]
    private TextMeshPro titleText;
    [SerializeField, ChildGameObjectsOnly]
    private TextMeshPro descriptionText;

    public void Setup(AbstractEvent e) {
        titleText.text = e.title;
        descriptionText.text = e.description;
    }
}
