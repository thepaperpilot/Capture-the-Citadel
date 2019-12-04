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
    GameObject intentFab;
    [SerializeField, ShowInInspector]
    GameObject statusFab;
    [SerializeField, ShowInInspector]
    Transform tickParent;
    [SerializeField, ShowInInspector]
    Transform missingPortion;
    [SerializeField, ShowInInspector]
    Transform intentParent;
    [SerializeField, ShowInInspector]
    Transform statusParent;
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
    private float statusWidth = 0.1f;
    private int statusPerRow = 8;
    private float intentWidth = 0.2f;

    public struct Intent
    {
        public IntentIcon.Intent icon;
        public int amount;

        public Intent (IntentIcon.Intent icon, int amount)
        {
            this.icon = icon;
            this.amount = amount;
        }

        public Intent(IntentIcon.Intent icon)
        {
            this.icon = icon;
            this.amount = -1;
        }
    }

    public void Init(int maxHealth, string name)
    {
        if(maxHealth < 1)
        {
            maxHealth = 1;
        }
        //transform.localScale = Vector3.back;
        this.maxHealth = maxHealth;
        nameText.text = name;
        unitHealth = 1.0f / ((float)maxHealth);
        while (tickParent.childCount > 0)
            DestroyImmediate(tickParent.GetChild(0).gameObject);

        //Reduce tick size for large health bars
        float factor = Mathf.Max(1, Mathf.Log(maxHealth));
        //Spawn the tick masks
        for (int i = 1; i < maxHealth; i++)
        {
            Transform temp = Instantiate(tickFab, tickParent).transform;
            temp.localRotation = Quaternion.identity;
            temp.localPosition = Vector3.right * ((2 * barWidth * i * unitHealth) - barWidth);

            
            temp.localScale = new Vector3(1 / factor, 1, 1);
        }
        ChangeValue(maxHealth);
    }

    public void ChangeValue(int newHealth)
    {
        try
        {
            missingPortion.localScale = new Vector3(Mathf.Clamp((maxHealth - newHealth) * unitHealth, 0, 1), 1, 1);
            healthText.text = newHealth + "/" + maxHealth;
        }
        catch { }
    }

    public void ClearIntents()
    {
        while (intentParent.childCount > 0)
            DestroyImmediate(intentParent.GetChild(0).gameObject);
    }

    public void SetIntents(List<Intent> intents)
    {
        ClearIntents();
        if(intents.Count < 1)
        {
            return;
        }

        float pos = (intents.Count - 1) * -0.5f * intentWidth;

        foreach(Intent intent in intents)
        {
            GameObject temp = Instantiate(intentFab, intentParent);
            temp.transform.localRotation = Quaternion.identity;
            temp.transform.localPosition = new Vector3(pos, 0, 0);
            temp.GetComponent<IntentIcon>().Init(intent.icon, intent.amount);
            pos += intentWidth;
        }
    }

    public void ClearStatuses()
    {
        while (statusParent.childCount > 0)
            DestroyImmediate(statusParent.GetChild(0).gameObject);
    }

    public void SetStatuses(List<Status> statuses)
    {
        ClearStatuses();
        statuses.Sort(new StatusDisplaySort());
        int count = 0;

        foreach(Status status in statuses)
        {
            GameObject temp = Instantiate(statusFab, statusParent);
            temp.transform.localRotation = Quaternion.identity;
            temp.transform.localPosition = new Vector3(statusWidth * (count%statusPerRow), -statusWidth * (count/statusPerRow), 0);
            temp.GetComponent<StatusIcon>().Init(status);
            count++;
        }
    }

    void Update()
    {
        if (!billboard) return;
        Vector3 target = PlayerManager.Instance.GetHeadsetPos();
        transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
    }
}
