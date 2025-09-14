using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class weaponManager : MonoBehaviour
{

    public int currentWeapon;
    public int changeWeapon;
    public bool action;

    public static weaponManager instance { get; private set; }

    private Animator anim;

    public GameObject[] weapon;

    Dictionary<int, string> weaponIndexToName = new Dictionary<int, string>
    {
        { 0, "pistol" },
        { 1, "akm" },
        { 2, "ak47" },
        { 3, "shortgun" },
        { 4, "reaper" },
        { 5, "mac" }
    };


   void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
       
        
    }

    void Start()
    {
        
        anim = GetComponent<Animator>();

        weapon_init();
    }


    void Update()
    {
        if(!action && !Throwable_action.instance.throw_)
        {
            weapon_mouse_scroll();
            weapon_num();
        }
    }



    void weapon_num()
    {
        for (int i = 1; i <= weapon.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                int selectedWeaponIndex = i - 1;

                if (selectedWeaponIndex < weapon.Length && currentWeapon != selectedWeaponIndex)
                {
                    changeWeapon = selectedWeaponIndex;
                    weapon_change_down(currentWeapon, selectedWeaponIndex);
                }
            }
        }
    }



    void weapon_init()
    {
        // 공통 nearClip 설정 (권총과 그 외 무기 구분)
        Camera.main.nearClipPlane = (currentWeapon == 0) ? 0.3f : 0.1f;

        UI_Manager.instance.Gun_image[currentWeapon].SetActive(true);
        UI_Manager.instance.currentWeapon = weaponIndexToName[currentWeapon];
        
        // 애니메이션 세팅
        if (currentWeapon == 0)
        {
            body_anim.Instance.SetPistol_init();
            player_anim.Instance.pistol_up();
        }
        else
        {
            body_anim.Instance.Setrifle_init();
            player_anim.Instance.rifle_up();
        }
    }



    void weapon_change_down(int currentWeapon_, int changeWeapon_)
    {
        action = true;
        UI_Manager.instance.Gun_image[currentWeapon_].SetActive(false);
        // 총 내리기
        if(currentWeapon == 0)
        {
            player_anim.Instance.pistol_down();
        }
        else
        {
            player_anim.Instance.rifle_down();
        }
        currentWeapon = changeWeapon_;
        changeWeapon = changeWeapon_;
    }




    public void weapon_change_up()
    {
        weapon[currentWeapon].SetActive(false);
    }



    public void weapon_change_camera()
    {
        weapon_init();
    }


    public void weapon_activate()
    {
        for (int i = 0; i < weapon.Length; i++)
        {
            weapon[i].SetActive(i == currentWeapon);
        }
    }


    public void action_reset()
    {
        action = false;
    }



    void weapon_mouse_scroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f) // 마우스 휠 ↑
        {
            changeWeapon = (currentWeapon - 1 + weapon.Length) % weapon.Length;
        }
        else if (scroll < 0f) // 마우스 휠 ↓
        {
            changeWeapon = (currentWeapon + 1) % weapon.Length;
        }
        else
        {
            return;
        }

        if (changeWeapon != currentWeapon)
        {
            weapon_change_down(currentWeapon, changeWeapon);
        }
    }




    public void SwitchWeapon()
    {
        for (int i = 0; i < weapon.Length; i++)
        {
            if (i == currentWeapon)
            {
                weapon[i].SetActive(false); 
            }
        }
    }


}
