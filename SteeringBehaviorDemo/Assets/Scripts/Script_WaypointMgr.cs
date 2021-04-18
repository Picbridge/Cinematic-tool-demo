using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_WaypointMgr : MonoBehaviour
{
    [SerializeField]
    public GameObject mWayPointPrefab = null;
    private GameObject showNode = null;
    public List<GameObject> mWaypointList = new List<GameObject>();
    public bool isWaypointReset = false;
    private bool isGenOnTime = true;
    [HideInInspector]
    public static Script_WaypointMgr instance;
    private void Start()
    {
        instance = this;
    }
    void Update()
    {
        ShowWaypointGeneration();
        GenerateWaypoint();

        if (Script_GUI_Mgr.instance.isReset)
            Reset();
    }
    public Vector3 CatmullPosition(int seg, float ratio)
    {

        Vector3 p1, p2, p3, p4;

        if (seg == 0)
        {
            p1 = mWaypointList[seg].transform.position;
            p2 = p1;
            p3 = mWaypointList[seg + 1].transform.position;
            if (mWaypointList.Count <= 2)
                p4 = p3;
            else
                p4 = mWaypointList[seg + 2].transform.position;
        }
        else if (seg == mWaypointList.Count - 2)
        {
            p1 = mWaypointList[seg - 1].transform.position;
            p2 = mWaypointList[seg].transform.position;
            p3 = mWaypointList[seg + 1].transform.position;
            p4 = p3;
        }
        else if (seg >= mWaypointList.Count - 1)
        {
            p1 = mWaypointList[seg - 1].transform.position;
            p2 = mWaypointList[mWaypointList.Count - 1].transform.position;
            p3 = p2;
            p4 = p2;
        }
        else if (seg < 0)
        {
            p1 = mWaypointList[0].transform.position;
            p2 = p1;
            p3 = p1;
            p4 = p1;
        }
        else
        {
            p1 = mWaypointList[seg - 1].transform.position;
            p2 = mWaypointList[seg].transform.position;
            p3 = mWaypointList[seg + 1].transform.position;
            if (mWaypointList.Count <= 2)
                p4 = p3;
            else
                p4 = mWaypointList[seg + 2].transform.position;
        }
        float t2 = ratio * ratio;
        float t3 = t2 * ratio;

        float x = 0.5f * ((2.0f * p2.x)
            + (-p1.x + p3.x)
            * ratio + (2.0f * p1.x - 5.0f * p2.x + 4 * p3.x - p4.x)
            * t2 + (-p1.x + 3.0f * p2.x - 3.0f * p3.x + p4.x)
            * t3);

        float y = 0.5f * ((2.0f * p2.y)
            + (-p1.y + p3.y)
            * ratio + (2.0f * p1.y - 5.0f * p2.y + 4 * p3.y - p4.y)
            * t2 + (-p1.y + 3.0f * p2.y - 3.0f * p3.y + p4.y)
            * t3);

        float z = 0.5f * ((2.0f * p2.z)
            + (-p1.z + p3.z)
            * ratio + (2.0f * p1.z - 5.0f * p2.z + 4 * p3.z - p4.z)
            * t2 + (-p1.z + 3.0f * p2.z - 3.0f * p3.z + p4.z)
            * t3);

        return new Vector3(x, y, z);
    }
    void GenerateWaypoint()
    {
        if (Input.GetMouseButtonDown(0) && Script_GUI_Mgr.instance.isGenerateModeOn && !Script_GUI_Mgr.instance.BlockedByUI)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                GameObject wayPoint;
                Vector3 genPos = hit.point + hit.normal * Script_GUI_Mgr.instance.nodeGenHeight;
                wayPoint = Instantiate(mWayPointPrefab, genPos, Quaternion.identity);
                mWaypointList.Add(wayPoint);
                if (mWaypointList.Count > 1)
                {
                    mWaypointList[index: (mWaypointList.Count - 2)].GetComponent<Script_Waypoint>().mNext =
                        mWaypointList[index: (mWaypointList.Count - 1)];
                }
            }
        }
        if (mWaypointList.Count >= 1)
            mWaypointList[mWaypointList.Count - 1].GetComponent<Script_Waypoint>().mNext = mWaypointList[mWaypointList.Count - 1];
    }

    void ShowWaypointGeneration()
    {
        if (Script_GUI_Mgr.instance.isGenerateModeOn && !Script_GUI_Mgr.instance.BlockedByUI)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (isGenOnTime)
            {
                if (Physics.Raycast(ray.origin, ray.direction, out hit))
                {
                    Vector3 newPos = hit.point + hit.normal* Script_GUI_Mgr.instance.nodeGenHeight;
                    showNode = Instantiate(mWayPointPrefab, newPos, Quaternion.identity);
                    showNode.GetComponent<Renderer>().material.SetFloat("_Mode", 3);
                    showNode.GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    showNode.GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    showNode.GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
                    showNode.GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
                    showNode.GetComponent<Renderer>().material.DisableKeyword("_ALPHABLEND_ON");
                    showNode.GetComponent<Renderer>().material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    showNode.GetComponent<Renderer>().material.renderQueue = 3000;
                    showNode.GetComponent<MeshRenderer>().material.color = new Color(1f,0f,0f,0.45f);
                    isGenOnTime = false;
                } 
            }
            else
            {
                if (Physics.Raycast(ray.origin, ray.direction, out hit)&& showNode!=null)
                {
                    Vector3 newPos = hit.point + hit.normal * Script_GUI_Mgr.instance.nodeGenHeight;
                    showNode.transform.position = newPos;
                }
            }
        }
        else
        {
            isGenOnTime = true;
            Destroy(showNode);
        }
    }

    private void Reset()
    {
        foreach(GameObject obj in mWaypointList)
        {
            Destroy(obj);
        }
        mWaypointList.Clear();
    }
}
