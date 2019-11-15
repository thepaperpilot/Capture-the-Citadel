using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ClassSelectorController : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly]
    private CardController card;

    private AbstractClass abstractClass;
    private TitleScreenController controller;

    void Update()
    {
        transform.localRotation = Quaternion.LookRotation(transform.position - PlayerManager.Instance.GetHeadsetPos());
    }

    public void Setup(AbstractClass abstractClass, TitleScreenController controller) {
        card.Setup(abstractClass.card, false);
        this.abstractClass = abstractClass;
        this.controller = controller;
    }

    public void Select() {
        controller.SelectClass(abstractClass);
    }
}
