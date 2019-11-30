using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusIcon : MonoBehaviour
{
    public SpriteRenderer sprite;
    public TextMeshPro amountText;

    public void Init(Sprite icon, int amount)
    {
        sprite.sprite = icon;
        amountText.text = amount.ToString();
    }
}
