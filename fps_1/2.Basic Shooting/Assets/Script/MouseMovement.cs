using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 500f;
    public Transform playerBody; // ← Player를 drag & drop

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 카메라 상하 회전
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // 플레이어 좌우 회전
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
