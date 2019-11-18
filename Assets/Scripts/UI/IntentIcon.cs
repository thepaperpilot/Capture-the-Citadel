using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntentIcon : MonoBehaviour
{
    public static readonly int MajorDamageThreshold = 10;

    public enum Intent
    {
        ATTACK,
        ATTACK_MAJOR,
        ATTACK_RANGED,
        BLOCK,
        MOVE,
        DEBUFF,
        DEBUFF_MAJOR,
        BUFF,
        BUFF_MAJOR,
        OTHER
    }
    [SerializeField,ShowInInspector]
    private Sprite attackTex;
    [SerializeField, ShowInInspector]
    private Sprite attackMajorTex;
    [SerializeField, ShowInInspector]
    private Sprite attackRangedTex;
    [SerializeField, ShowInInspector]
    private Sprite blockTex;
    [SerializeField, ShowInInspector]
    private Sprite moveTex;
    [SerializeField, ShowInInspector]
    private Sprite debuffTex;
    [SerializeField, ShowInInspector]
    private Sprite debuffMajorTex;
    [SerializeField, ShowInInspector]
    private Sprite buffTex;
    [SerializeField, ShowInInspector]
    private Sprite buffMajorTex;
    [SerializeField, ShowInInspector]
    private Sprite otherTex;

    public SpriteRenderer sprite;
    public TextMeshPro amountText;

    public void Init(Intent intent, int amount)
    {
        switch (intent)
        {
            case Intent.ATTACK:
                sprite.sprite = attackTex;
                break;
            case Intent.ATTACK_MAJOR:
                sprite.sprite = attackMajorTex;
                break;
            case Intent.ATTACK_RANGED:
                sprite.sprite = attackRangedTex;
                break;
            case Intent.BLOCK:
                sprite.sprite = blockTex;
                break;
            case Intent.MOVE:
                sprite.sprite = moveTex;
                break;
            case Intent.DEBUFF:
                sprite.sprite = debuffTex;
                break;
            case Intent.DEBUFF_MAJOR:
                sprite.sprite = debuffMajorTex;
                break;
            case Intent.BUFF:
                sprite.sprite = buffTex;
                break;
            case Intent.BUFF_MAJOR:
                sprite.sprite = buffMajorTex;
                break;
            default:
                sprite.sprite = otherTex;
                break;
        }

        if(amount > 0)
        {
            amountText.text = amount.ToString();
        }
        else
        {
            amountText.text = "";
        }
    }
}
