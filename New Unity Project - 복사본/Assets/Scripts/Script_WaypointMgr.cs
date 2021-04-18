using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_WaypointMgr : MonoBehaviour
{
    [SerializeField]
    private GameObject mWayPoint = null;
    [SerializeField]
    private GameObject mMap = null;

    void Update()
    {
        if (/*Input.GetMouseButtonDown(0)*/Input.GetKeyDown(KeyCode.G))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (mMap.scene.GetPhysicsScene().Raycast(ray.origin, ray.direction, out hit))
            {
                Instantiate(mWayPoint, hit.point, Quaternion.identity);
            }
        }
    }
}
