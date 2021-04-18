using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class Script_IngameCameraMgr : MonoBehaviour
    {
        [SerializeField]
        private GameObject mAvoidancetPrefab = null;
        private int mCurrentSeg = 0;
        private float mTransition = 0;
        private float mCameraTransit = 1f / 4f;
        private Vector3 desiredPos, updatedRawPos;
        private Vector3 mActualPos;
        private List<GameObject> mOccludedObjects = new List<GameObject>();
        private bool wasInSight = false;
        private GameObject mLookAtUpdate = null;
        private const float speed = 6000;
        private const float maxAvoidNodeDist = 40f;
        private const float maxLoopCount = 40f;
        // Use this for initialization
        // Update is called once per frame
        void Update()
        {
            
            if (Script_WaypointMgr.instance.mWaypointList.Count > 1)
            {
                AvoidNodeGen();

                if (mCurrentSeg == 0 && mTransition < 0.05f)
                {
                    Vector3 formerNode = Script_WaypointMgr.instance.mWaypointList[0].transform.position;
                    Vector3 postNode = Script_WaypointMgr.instance.mWaypointList[1].transform.position;
                    Vector3 dir = postNode - formerNode;
                    Vector3 desiredInitialLook = dir.normalized + transform.position;
                    transform.LookAt(desiredInitialLook);
                    transform.position = formerNode;
                    mActualPos = transform.position;
                }

            }
           
            if (Script_GUI_Mgr.instance.isPlaying)
                Play();
            if (Script_GUI_Mgr.instance.isReset)
                Reset();
        }
        private void Reset()
        {
            mCurrentSeg = 0;
            mTransition = 0;
            desiredPos = Vector3.zero;
            mActualPos = transform.position;
            updatedRawPos = Vector3.zero;
            Script_GUI_Mgr.instance.isReset = false;

        }
        private void Play()
        {

            mTransition += Time.deltaTime * mCameraTransit;

            if (mTransition > 1)
            {
                if (mCurrentSeg < (Script_WaypointMgr.instance.mWaypointList.Count - 2))
                {
                    mCurrentSeg++;
                    mTransition = 0;
                }
                else
                {
                    mTransition = 1;
                    Script_GUI_Mgr.instance.isReset = true;
                    Script_GUI_Mgr.instance.isPlaying = false;
                }
            }
            else if (mTransition < 0)
            {
                mTransition = 1;
                mCurrentSeg--;
            }

            UpdatePlay();
        }
        private void UpdatePlay()
        {
            updatedRawPos = Script_WaypointMgr.instance.CatmullPosition(mCurrentSeg, mTransition);

            desiredPos = updatedRawPos;
            float fraction = Vector3.Distance(transform.position, desiredPos);
            if (!UpdateLookAt())
            {
                mCameraTransit = 1 / (1 / Script_GUI_Mgr.instance.camSpeed * fraction);
                var targetRotation = Quaternion.LookRotation(desiredPos - transform.position);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Script_GUI_Mgr.instance.camRotSpeed * Time.deltaTime*1.7f);
                transform.position = Vector3.Lerp(transform.position, desiredPos, fraction *Time.deltaTime);
            }

            mActualPos = Vector3.Lerp(mActualPos, desiredPos, fraction *Time.deltaTime);

        }
        private float AngleBetweenTargetObj(GameObject lookItem)
        {
            Vector3 targetDir = (lookItem.transform.position - transform.position).normalized;
            Vector3 desiredDir = (desiredPos - transform.position).normalized;
            float angle = Vector3.Angle(targetDir, desiredDir);
            angle = Mathf.Abs(angle);
            return 1/angle;
        }
        private bool UpdateLookAt()
        {
            
            if(Script_CoreObjectsMgr.instance.mObjectsList.Count > 0)
            {
                GameObject lookItem = Script_CoreObjectsMgr.instance.GetClosestObject();
                if(mLookAtUpdate!=null&&lookItem!=mLookAtUpdate)
                    mLookAtUpdate.GetComponent<MeshRenderer>().material.color = Color.yellow;
                if (Vector3.Distance(mActualPos, lookItem.transform.position) < Script_GUI_Mgr.instance.camLookAtDist)
                {
                    bool isVisible = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(GetComponent<Camera>()),
                                                                    lookItem.GetComponent<MeshRenderer>().bounds);
                    if (isVisible)
                    {
                        wasInSight = true;  
                    }
                    else
                    {
                        if (wasInSight)
                        {
                            if (mLookAtUpdate != null)
                                mLookAtUpdate.GetComponent<MeshRenderer>().material.color = Color.yellow;
                            float dist = Vector3.Distance(transform.position, mActualPos);
                            mCameraTransit = 1 / (1 / Script_GUI_Mgr.instance.camSpeedRot * dist);
                            transform.position = Vector3.Lerp(transform.position, mActualPos, dist * Time.deltaTime);
                            if (mOccludedObjects.Count > 0)
                            {
                                foreach (GameObject obj in mOccludedObjects)
                                {
                                    Color newColor = obj.gameObject.GetComponent<MeshRenderer>().material.color;
                                    newColor.a = 1f;
                                    obj.gameObject.GetComponent<MeshRenderer>().material.color = newColor;
                                }
                            }
                            return false;
                        }
                    }
                    lookItem.GetComponent<MeshRenderer>().material.color = Color.green;
                    mLookAtUpdate = lookItem;
                    Vector3 newLook = new Vector3(lookItem.transform.position.x, transform.position.y, lookItem.transform.position.z);
                    var targetRotation = Quaternion.LookRotation(newLook - transform.position);
                    float angle = Mathf.Pow(AngleBetweenTargetObj(lookItem), 2) * speed;
                    // Smoothly rotate towards the target point.
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, angle * Script_GUI_Mgr.instance.camRotSpeed* Time.deltaTime);
                    Vector3 dir = (lookItem.transform.position - mActualPos).normalized;
                    dir.y = 0;
                    float fraction = Vector3.Distance(transform.position, mActualPos - dir * Script_GUI_Mgr.instance.camRotRad);
                    mCameraTransit = 1 / (1 / Script_GUI_Mgr.instance.camSpeedRot * fraction);
                    transform.position = Vector3.Lerp(transform.position, mActualPos - dir * Script_GUI_Mgr.instance.camRotRad, fraction/ Script_GUI_Mgr.instance.camRotRad * Time.deltaTime);
                    RaycastHit hit;
                    float bumper = 3f;

                    if(Physics.Linecast(transform.position - bumper*dir, lookItem.transform.position, out hit) &&
                        hit.collider.gameObject != lookItem)
                    {
                        mOccludedObjects.Add(hit.collider.gameObject);
                        hit.collider.gameObject.GetComponent<Renderer>().material.SetFloat("_Mode", 3);
                        hit.collider.gameObject.GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        hit.collider.gameObject.GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        hit.collider.gameObject.GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
                        hit.collider.gameObject.GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
                        hit.collider.gameObject.GetComponent<Renderer>().material.DisableKeyword("_ALPHABLEND_ON");
                        hit.collider.gameObject.GetComponent<Renderer>().material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        hit.collider.gameObject.GetComponent<Renderer>().material.renderQueue = 3000;
                        Color newColor = hit.collider.gameObject.GetComponent<MeshRenderer>().material.color;
                        newColor.a = 0.3f;
                        hit.collider.gameObject.GetComponent<MeshRenderer>().material.color = newColor;
                    }
                    else
                    {
                        if (mOccludedObjects.Count > 0)
                        {
                            foreach (GameObject obj in mOccludedObjects)
                            {
                                Color newColor = obj.gameObject.GetComponent<MeshRenderer>().material.color;
                                newColor.a = 1f;
                                obj.gameObject.GetComponent<MeshRenderer>().material.color = newColor;
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    float fraction = Vector3.Distance(transform.position, mActualPos);
                    mCameraTransit = 1 / (1 / Script_GUI_Mgr.instance.camSpeedRot * fraction);
                    transform.position = Vector3.Lerp(transform.position, mActualPos, fraction * Time.deltaTime);
                }
                wasInSight = false;
            }
           
            if (mLookAtUpdate != null)
                mLookAtUpdate.GetComponent<MeshRenderer>().material.color = Color.yellow;
            return false;
        }

        void AvoidNodeGen()
        {
            var allButTriggers = ~(1 << 10);

            foreach (GameObject nodes in Script_WaypointMgr.instance.mWaypointList.ToList())
            {
                GameObject from = nodes;
                int wayPointStack = 0;
                foreach (GameObject waypoints in Script_WaypointMgr.instance.mWaypointList.ToList())
                {
                    if (waypoints == nodes)
                        continue;
                    //if layer == waypoint
                    if (waypoints.layer == 10 &&
                        Script_WaypointMgr.instance.mWaypointList.IndexOf(waypoints) > Script_WaypointMgr.instance.mWaypointList.IndexOf(nodes))
                        wayPointStack++;

                    if (wayPointStack > 1)
                    {
                        continue;
                    }

                    if (waypoints.layer == 10 && 
                        Script_WaypointMgr.instance.mWaypointList.IndexOf(waypoints) > Script_WaypointMgr.instance.mWaypointList.IndexOf(nodes)&&
                        wayPointStack < 2)
                    {

                        GameObject to = waypoints;

                        if (!Physics.Linecast(from.transform.position, to.transform.position, allButTriggers))
                        {
                            int count = Script_WaypointMgr.instance.mWaypointList.IndexOf(from) + 1;
                            while (count < Script_WaypointMgr.instance.mWaypointList.IndexOf(to))
                            {
                                if (Script_WaypointMgr.instance.mWaypointList[count].layer != 10)
                                {
                                    Destroy(Script_WaypointMgr.instance.mWaypointList[count]);
                                    Script_WaypointMgr.instance.mWaypointList.RemoveAt(count);
                                }
                            }
                        }
                    }
                }
            }
            if (Script_WaypointMgr.instance.mWaypointList[mCurrentSeg + 1] != null)
            {
                for (int i = mCurrentSeg; i < Script_WaypointMgr.instance.mWaypointList.Count - 1; ++i)
                {

                    bool isCollided = false;
                    Vector3 formerNode = Script_WaypointMgr.instance.mWaypointList[i+1].transform.position;
                    Vector3 postNode = Script_WaypointMgr.instance.mWaypointList[i].transform.position;
                    Vector3 desire = (formerNode + postNode) / 2f;

                    Vector3 dir = formerNode - postNode;

                    Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;
                    Vector3 right = -left;
                    Vector3 up = Vector3.up;
                    Vector3 down = -up;

                    Vector3 goLeftPost = postNode;
                    Vector3 goRightPost = postNode;
                    Vector3 goUpPost = postNode;
                    Vector3 goDownPost = postNode;

                    float nearestDist = Mathf.Infinity;
                    bool checkedAll = false;
                    bool checkedRight = false;
                    bool checkedLeft = false;
                    bool checkedUp = false;
                    bool checkedDown = false;
                    int checkCount = 0;
                    int loopCount = 0;
                    while (!checkedAll)//Physics.Linecast(formerNode, postNode, out newHitInfo))
                    {
                        bool isRightHit = Physics.Linecast(formerNode, goRightPost, allButTriggers);
                        bool isLeftHit = Physics.Linecast(formerNode, goLeftPost, allButTriggers);
                        bool isUpHit = Physics.Linecast(formerNode, goUpPost, allButTriggers);
                        bool isDownHit = Physics.Linecast(formerNode, goDownPost, allButTriggers);

                        if (!checkedRight && isRightHit)
                        {
                            goRightPost += right;
                            isCollided = true;
                            if (loopCount > maxLoopCount)
                            {
                                checkedRight = true;
                            }
                        }
                        else if (!checkedRight && isCollided)
                        {
                            Vector3 rightPos = (formerNode + goRightPost) / 2f + right * Script_GUI_Mgr.instance.camAvoidDist;
                            float rightDist = Vector3.Distance(formerNode, rightPos)+Vector3.Distance(rightPos, postNode);
                            isRightHit = Physics.Linecast(formerNode, rightPos, allButTriggers);
                            if ((rightDist < nearestDist) && !isRightHit)
                            {
                                desire = rightPos;
                                nearestDist = rightDist;
                            }
                            checkedRight = true;
                            checkCount++;
                        }

                        if (!checkedUp && isUpHit)
                        {
                            goUpPost += up;
                            isCollided = true;

                            if (loopCount > maxLoopCount)
                            {
                                checkedUp = true;
                            }
                        }
                        else if (!checkedUp && isCollided)
                        {
                            Vector3 upPos = (formerNode + goUpPost)/ 2f + up * Script_GUI_Mgr.instance.camAvoidDist;
                            float upDist = Vector3.Distance(formerNode, upPos) + Vector3.Distance(upPos, postNode);
                            isUpHit = Physics.Linecast(formerNode, upPos, allButTriggers);

                            if ((upDist < nearestDist)&&!isUpHit)
                            {
                                desire = upPos;
                                nearestDist = upDist;
                            }
                            checkedUp = true;
                            checkCount++;
                        }

                        if (!checkedDown && isDownHit)
                        {
                            goDownPost += down;
                            isCollided = true;
                            if (loopCount > maxLoopCount)
                            {
                                checkedDown = true;
                            }
                        }
                        else if (!checkedDown && isCollided)
                        {
                            Vector3 downPos = (formerNode + goDownPost) / 2f + down * Script_GUI_Mgr.instance.camAvoidDist;
                            float downDist = Vector3.Distance(formerNode, downPos) + Vector3.Distance(downPos, postNode);
                            isDownHit = Physics.Linecast(formerNode, downPos, allButTriggers);

                            if ((downDist < nearestDist)&&!isDownHit)
                            {
                                desire = downPos;
                                nearestDist = downDist;
                            }
                            checkedDown = true;
                            checkCount++;
                        }

                        if (!checkedLeft && isLeftHit)
                        {
                            goLeftPost += left;
                            isCollided = true;
                            if (loopCount > maxLoopCount)
                            {
                                checkedLeft = true;
                            }
                        }
                        else if (!checkedLeft && isCollided)
                        {
                            Vector3 leftPos = (formerNode + goLeftPost) / 2f + left * Script_GUI_Mgr.instance.camAvoidDist;
                            float leftDist = Vector3.Distance(formerNode, leftPos) + Vector3.Distance(leftPos, postNode);
                            isLeftHit = Physics.Linecast(formerNode, leftPos, allButTriggers);

                            if ((leftDist < nearestDist) && !isLeftHit)
                            {
                                desire = leftPos;
                                nearestDist = leftDist;
                            }
                            checkedLeft = true;
                            checkCount++;
                        }

                        if ((!isCollided && nearestDist ==Mathf.Infinity )|| checkCount > 1)
                        {
                            checkedAll = true;
                        }
                        else if(checkedRight &&
                                checkedUp &&
                                checkedDown &&
                                checkedLeft)
                        {
                                checkedAll = true;
                        }
                        loopCount++;
                    }
                    if (isCollided)
                    {
                        GameObject newSegment;
                        float dist = Vector3.Distance(Script_WaypointMgr.instance.mWaypointList[i].transform.position, desire);
                        if(dist > maxAvoidNodeDist)
                        {
                            Vector3 desireDir = desire - Script_WaypointMgr.instance.mWaypointList[i].transform.position;
                            desireDir.Normalize();
                            desire = Script_WaypointMgr.instance.mWaypointList[i].transform.position + desireDir * maxAvoidNodeDist;
                        }
                        newSegment = Instantiate(mAvoidancetPrefab, desire, Quaternion.identity);
                        Script_WaypointMgr.instance.mWaypointList.Insert(i + 1, newSegment);
                    }
                }
            }
        }
    }
}