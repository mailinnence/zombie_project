using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("점프 관련 변수")]
    private CharacterController controller; 
    public Transform cameraHolder;     // CameraParent 또는 CameraRig를 drag & drop
    public float speed = 12f;          // 이동 속도
    public float gravity = -9.81f * 2; // 중력
    public float jumpHeight = 3f;      // 점프 높이
    private int jumpCount;          // 현재까지의 점프 횟수
    public int maxJumpCount = 2;    // 최대 점프 횟수


    [Header("착지 관련 변수")]
    public Transform groundCheck;       // 표면 센서
    public float groundDistance = 0.4f; // 표면 거리 기준
    public LayerMask groundMask;        // 표면 레이어


    private Vector3 move;
    Vector3 velocity;                   // 현재 속도
    bool isGrounded;                    // 지면에 닿았는지 여부

    private Vector3 lastPosition = Vector3.zero;
    bool isMoving;

    [Header("대시 관련")]
    public float dashSpeed = 30f;
    public float dashDuration = 0.2f;
    private bool isDashing = false;
    private Vector3 dashDirection;
    private Vector3 lastMoveDirection = Vector3.zero;
    private int airdash; // 공중에서는 한번만 할 수 있어야 한다.


    void Start()
    {
        controller = GetComponent<CharacterController>();
        airdash = 0;
    }

    private bool canDoubleJump = false;

    void Update()
    {
       
        UpdateBodyAnim();


        if (!isDashing && Input.GetKeyDown(KeyCode.LeftShift) && airdash == 0 )
        {
            if(isGrounded)
            {
                StartCoroutine(Dash());
            }
            else
            {
                airdash = 1;
                StartCoroutine(Dash());
            }
        }


        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            jumpCount = 0;
            canDoubleJump = true; // 지면에 닿았을 때만 2단 점프 가능
            airdash = 0;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        move = cameraHolder.right * x + cameraHolder.forward * z;
        move.y = 0f;
        

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") )
        {
            if (isGrounded)
            {
                // 1단 점프 (지면에서)
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpCount = 1;
                SoundManager.Instance.jump.PlayOneShot(SoundManager.Instance.jump_);
            }
            else if (jumpCount == 0)
            {
                // 공중에서 시작했을 때 1단 점프 허용
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpCount = 1;
                canDoubleJump = false; // 공중 시작은 2단 점프 안 됨
                SoundManager.Instance.jump.PlayOneShot(SoundManager.Instance.jump_);
            }
            else if (jumpCount == 1 && canDoubleJump)
            {
                // 지면에서 시작한 경우에만 허용되는 2단 점프
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpCount = 2;
                canDoubleJump = false;
                SoundManager.Instance.jump.PlayOneShot(SoundManager.Instance.jump_);
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (lastPosition != transform.position && isGrounded)
            isMoving = true;
        else
            isMoving = false;

        lastPosition = transform.position;

    }



    private IEnumerator Dash()
    {
        // WASD 키 중 하나라도 누르고 있는지 체크
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        {
            yield break; // 아무것도 안 누르고 있으면 대시 안 함
        }
        else
        {
            SoundManager.Instance.dash.PlayOneShot(SoundManager.Instance.dash_);
            lastMoveDirection = move.normalized;
        }


        isDashing = true;
        // MouseMovement.instance.offMouse = true;

        dashDirection = lastMoveDirection;
        dashDirection.y = 0f;

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            controller.Move(dashDirection.normalized * dashSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        MouseMovement.instance.offMouse = false;
    }


    void UpdateBodyAnim()
    {
        if (isGrounded && !isDashing && isMoving)
        {
            if (body_anim.Instance != null)
                body_anim.Instance.SetMoveTrue();
        }
        else
        {
            if (body_anim.Instance != null)
                body_anim.Instance.SetMoveFalse();
        }
    }




    // void ads_view()
    // {
    //     if (Input.GetKeyDown(KeyCode.V) && isGrounded && !isDashing && !ads)
    //     {
    //         if(weaponManager.instance.currentWeapon == 0 && !ads)
    //         {
    //             ads = true;
    
    //             body_anim.Instance.SetPistol_ads_on();
    //         }
            
    //     }
    //     else if(Input.GetKeyDown(KeyCode.V) && ads)
    //     {
    //         if(weaponManager.instance.currentWeapon == 0 && ads)
    //         {
    //             ads = false;
                
    //             body_anim.Instance.SetPistol_ads_off();
    //         }
    //     }
    // }





}

