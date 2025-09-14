using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WeadponModel; 

public class Throwable_action : MonoBehaviour
{
    public static Throwable_action instance { get; private set; }

    [Header("Grenade")]
    public float throwForce = 40f;
    public GameObject grenadePrefab;
    public GameObject throwableSpawn;
    public float forceMultiplier;

    public bool throw_;



    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G) && !throw_ && bullet_manager.instance.bulletData[WeaponModel.grenade].currentBullet > 0)
        {
            bullet_manager.instance.bulletData[WeaponModel.grenade].currentBullet--;
            UI_Manager.instance.UpdateBombUI();

            throw_ = true;
            StartCoroutine(ResetThrow());
            if(weaponManager.instance.currentWeapon == 0)
            {
                player_anim.Instance.pistol_grenade();
            }
            else
            {
                player_anim.Instance.rifle_grenade();
            }
        }
    }


    IEnumerator ResetThrow()
    {
        yield return new WaitForSeconds(1.8f); // 1초 대기
        throw_ = false;
    }


    public void ThrowLethal()
    {
        GameObject lethalPrefab = grenadePrefab;
        
        // GameObject throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        
        GameObject throwable = PoolManager.instance.Get(2); // 풀에서 가져오기
        // 위치 및 회전 수동 설정
        throwable.transform.SetPositionAndRotation(
            throwableSpawn.transform.position,
            Camera.main.transform.rotation
        );

        Rigidbody rb = throwable.GetComponent<Rigidbody>();
        
        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);
        
        throwable.GetComponent<Throwable>().hasBeenThrown = true;
        
    }

}
