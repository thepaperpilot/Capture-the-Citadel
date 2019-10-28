using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [FoldoutGroup("Components")]
    [SerializeField, ChildGameObjectsOnly]
    private MeshRenderer back;
    [FoldoutGroup("Components")]
    [SerializeField, ChildGameObjectsOnly]
    private MeshRenderer front;
    [FoldoutGroup("Components")]
    [SerializeField, ChildGameObjectsOnly]
    private MeshRenderer border;
    [FoldoutGroup("Components")]
    [SerializeField, ChildGameObjectsOnly]
    new private TextMeshPro name;
    [FoldoutGroup("Components")]
    [SerializeField, ChildGameObjectsOnly]
    private SpriteRenderer image;
    [FoldoutGroup("Components")]
    [SerializeField, ChildGameObjectsOnly]
    private TextMeshPro description;
    [FoldoutGroup("Components")]
    [SerializeField, ChildGameObjectsOnly]
    private TextMeshPro cost;

    [FoldoutGroup("Card Borders", true)]
    [HorizontalGroup("Card Borders/h")]
    [BoxGroup("Card Borders/h/Common")]
    [SerializeField, AssetsOnly, HideLabel, PreviewField(ObjectFieldAlignment.Center)]
    private Material common;
    [BoxGroup("Card Borders/h/Uncommon")]
    [SerializeField, AssetsOnly, HideLabel, PreviewField(ObjectFieldAlignment.Center)]
    private Material uncommon;
    [BoxGroup("Card Borders/h/Rare")]
    [SerializeField, AssetsOnly, HideLabel, PreviewField(ObjectFieldAlignment.Center)]
    private Material rare;
    
    public void Setup(AbstractCard card) {
        switch (card.rarity) {
            case AbstractCard.Rarities.COMMON:
                border.material = common;
                break;
            case AbstractCard.Rarities.UNCOMMON:
                border.material = uncommon;
                break;
            case AbstractCard.Rarities.RARE:
                border.material = rare;
                break;
        }
#if UNITY_EDITOR
        /* TextMeshPro components weren't showing up in the Preview Renderer so I tried adding TextMesh components instead, but those didn't show up either :/ */
        /*
        GameObject namePreview = new GameObject("Name Preview");
        namePreview.transform.SetParent(name.transform);
        namePreview.transform.localPosition = Vector3.zero;
        namePreview.transform.localRotation = Quaternion.identity;
        namePreview.transform.localScale = new Vector3(.1f, .1f, 1);

        TextMesh namePreviewText = namePreview.AddComponent<TextMesh>();
        namePreviewText.text = card.name;
        namePreviewText.anchor = TextAnchor.MiddleCenter;
        
        GameObject descPreview = new GameObject("Description Preview");
        descPreview.transform.SetParent(description.transform);
        descPreview.transform.localPosition = Vector3.zero;
        descPreview.transform.localRotation = Quaternion.identity;
        descPreview.transform.localScale = new Vector3(.1f, .1f, 1);
        
        TextMesh descPreviewText = descPreview.AddComponent<TextMesh>();
        descPreviewText.text = card.description;
        descPreviewText.anchor = TextAnchor.MiddleCenter;
        */
#endif
        name.text = card.name;
        image.sprite = card.image;
        description.text = card.description;
        cost.text = card.energyCost.ToString();
    }
}
