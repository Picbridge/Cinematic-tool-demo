using UnityEngine;
using System.Collections;
using System.Linq;

namespace Assets.Scripts
{
    public class Script_Waypoint : MonoBehaviour
    {
        public GameObject mNext;
        public bool isChildActive;
        private void Start()
        {
            mNext = null;
            isChildActive = false;
        }

        [System.Obsolete]
        private void Update()
        {
            if (mNext != null)
            {
                GetComponent<LineRenderer>().SetWidth(0.3f,0.3f);
                GetComponent<LineRenderer>().SetPosition(0, transform.position);
                GetComponent<LineRenderer>().SetPosition(1, mNext.transform.position);
            }
            RemoveWaypoint();
            GenerateMouseMove();

        }
        private void OnMouseDown()
        {
            RemoveWaypoint();
        }
        void GenerateMouseMove()
        {
            Vector3 mousePos = new Vector3(0, 0, 0);
            if (Script_GUI_Mgr.instance.isWaypointMove && !Script_GUI_Mgr.instance.BlockedByUI)
            {
                if (isChildActive == false)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                isChildActive = false;
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        void RemoveWaypoint()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit)&&Input.GetMouseButtonDown(0) && Script_GUI_Mgr.instance.isRemoveModeOn && !Script_GUI_Mgr.instance.BlockedByUI)
            {
                
                foreach (GameObject nodes in Script_WaypointMgr.instance.mWaypointList.ToList())
                {
                    if (nodes == hit.collider.gameObject)
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
                            //if layer == waypoint and it's prior than selected node
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
                        int count = Script_WaypointMgr.instance.mWaypointList.IndexOf(prevWaypoint) + 1;
                        while (count < Script_WaypointMgr.instance.mWaypointList.IndexOf(nextWaypoint))
                        {
                            if (Script_WaypointMgr.instance.mWaypointList[count].layer == 9)
                            {
                                Destroy(Script_WaypointMgr.instance.mWaypointList[count]);
                                Script_WaypointMgr.instance.mWaypointList.RemoveAt(count);
                            }
                            else
                                count++;
                        }
                        prevWaypoint.GetComponent<Script_Waypoint>().mNext = nextWaypoint;
                        Script_WaypointMgr.instance.mWaypointList.Remove(nodes);
                        Destroy(nodes);
                        continue;
                    }
                }
            }
        }
    }
}