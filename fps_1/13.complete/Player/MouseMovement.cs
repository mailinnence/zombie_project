using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 1300f;
    public Transform playerBody; // ← Player를 drag & drop
    public static MouseMovement instance;
    public bool offMouse;


    float xRotation = 0f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if(!offMouse)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -85f, 85f);

            // 카메라 상하 회전
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            // 플레이어 좌우 회전 - x축만 이동하게 함으로써 플레이어가 뒤집어지지 않게 한다.
            playerBody.Rotate(Vector3.up * mouseX);
        }


    }
}
