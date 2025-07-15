using UnityEngine;

public class PlayerMovement_ : MonoBehaviour
{
    [Header("점프 관련 변수")]
    private CharacterController controller;
    public Transform cameraHolder; // CameraParent 또는 CameraRig를 drag & drop
    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    [Header("착지 관련 변수")]
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

        Vector3 forward = cameraHolder.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 right = cameraHolder.right;
        right.y = 0f;
        right.Normalize();

        Vector3 move = right * x + forward * z;

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = move * speed;
        finalMove.y = velocity.y;

        controller.Move(finalMove * Time.deltaTime);

        isMoving = move.sqrMagnitude > 0.001f && isGrounded;
    }
}
