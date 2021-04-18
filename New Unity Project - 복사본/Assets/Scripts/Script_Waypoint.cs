using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Script_Waypoint : MonoBehaviour
    {
        private const float TRANSFORM_SPEED = 0.05f;

        private Vector3 mOffset;

        public bool mIsZaxisOn = true;
        public bool mIsXaxisOn = false;
        public bool mIsYaxisOn = false;
        private void OnMouseDown()
        {
            mOffset = gameObject.transform.position - GetMouseWorld();
        }
        Vector3 GetMouseWorld()
        {
            Vector3 mousePos = new Vector3(0, 0, 0);//Input.mousePosition;
            if (mIsZaxisOn)
            {
                mIsXaxisOn = false;
                mIsYaxisOn = false;
                mousePos.z += Input.mousePosition.x; 
            }
            if (mIsXaxisOn)
            {
                mIsZaxisOn = false;
                mIsYaxisOn = false;
                mousePos.x += Input.mousePosition.x;
            }
            if (mIsYaxisOn)
            {
                mIsZaxisOn = false;
                mIsXaxisOn = false;
                mousePos.y += Input.mousePosition.y;
            }

            return mousePos* TRANSFORM_SPEED;
        }
        private void OnMouseDrag()
        {
            transform.position = GetMouseWorld() + mOffset;
        }
    }
}