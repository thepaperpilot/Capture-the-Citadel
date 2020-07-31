using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class OptionController : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly]
    private TextMeshPro sign;

    private AbstractEvent.Option option;
    private AbstractEvent controller;

    void Update()
    {
        transform.localRotation = Quaternion.LookRotation(transform.position - PlayerManager.Instance.GetHeadsetPos());
    }

    public void Setup(AbstractEvent.Option option, AbstractEvent controller) {
        sign.text = option.description;
        this.option = option;
        this.controller = controller;
    }

    public void Choose() {
        controller.Choose(option);
    }
}
