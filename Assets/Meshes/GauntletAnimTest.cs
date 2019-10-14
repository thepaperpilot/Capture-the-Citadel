using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GauntletAnimTest : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            anim.SetInteger("state", 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            anim.SetInteger("state", 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            anim.SetInteger("state", 3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            anim.SetInteger("state", 4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            anim.SetInteger("state", 5);
        }
    }
}
