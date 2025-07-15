using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(CharacterController))]
public class player_fps : MonoBehaviour
{
    [Header("Locomotion Direction")]
    public float speed = 5f;
    public Animator animator;

    [Header("MouseMovement")]
    public float mouseSensitivity = 500f;
    public Transform playerBody; // ← Player를 drag & drop

    float xRotation = 0f;


    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }


  

    void Update()
    {
        direction();
        MouseMovement();
    }




    void direction()
    {

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();
            controller.Move(direction * speed * Time.deltaTime);
        }

        // 부드러운 애니메이션 전환을 위해 보간 적용
        animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);

    }


    void MouseMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 카메라 상하 회전
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 플레이어 좌우 회전 - x축만 이동하게 함으로써 플레이어가 뒤집어지지 않게 한다.
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
