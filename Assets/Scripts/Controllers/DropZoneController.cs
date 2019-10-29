using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DropZoneController : MonoBehaviour
{
    [FoldoutGroup("Components")]
    [SerializeField, ChildGameObjectsOnly]
    private SpriteRenderer image;
    [FoldoutGroup("Components")]
    [SerializeField, ChildGameObjectsOnly]
    private SpriteRenderer pad;
    
    public void Setup(AbstractRelic.RelicAction action) {
        image.sprite = action.sprite;
        pad.color = action.color;
    }
}
