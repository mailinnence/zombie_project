using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class test : MonoBehaviour
{
    private CharacterController controller;

    [Header("Movement")]
    public float speed = 5f;
    public Animator animator;
    public Transform cameraHolder;

    [Header("Jump & Gravity")]
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;
    private Vector3 velocity;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

    

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    
    void Update()
    {
        isGrounded = CheckGrounded();
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("YVelocity", velocity.y);

        HandleJump();   
        MovePlayer();
        ApplyGravity();
    }


    void MovePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        // 카메라 forward/right의 XZ 수평 방향만 사용
        Vector3 flatForward = Vector3.ProjectOnPlane(cameraHolder.forward, Vector3.up).normalized;
        Vector3 flatRight = Vector3.ProjectOnPlane(cameraHolder.right, Vector3.up).normalized;

        // 평면 기반 이동 방향 계산
        Vector3 moveDir = flatForward * inputDir.z + flatRight * inputDir.x;

        Vector3 move = moveDir * speed;
        move.y = velocity.y;

        controller.Move(move * Time.deltaTime);

        animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);
    }



    bool CheckGrounded()
    {
        bool grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        return grounded;
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }
}



/*
void MovePlayer()
{
    float horizontal = Input.GetAxisRaw("Horizontal");
    float vertical = Input.GetAxisRaw("Vertical");

    Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

    // 카메라 forward/right의 XZ 수평 방향만 사용
    Vector3 flatForward = Vector3.ProjectOnPlane(cameraHolder.forward, Vector3.up).normalized;
    Vector3 flatRight = Vector3.ProjectOnPlane(cameraHolder.right, Vector3.up).normalized;

    // 평면 기반 이동 방향 계산
    Vector3 moveDir = flatForward * inputDir.z + flatRight * inputDir.x;

    Vector3 move = moveDir * speed;
    move.y = velocity.y;

    controller.Move(move * Time.deltaTime);

    animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
    animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);
}




*/