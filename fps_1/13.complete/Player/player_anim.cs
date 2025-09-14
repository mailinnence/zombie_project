using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_anim : MonoBehaviour
{

    public int currentWeapon;
    public int changeWeapon;
    public bool parryingSuccess; 
    public bool parryingSuccess_one; 

    public static player_anim Instance { get; private set; }

    private Animator anim;
    public GameObject parryingObject;



    void Awake()
    {
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


    void Update()
    {
        if(parryingSuccess && !parryingSuccess_one)
        {
            parryingSuccess_one = true;
            StartCoroutine(ResetParryingSuccessAfterDelay(3f));

        }
    }

   


    // 수료탄
    public void pistol_grenade()
    {
        anim.SetTrigger("pistol_grenade");
    }

    public void rifle_grenade()
    {
        anim.SetTrigger("rifle_grenade");
    }



    public void grenade_anim()
    {
        Throwable_action.instance.ThrowLethal();
    }


    public void grenade_deactivate_weapon()
    {
        weaponManager.instance.SwitchWeapon();
    }






    // 권총 
    public void pistol_shot()
    {
        anim.SetTrigger("pistol_shot");
    }

    public void pistol_down()
    {
        anim.SetTrigger("pistol down");
    }

    public void pistol_up()
    {
        anim.SetTrigger("pistol up");
    }

    public void pistol_reload()
    {
        anim.SetTrigger("pistol_reload");
    }



    // 라이플
    public void rifle_shot()
    {
        anim.SetTrigger("rifle_shot");
    }

    public void rifle_shot_shortgun()
    {
        anim.SetTrigger("rifle_shot_shortgun");
    }    

    public void rifle_bigShot()
    {
        anim.SetTrigger("rifle_BigShot");
    }    


    public void rifle_down()
    {
        anim.SetTrigger("rifle down");
    }

    public void rifle_up()
    {
        anim.SetTrigger("rifle up");
    }

    public void rifle_reload()
    {
        anim.SetTrigger("rifle_reload");
    }


    public void change_weapon()
    {
        weaponManager.instance.weapon_change_up();
    }


    public void change_weapon_camera()
    {
        weaponManager.instance.weapon_change_camera();
    }


    public void change_weapon_activate()
    {
        weaponManager.instance.weapon_activate();
    }



    public void action_reset()
    {
        weaponManager.instance.action_reset();
    }


    public void parry()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            anim.SetTrigger("parry"); 
            // 무기를 안 보이게 만드는 비활성화 함수 필요
            // 애니메이션 변수값을 통한 idle 전환
            // 패링 중 공격, 대쉬, 무기전환 금지 구현 
        }
        
    }

    public void parry_counter()
    {
        // 변수를 주고 그때에 공격을 맞았다면 그 대상이 앞에 있을겨우 공격 가능
    }


    // 수월하게 구현하기 위해서는 좀비가 부서지는 함수가 필요하다 + 발차기





    public void kick()
    {
        anim.SetTrigger("kick"); 
    }




    public void blow()
    {
        
        anim.SetTrigger("blow"); 

    }




    public void camera_parryOn()
    {
        Camera.main.nearClipPlane = 0.2f;
        parryingObject.layer = LayerMask.NameToLayer("parrying");
    }

    public void camera_parryOff()
    {
        
        Camera.main.nearClipPlane = 0.3f;
        parryingObject.layer = LayerMask.NameToLayer("Default");
    }



        
    private IEnumerator ResetParryingSuccessAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        parryingSuccess = false;
        parryingSuccess_one = false;
    }





}
