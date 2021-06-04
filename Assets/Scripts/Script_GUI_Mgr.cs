using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Script_GUI_Mgr : MonoBehaviour
{
    [HideInInspector]
    public bool isGenerateModeOn = false;
    [HideInInspector]
    public bool isRemoveModeOn = false;
    [HideInInspector]
    public bool isWaypointMove = false;
    [HideInInspector]
    public bool isPlaying = false;
    [HideInInspector]
    public bool isReset = false;
    [HideInInspector]
    public bool isSettingObj = false;
    [HideInInspector]
    public bool isCreditOn = false;
    [HideInInspector]
    public float camLookAtDist;
    [HideInInspector]
    public float camRotSpeed;
    [HideInInspector]
    public float camAvoidDist;
    [HideInInspector]
    public bool BlockedByUI;
    [HideInInspector]
    public float camRotRad;
    [HideInInspector]
    public float camSpeed;
    [HideInInspector]
    public float camSpeedRot;
    [HideInInspector]
    public float nodeGenHeight;

    [SerializeField]
    private GameObject Credit = null;
    private Vector3 creditPos;

    private Vector2 scrollViewVector = Vector2.zero;
    private Rect dropDownRect;
    private Rect simulateRect;
    private Rect resetRect;
    private Rect mainRect;
    private Rect camDistRect;
    private Rect camRotSpeedRect;
    private Rect camRotRadRect;
    private Rect camAvoidRect;
    private Rect camSpeedRect;
    private Rect camSpeedRotRect;
    private Rect nodeGenHeightRect;
    private Rect removeNodeRect;
    private Rect CreditRect;

    int numOfMenus = 12;
    public static Script_GUI_Mgr instance;
    // Start is called before the first frame update
    private void Start()
    {
        dropDownRect = new Rect(125, 102, 125, 300);
        simulateRect = new Rect(0, 155, 125, 300);
        resetRect = new Rect(117, 155, 125, 300);
        mainRect = new Rect(10, 10, 240, numOfMenus*35);
        camDistRect = new Rect(20, 280, 160, 25);
        camRotSpeedRect = new Rect(20, 305, 160, 25);
        camAvoidRect = new Rect(20, 180, 160, 25);
        camRotRadRect = new Rect(20, 330, 160, 25);
        camSpeedRect = new Rect(20, 205, 160, 25);
        camSpeedRotRect = new Rect(20, 355, 160, 25);
        nodeGenHeightRect = new Rect(20, 80, 160, 25);
        removeNodeRect = new Rect(20, 55, 160, 25);
        CreditRect = new Rect(20, 380, 160, 25);

        creditPos = Credit.transform.GetChild(0).position;

        camRotRad = 3f;
        camLookAtDist = 10;
        camRotSpeed = 0.7f;
        camAvoidDist = 5f;
        camSpeed = 1f;
        camSpeedRot = 1f;
        nodeGenHeight = 3f;

        instance = this;

    }
    private void OnGUI()
    {
        if (!isCreditOn)
        {
            // Make a background box
            GUI.Box(mainRect, "Menu");
        if (mainRect.Contains(Event.current.mousePosition))
            BlockedByUI = true;
        else
            BlockedByUI = false;
        if (GUI.Button(new Rect((resetRect.x + 20), resetRect.y, resetRect.width - 20, 25), "Reset"))
        {
            if(!isPlaying)
                isReset = true;
        }

        if(GUI.Toggle(new Rect(20, dropDownRect.y, 170, 25),isWaypointMove, "Waypoint Movement"))
        {
            isWaypointMove = true;
            isGenerateModeOn = false;
            isRemoveModeOn = false;
        }
        else
            isWaypointMove = false;

        if (GUI.Toggle(new Rect(20, 30, 170, 20), isGenerateModeOn, "Generate Waypoint"))
        {
            isGenerateModeOn = true;
            isSettingObj = false;
            isRemoveModeOn = false;
        }
        else
            isGenerateModeOn = false;

        if (GUI.Toggle(removeNodeRect, isRemoveModeOn, "Remove Waypoint"))
        {
            isRemoveModeOn = true;
            isGenerateModeOn = false;
        }
        else
            isRemoveModeOn = false;

        if (isGenerateModeOn)
        {
            isWaypointMove = false;
            isPlaying = false;
            isReset = false;

        }
            
        if (!isPlaying)
        {
            if (GUI.Button(new Rect((simulateRect.x + 20), simulateRect.y, simulateRect.width - 20, 25), "Simulate")&&
                Script_WaypointMgr.instance.mWaypointList.Count>1)
            {
                    isPlaying = true;
            }
        }
        else
        {
            if (GUI.Button(new Rect((simulateRect.x + 20), simulateRect.y, simulateRect.width - 20, 25), "Stop"))
            {
                isPlaying = false;
            }
        }

        GUI.Label(new Rect(20, 130, 170, 25), "Camera Control:");
        GUI.Label(new Rect(20, 230, 170, 25), "Camera Look at:");
        if(GUI.Toggle(new Rect(20, 255, 170, 25), isSettingObj, "Set objects to look"))
        {
            isSettingObj = true;
            isGenerateModeOn = false;
            isRemoveModeOn = false;
        }
        else
            isSettingObj = false;

        GUI.Label(nodeGenHeightRect, "Waypoint gen height:");
        GUI.Label(new Rect(nodeGenHeightRect.x + 120, nodeGenHeightRect.y - 10, nodeGenHeightRect.width - 55, 25), nodeGenHeight.ToString());
        nodeGenHeight = GUI.HorizontalSlider(new Rect(nodeGenHeightRect.x + 120, nodeGenHeightRect.y + 7, nodeGenHeightRect.width - 55, 25), nodeGenHeight, 0, 20);

        GUI.Label(camDistRect, "Look from distance:");
        GUI.Label(new Rect(camDistRect.x + 120, camDistRect.y-10, camDistRect.width - 55, 25),  camLookAtDist.ToString());
        camLookAtDist = GUI.HorizontalSlider(new Rect(camDistRect.x+120, camDistRect.y+5, camDistRect.width-55, 25), camLookAtDist, 0, 100);

        GUI.Label(camRotSpeedRect, "Cam steer speed:");
        GUI.Label(new Rect(camRotSpeedRect.x + 120, camRotSpeedRect.y - 8, camRotSpeedRect.width - 55, 25), camRotSpeed.ToString());
        camRotSpeed = GUI.HorizontalSlider(new Rect(camRotSpeedRect.x + 120, camRotSpeedRect.y + 7, camRotSpeedRect.width - 55, 25), camRotSpeed, 0, 2);

        GUI.Label(camAvoidRect, "Avoidance distance:");
        GUI.Label(new Rect(camAvoidRect.x + 120, camAvoidRect.y - 8, camAvoidRect.width - 55, 25), camAvoidDist.ToString());
        camAvoidDist = GUI.HorizontalSlider(new Rect(camAvoidRect.x + 120, camAvoidRect.y + 7, camAvoidRect.width - 55, 25), camAvoidDist, 0, 20);
        
        GUI.Label(camRotRadRect, "Cam orbit radius:");
        GUI.Label(new Rect(camRotRadRect.x + 120, camRotRadRect.y - 8, camRotRadRect.width - 55, 25), camRotRad.ToString());
        camRotRad = GUI.HorizontalSlider(new Rect(camRotRadRect.x + 120, camRotRadRect.y + 7, camRotRadRect.width - 55, 25), camRotRad, 1, 15);

        GUI.Label(camSpeedRect, "Cam speed:");
        GUI.Label(new Rect(camSpeedRect.x + 120, camSpeedRect.y - 8, camSpeedRect.width - 55, 25), camSpeed.ToString());
        camSpeed = GUI.HorizontalSlider(new Rect(camSpeedRect.x + 120, camSpeedRect.y + 7, camSpeedRect.width - 55, 25), camSpeed, 0, 2);

        GUI.Label(camSpeedRotRect, "Orbit speed:");
        GUI.Label(new Rect(camSpeedRotRect.x + 120, camSpeedRotRect.y - 8, camSpeedRotRect.width - 55, 25), camSpeedRot.ToString());
        camSpeedRot = GUI.HorizontalSlider(new Rect(camSpeedRotRect.x + 120, camSpeedRotRect.y + 7, camSpeedRotRect.width - 55, 25), camSpeedRot, 0, 2);


            if (GUI.Button(new Rect((CreditRect.x + 30), CreditRect.y + 10, CreditRect.width - 20, 25), "Credit On"))
            {
                isCreditOn = true;
                Credit.transform.GetChild(0).position = creditPos;
                Credit.SetActive(true);
            }
        }
        else
        {
            if (GUI.Button(new Rect((mainRect.x), mainRect.y, CreditRect.width - 20, 25), "Credit Off"))
            {
                isCreditOn = false;
                Credit.SetActive(false);
            }
        }
    }
}

