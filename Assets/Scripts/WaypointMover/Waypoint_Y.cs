using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts;

public class Waypoint_Y : MonoBehaviour
{
    private Vector3 mOffset;
    private const float TRANSFORM_SPEED = 0.1f;
    private void OnMouseDown()
    {
        Cursor.lockState = CursorLockMode.Locked;
        mOffset = transform.parent.position - GetMouseWorld();
    }
    private void OnMouseUp()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }
    Vector3 GetMouseWorld()
    {
        Vector3 mousePos = new Vector3(0, 0, 0);
        mousePos.y += Input.mousePosition.y;
        return mousePos * TRANSFORM_SPEED;
    }
    void AvoidNodeUpdate()
    {
        foreach (GameObject nodes in Script_WaypointMgr.instance.mWaypointList.ToList())
        {

            if (nodes == transform.parent.gameObject)
            {
                GameObject saveWaypoint = null;
                GameObject prevWaypoint = null;
                GameObject nextWaypoint = null;
                foreach (GameObject waypoints in Script_WaypointMgr.instance.mWaypointList.ToList())
                {
                    if (waypoints == nodes)
                    {
                        prevWaypoint = saveWaypoint;
                        continue;
                    }
                    //if layer == waypoint and it's 
                    if (waypoints.layer == 10 &&
                        Script_WaypointMgr.instance.mWaypointList.IndexOf(waypoints) < Script_WaypointMgr.instance.mWaypointList.IndexOf(nodes))
                        saveWaypoint = waypoints;
                }
                if (Script_WaypointMgr.instance.mWaypointList.IndexOf(nodes) == Script_WaypointMgr.instance.mWaypointList.Count - 1)
                    nextWaypoint = nodes;
                else
                    nextWaypoint = nodes.GetComponent<Script_Waypoint>().mNext;
                if (prevWaypoint == null)
                    prevWaypoint = nodes;
                //prevWaypoint.GetComponent<MeshRenderer>().material.color = Color.black;
                //nextWaypoint.GetComponent<MeshRenderer>().material.color = Color.white;
                int count = Script_WaypointMgr.instance.mWaypointList.IndexOf(prevWaypoint) + 1;
                while (count < Script_WaypointMgr.instance.mWaypointList.IndexOf(nextWaypoint))
                {
                    if (Script_WaypointMgr.instance.mWaypointList[count].layer != 10)
                    {
                        Destroy(Script_WaypointMgr.instance.mWaypointList[count]);
                        Script_WaypointMgr.instance.mWaypointList.RemoveAt(count);
                    }
                    else
                        count++;
                }
            }
        }
    }
    private void OnMouseDrag()
    {
        AvoidNodeUpdate();
        transform.parent.position = GetMouseWorld() + mOffset;
    }
}
