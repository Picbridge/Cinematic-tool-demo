using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class Script_CoreObjectsMgr : MonoBehaviour
{
    public List<GameObject> mObjectsList = new List<GameObject>();
    public List<Color> mObjectsColor = new List<Color>();
    public Camera mIngameCam = null;
    LineRenderer mRange;
    const int SEGMENT = 51;
    [HideInInspector]
    public static Script_CoreObjectsMgr instance;
    [SerializeField]
    private Material LookDistance = null;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        AddObjectsToSee();
        ShowLookDistance();
    }

    [System.Obsolete]
    void ShowLookDistance()
    {
        float x, z;
        float angle = 0f;
        
        foreach (GameObject objects in mObjectsList)
        {
            mRange = objects.gameObject.GetComponent<LineRenderer>();
            mRange.SetVertexCount(SEGMENT);
            mRange.useWorldSpace = false;
            mRange.material = LookDistance;
            for (int i = 0;i< SEGMENT; i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * (Script_GUI_Mgr.instance.camLookAtDist)/ objects.transform.localScale.x;
                z = Mathf.Cos(Mathf.Deg2Rad * angle) * (Script_GUI_Mgr.instance.camLookAtDist)/ objects.transform.localScale.z;
                Vector3 rangePos = new Vector3(x, 0, z);
                mRange.SetPosition(i, rangePos);
                angle += (360f / SEGMENT);
            }
        }
    }

    void AddObjectsToSee()
    {

        if (Input.GetMouseButtonDown(0) && Script_GUI_Mgr.instance.isSettingObj && !Script_GUI_Mgr.instance.BlockedByUI)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool isDuplicated = false;
            if (Physics.Raycast(ray.origin, ray.direction, out hit) &&
                hit.collider.gameObject.layer == 11)
            {
                GameObject objectToSee = hit.collider.gameObject;
                int index = 0;
                foreach (GameObject obj in mObjectsList)
                {
                    if(obj == objectToSee)
                    {
                        isDuplicated = true;
                    }
                    else
                        index++;
                }
                if (isDuplicated)
                {
                    objectToSee.GetComponent<MeshRenderer>().material.color = mObjectsColor[index];
                    Destroy(objectToSee.GetComponent<LineRenderer>());
                    mObjectsList.Remove(objectToSee);
                    mObjectsColor.RemoveAt(index);
                }
                else
                {
                    mObjectsColor.Add(objectToSee.GetComponent<MeshRenderer>().material.color);
                    objectToSee.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    objectToSee.AddComponent<LineRenderer>();
                    mObjectsList.Add(objectToSee);
                }
            }
        }
    }

    public GameObject GetClosestObject()
    {
        int index = 0;

        for (int i = 0; i < mObjectsList.Count; ++i)
        {
            //check distance from previous node -- will probably do something more precise later, might not need to since camera is fixed to track
            float dist0 = Vector3.Distance(mIngameCam.transform.position, mObjectsList[i].transform.position); //potential leader
            float dist1 = Vector3.Distance(mIngameCam.transform.position, mObjectsList[index].transform.position); //current leader
            if (dist0 < dist1)
            {
                index = i;
            }
        }

        return mObjectsList[index];
    }
}
