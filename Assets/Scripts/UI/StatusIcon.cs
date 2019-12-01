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

    public void Init(Status status)
    {
        sprite.sprite = GetIcon(status.name);
        amountText.text = status.amount.ToString();
    }

    private Sprite GetIcon(Status.NAME name)
    {
        switch (name)
        {
            case Status.NAME.STRENGTH:
                return strengthTex;
            case Status.NAME.DEFENSE:
                return defenseTex;
            case Status.NAME.SWIFTNESS:
                return swiftnessTex;
            case Status.NAME.AEGIS:
                return aegisTex;
            case Status.NAME.FORTIFY:
                return fortifyTex;
            case Status.NAME.HAMSTRING:
                return hamstringTex;
            case Status.NAME.POISON:
                return poisonTex;
            case Status.NAME.SPIKES:
                return spikesTex;
            case Status.NAME.PRAYER_FATIGUE:
                return prayerFatigueTex;
            default:
                return null;
        }
    }
}
