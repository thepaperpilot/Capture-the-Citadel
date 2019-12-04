using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RelicPickup : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly]
    private TextMeshPro sign;
    [SerializeField, ChildGameObjectsOnly]
    private Transform signRoot;

    public AbstractRelic holdingRelic;

    public void Init(AbstractRelic relic)
    {
        holdingRelic = relic;
        sign.text = relic.name + "\n" + relic.description;
        Transform model = Instantiate(relic.model, transform).transform;
        model.localPosition = Vector3.zero;
        model.localRotation = Quaternion.identity;
    }

    void Update()
    {
        signRoot.localRotation = Quaternion.LookRotation(transform.position - PlayerManager.Instance.GetHeadsetPos());
    }

    public void Collect()
    {
        RelicsManager.Instance.AddRelic(new RelicsManager.RelicData { relic = holdingRelic });
        Destroy(this);
    }
}
