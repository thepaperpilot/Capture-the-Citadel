using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class CampfireSelectorController : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly]
    private TextMeshPro sign;

    CampfireScreenController.CampfireAction action;
    private CampfireScreenController controller;

    void Update()
    {
        transform.localRotation = Quaternion.LookRotation(transform.position - PlayerManager.Instance.GetHeadsetPos());
    }

    public void Setup(CampfireScreenController.CampfireAction action, CampfireScreenController controller) {
        switch (action)
        {
            case CampfireScreenController.CampfireAction.REST:
                sign.text = "Rest\nRegain " + controller.GetHealAmount() + " health";
                break;
            case CampfireScreenController.CampfireAction.UPGRADE:
                sign.text = "WIP";
                break;
        }
        
        this.action = action;
        this.controller = controller;
    }

    public void Select() {
        controller.SelectAction(action);
    }

    
}
