using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicPedestalController : MonoBehaviour
{
    public GameObject relicRewardFab;
    public Transform relicPos;

    public void Init(AbstractRelic relic)
    {
        RelicPickup pickup = Instantiate(relicRewardFab, relicPos).GetComponent<RelicPickup>();
        pickup.transform.localPosition = Vector3.zero;
        pickup.transform.localRotation = Quaternion.identity;
        pickup.Init(relic);
    }
}
