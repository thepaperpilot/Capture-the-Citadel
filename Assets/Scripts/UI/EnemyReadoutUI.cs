using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyReadoutUI : MonoBehaviour
{
    [SerializeField, ShowInInspector]
    GameObject tickFab;
    [SerializeField, ShowInInspector]
    Transform tickParent;
    [SerializeField, ShowInInspector]
    Transform missingPortion;
    [SerializeField, ShowInInspector]
    TextMeshPro healthText;
    [SerializeField, ShowInInspector]
    TextMeshPro nameText;
    [SerializeField]
    bool billboard = true;

    int maxHealth;
    float unitHealth;

    [SerializeField, ShowInInspector]
    private float barWidth = 0.32f;

    public void Init(int maxHealth, string name)
    {
        //transform.localScale = Vector3.back;
        this.maxHealth = maxHealth;
        nameText.text = name;
        unitHealth = 1.0f / ((float)maxHealth);
        while (tickParent.childCount > 0)
            DestroyImmediate(tickParent.GetChild(0).gameObject);
        for(int i = 1; i < maxHealth; i++)
        {
            Transform temp = Instantiate(tickFab, tickParent).transform;
            temp.localRotation = Quaternion.identity;
            temp.localPosition = Vector3.right * ((2 * barWidth * i * unitHealth) - barWidth);
        }
        ChangeHealth(maxHealth);
    }

    public void ChangeHealth(int newHealth)
    {
        missingPortion.localScale = new Vector3((maxHealth - newHealth) * unitHealth, 1, 1);
        healthText.text = newHealth + "/" + maxHealth;
    }

    void Update()
    {
        if (!billboard) return;
        Vector3 target = PlayerManager.Instance.GetHeadsetPos();
        transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
    }
}
