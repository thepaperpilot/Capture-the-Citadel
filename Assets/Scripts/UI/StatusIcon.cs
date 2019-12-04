using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusIcon : MonoBehaviour
{
    public SpriteRenderer sprite;
    public TextMeshPro amountText;

    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite strengthTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite defenseTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite swiftnessTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite aegisTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite fortifyTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite hamstringTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite poisonTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite spikesTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite prayerFatigueTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite sabotageTex;
    [SerializeField, FoldoutGroup("Sprites")]
    public Sprite handyTex;

    public void Init(Status status)
    {
        sprite.sprite = GetIcon(status.name);
        amountText.text = status.amount.ToString();
    }

    private Sprite GetIcon(Status.Name name)
    {
        switch (name)
        {
            case Status.Name.STRENGTH:
                return strengthTex;
            case Status.Name.DEFENSE:
                return defenseTex;
            case Status.Name.SWIFTNESS:
                return swiftnessTex;
            case Status.Name.AEGIS:
                return aegisTex;
            case Status.Name.FORTIFY:
                return fortifyTex;
            case Status.Name.HAMSTRING:
                return hamstringTex;
            case Status.Name.POISON:
                return poisonTex;
            case Status.Name.SPIKES:
                return spikesTex;
            case Status.Name.PRAYER_FATIGUE:
                return prayerFatigueTex;
            case Status.Name.SABOTAGE:
                return sabotageTex;
            case Status.Name.HANDY:
                return handyTex;
            default:
                return null;
        }
    }
}
