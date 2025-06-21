using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public Transform cameraHolder; // ← CameraParent 또는 CameraRig를 drag & drop
    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    private Vector3 lastPosition = Vector3.zero;
    bool isMoving;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // ⬇️ 카메라 기준으로 이동
        Vector3 move = cameraHolder.right * x + cameraHolder.forward * z;
        move.y = 0f; // 수직 방향 제거 (중력과 충돌 방지)

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (lastPosition != transform.position && isGrounded)
            isMoving = true;
        else
            isMoving = false;

        lastPosition = transform.position;
    }
}
