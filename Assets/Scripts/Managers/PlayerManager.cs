using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject playAreaAlias;
    [SerializeField] private GameObject headsetAlias;
    [SerializeField] private List<Transform> leftControllerSources;
    [SerializeField] private List<Transform> rightControllerSources;
    private Transform leftSource;
    private Transform rightSource;
    Rigidbody leftRB;
    Rigidbody rightRB;
    // Start is called before the first frame update

    void Start()
    {
        leftSource = leftControllerSources[0];
        rightSource = rightControllerSources[0];
        leftRB = leftHand.GetComponent<Rigidbody>();
        rightRB = rightHand.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        leftHand.transform.position = leftSource.position;
        leftHand.transform.rotation = leftSource.rotation;
        rightHand.transform.position = rightSource.position;
        rightHand.transform.rotation = rightSource.rotation;
    }

    private void FixedUpdate()
    {
        //Move Hands
        Vector3 leftDelta = leftSource.position - leftHand.transform.position;
        //leftRB.AddForce(leftDelta, ForceMode.Acceleration);
        leftHand.transform.position = leftSource.position;
        Vector3 rightDelta = rightSource.position - rightHand.transform.position;
        rightHand.transform.position = rightSource.position;
        //rightRB.AddForce(rightDelta, ForceMode.Acceleration);
    }
}
