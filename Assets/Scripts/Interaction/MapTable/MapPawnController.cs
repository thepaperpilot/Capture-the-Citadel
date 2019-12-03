using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPawnController : MonoBehaviour
{
    public Transform raySource;
    public MapTableController controller;

    public bool locked = false;
    public float maxDist = 0.2f;

    public LayerMask mapLayer;

    public void Drop()
    {
        MapTableTile targetTile = null;
        RaycastHit hit;
        if(Physics.Raycast(raySource.position, Vector3.down, out hit, maxDist, mapLayer)){
            if (hit.collider.CompareTag("MapTile"))
            {
                targetTile = hit.collider.GetComponentInParent<MapTableTile>();
            }
        }

        controller.Select(targetTile, this);
    }
}
