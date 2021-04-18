using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_CameraController : MonoBehaviour
{
    private const float SPEED_UP = 20f;
    private const float SPEED_NORMAL = 10f;

    public float m_MouseSenibility = 100f;
    private float m_RotationX = 0f;
    private float m_RotationY = 0f;
    private float m_MoveY = 0;
    private float m_Speed = SPEED_NORMAL;

    private bool m_isMousePressed = false;
    private bool m_isMoveUpPressed = false;
    private bool m_isMoveDownPressed = false;
    private bool m_isShiftPressed = false;
    private void Start()
    {
        m_RotationX = transform.localRotation.eulerAngles.x;
        m_RotationY = transform.localRotation.eulerAngles.y;
}
    void Update()
    {
        UpdateKeyPressed();

        if (m_isMousePressed)
        {
            Cursor.lockState = CursorLockMode.Locked;
            CameraMove();
            CameraView();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
    void UpdateKeyPressed()
    {
        if (Input.GetMouseButtonDown(1))
        {
            m_isMousePressed = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            m_isMousePressed = false;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_isMoveDownPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            m_isMoveDownPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            m_isMoveUpPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            m_isMoveUpPressed = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            m_isShiftPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            m_isShiftPressed = false;
        }
    }
    void CameraMove()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        if (m_isMoveDownPressed)
        {
            m_MoveY = -1;
        }
        else if(m_isMoveUpPressed)
        {
            m_MoveY = 1;
        }
        else
        {
            m_MoveY = 0;
        }
        if (m_isShiftPressed)
        {
            m_Speed = SPEED_UP;
        }
        else
        {
            m_Speed = SPEED_NORMAL;
        }

        Vector3 move = transform.right * x + transform.forward * z + transform.up * m_MoveY;
        transform.position += move* m_Speed *Time.deltaTime;
    }
    void CameraView()
    {
        float mouseX = Input.GetAxis("Mouse X") * m_MouseSenibility * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * m_MouseSenibility * Time.deltaTime;

        m_RotationX -= mouseY;
        m_RotationY += mouseX;

        transform.localRotation = Quaternion.Euler(m_RotationX, m_RotationY, 0f);
    }
}
