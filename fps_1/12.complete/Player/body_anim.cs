using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class body_anim : MonoBehaviour
{

    public static body_anim Instance { get; private set; }

    private Animator anim;

    void Awake()
    {
        // 싱글톤 중복 방지
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    // 움직임 처리
    public void SetMoveTrue()
    {
        if (anim != null)
        {
            anim.SetBool("move", true);
        }
    }

    // 움직임 정지
    public void SetMoveFalse()
    {
        if (anim != null)
        {
            anim.SetBool("move", false);
        }
    }


    // 권총 초기화
    public void SetPistol_init()
    {
        anim.SetTrigger("pistol idle");
    }


    // 라이플 초기화
    public void Setrifle_init()
    {
        anim.SetTrigger("rifle idle");
    }


    public void foot_sound()
    {
        SoundManager.Instance.foot.PlayOneShot(SoundManager.Instance.foot_);
    }




}
