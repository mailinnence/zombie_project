using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WeadponModel;

public class Weapon : MonoBehaviour
{
    public static Weapon Instance { get; private set; }

    public Camera playerCamera;

    [Header("Gun")]
    // case
    public CaseShotTrigger caseShotTrigger;

    // Shooting
    public bool isShooting, readyToShoot; // 총을 쏘고 있는 중, 총을 쏠수 있는 상태
    bool allowReset = true;               
    public float shootingDelay = 2f;      // 총 딜레이

    // Burst
    public int bulletsPerBurst = 3;       // 한 번 마우스를 눌렀을 때 연속으로 몇 발을 쏠지
    public int burstBulletsLeft;          // 현재 연사 중 남은 발사 횟수를 추적하는 변수

    // Spread
    public float spreadIntensity;         // 총이 퍼지는 정도

    // Loading 
    public float reloadTime;               // 장전 시간
    public bool isReloading;                // 재장전 여부

    public enum ShootingMode              // 총의 상태 - 단일, 점사, 연발
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;
    public Transform bulletParent; // 풀링된 총알의 부모 오브젝트

    // Bullet
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f; // seconds

    // Object Pooling
    public List<(GameObject bullet, Rigidbody rb)> bulletPool = new List<(GameObject, Rigidbody)>();

    // effect
    public GameObject muzzleEffecrt;
    public WeaponModel thisWeaponModel;


    private Animator animator;


    void OnEnable()
    {
        UI_Manager.instance.UpdateBulletUI(thisWeaponModel); // ui 변경
    }



    private void Awake()
    {
        Instance = this;
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();

    }


    private (GameObject bullet, Rigidbody rb) GetPooledBullet()
    {
        foreach (var pooledBullet in bulletPool)
        {
            if (!pooledBullet.bullet.activeInHierarchy)
            {
                pooledBullet.rb.linearVelocity = Vector3.zero;
                pooledBullet.rb.angularVelocity = Vector3.zero;
                return pooledBullet;
            }
        }

        GameObject newBullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        Rigidbody rb = newBullet.GetComponent<Rigidbody>();
        newBullet.SetActive(false);
        var newPooledBullet = (bullet: newBullet, rb: rb);
        bulletPool.Add(newPooledBullet);
        return newPooledBullet;
    }



    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1) 
        && weaponManager.instance.currentWeapon == 0 
        && !player_anim.Instance.parryingSuccess)
        {
            player_anim.Instance.parry();
        }

        if(Input.GetKeyDown(KeyCode.Mouse1) 
        && weaponManager.instance.currentWeapon == 0 
        && player_anim.Instance.parryingSuccess 
        && UI_Manager.instance.physical_cur >= 30)
        {
            UI_Manager.instance.physical_cur -= 30;
            player_anim.Instance.blow();
            zombie.Instance.Knockback_hit_1();
        }

        if(Input.GetKeyDown(KeyCode.Q) 
        && weaponManager.instance.currentWeapon == 0 
        && UI_Manager.instance.physical_cur >= 30)
        {
            UI_Manager.instance.physical_cur -= 30;
            player_anim.Instance.kick();
            zombie.Instance.Knockback_hit_2();
        }
        
        if (currentShootingMode == ShootingMode.Auto)
        {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        // R 을 누르면 탄창의 갯수보다 남은 탄창 수보다 작고 장전 중이 아닐경우
        if(Input.GetKeyDown(KeyCode.R) 
        && bullet_manager.instance.bulletData[thisWeaponModel].currentBullet < bullet_manager.instance.bulletData[thisWeaponModel].magazineSize 
        && !isReloading)
        {
            Reload();
        }



        if (readyToShoot && isShooting && !isReloading && !Throwable_action.instance.throw_)
        {
            if(!weaponManager.instance.action)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }
        }
        

        // 총알이 없으면 작동x
        if(bullet_manager.instance.bulletData[thisWeaponModel].currentBullet <= 0)
        {
            animator.SetBool("empty" , true);
        }
        else
        {
            animator.SetBool("empty" , false);
        }
        
    }



private void FireWeapon()
{
    // 총알이 없으면 작동x
    if (bullet_manager.instance.bulletData[thisWeaponModel].currentBullet <= 0)
    {
        SoundManager.Instance.empty();
        return;
    }

    // 총알 차감 및 UI 업데이트
    bullet_manager.instance.bulletData[thisWeaponModel].currentBullet--;
    UI_Manager.instance.UpdateBulletUI(thisWeaponModel);

    // 애니메이션
    if (weaponManager.instance.currentWeapon == 0)
    {
        player_anim.Instance.pistol_shot();
    }
    else if (weaponManager.instance.currentWeapon >= 1 && weaponManager.instance.currentWeapon <= 2)
    {
        player_anim.Instance.rifle_shot();
    }
    else if (weaponManager.instance.currentWeapon == 3)
    {
        player_anim.Instance.rifle_shot_shortgun();
    }
    else
    {
        player_anim.Instance.rifle_bigShot();
    }

    // 총구 화염 이펙트
    muzzleEffecrt.GetComponent<ParticleSystem>().Play();

    animator.SetTrigger("recoil");
    SoundManager.Instance.PlayShootingSound(thisWeaponModel);

    if (caseShotTrigger != null)
    {
        caseShotTrigger.shot();
    }

    readyToShoot = false;

    // 샷건일 경우 여러 발 발사
    if (weaponManager.instance.currentWeapon == 3) // 샷건
    {
        int pelletsPerShot = 6;

        // 1. 한 번의 Raycast 기준으로 방향 6개 미리 계산
        Vector3[] pelletDirections = CalculateDirectionsAndSpread_shortGun(pelletsPerShot);

        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 shootingDirection = pelletDirections[i];

            var pooledBullet = GetPooledBullet();
            GameObject bullet = pooledBullet.bullet;
            Rigidbody rb = pooledBullet.rb;

            bullet.transform.position = bulletSpawn.position;
            bullet.transform.rotation = Quaternion.LookRotation(bulletSpawn.forward);
            bullet.transform.SetParent(null);
            bullet.SetActive(true);

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            // rb.AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
            rb.linearVelocity = shootingDirection * bulletVelocity;

            StartCoroutine(DeactivateBulletAfterTime(bullet, bulletPrefabLifeTime));
        }
    }
    else
    {
        // 일반 총기 발사 로직
        Vector3 shootingDirection = CalcuateDirectionAndSpread().normalized;


        var pooledBullet = GetPooledBullet();
        GameObject bullet = pooledBullet.bullet;
        Rigidbody rb = pooledBullet.rb;

        bullet.transform.position = bulletSpawn.position;
        bullet.transform.rotation = Quaternion.LookRotation(bulletSpawn.forward);
        bullet.transform.SetParent(null);
        bullet.SetActive(true);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // rb.AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        rb.linearVelocity = shootingDirection * bulletVelocity;

        StartCoroutine(DeactivateBulletAfterTime(bullet, bulletPrefabLifeTime));
    }

    // 쿨다운 타이머
    if (allowReset)
    {
        Invoke("ResetShot", shootingDelay);
        allowReset = false;
    }

    // 점사 모드일 경우 연속 발사
    if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
    {
        burstBulletsLeft--;
        Invoke("FireWeapon", shootingDelay);
    }
}



    private void Reload() // 재장전
    {
        if(weaponManager.instance.currentWeapon == 0)
        {
            player_anim.Instance.pistol_reload();
        }
        else
        {
            player_anim.Instance.rifle_reload();
        }

        SoundManager.Instance.PlayReloadSound(thisWeaponModel);
        isReloading = true;
        Invoke("ReloadCompleted" , reloadTime);
    }


    private void ReloadCompleted()
    {
        // 현재 총이 보유중인 >= 탄창 크기 -= (탄창 크기 - 현재 가지고 있는 총알) >> 현재 가지고 있는 총알 += 탄창 크기
        if(bullet_manager.instance.bulletData[thisWeaponModel].reserveBullet >= bullet_manager.instance.bulletData[thisWeaponModel].magazineSize)
        {
            bullet_manager.instance.bulletData[thisWeaponModel].reserveBullet -= bullet_manager.instance.bulletData[thisWeaponModel].magazineSize - bullet_manager.instance.bulletData[thisWeaponModel].currentBullet;
            bullet_manager.instance.bulletData[thisWeaponModel].currentBullet = bullet_manager.instance.bulletData[thisWeaponModel].magazineSize;
        }

        // 현재 총이 보유중인 < 탄창 크기 -= (현재 총이 보유중인 == 0) >> 현재 가지고 있는 총알 += 현재 총이 보유중인 
        else if(bullet_manager.instance.bulletData[thisWeaponModel].reserveBullet < bullet_manager.instance.bulletData[thisWeaponModel].magazineSize)
        {
            bullet_manager.instance.bulletData[thisWeaponModel].reserveBullet = 0;
            bullet_manager.instance.bulletData[thisWeaponModel].currentBullet += bullet_manager.instance.bulletData[thisWeaponModel].reserveBullet; 
        }
        UI_Manager.instance.UpdateBulletUI(thisWeaponModel);
        isReloading = false;
    }


    public void Ammo_Supply(WeaponModel weapon)
    {
        if(bullet_manager.instance.bulletData[weapon].maxBullet >= bullet_manager.instance.bulletData[weapon].reserveBullet + bullet_manager.instance.bulletData[weapon].AmmoSupply) 
        {
            bullet_manager.instance.bulletData[weapon].reserveBullet += bullet_manager.instance.bulletData[weapon].AmmoSupply;
        }
        else
        {
            bullet_manager.instance.bulletData[weapon].reserveBullet = bullet_manager.instance.bulletData[weapon].maxBullet;
        }

        if(thisWeaponModel == weapon)
        {
            UI_Manager.instance.UpdateBulletUI(weapon);
        }
    }


    public void bomo_Supply()
    {
        bullet_manager.instance.bulletData[WeaponModel.grenade].currentBullet += 1; 
        UI_Manager.instance.UpdateBombUI();
    }


    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }


    public Vector3 CalcuateDirectionAndSpread()
    {
        // 화면의 정중앙에서 카메라 기준으로 전방 방향의 Ray(광선)를 생성
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        
        // ray를 레이캐스트 해서 뭔가(벽, 바닥, 오브젝트 등)에 부딪혔다면, 그 충돌 지점(hit.point) 을 타겟 지점(targetPoint) 으로 사용
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            // 아무것도 안 맞았다면, 광선을 100 유닛 전방으로 직진시킨 지점을 targetPoint로 사용
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return (direction + new Vector3(0, y, z)).normalized;
    }



    public Vector3[] CalculateDirectionsAndSpread_shortGun(int pelletCount = 6)
    {
        // 화면 중앙에서 Ray 생성
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;

        // Raycast 결과로 목표 지점 결정
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 baseDirection = targetPoint - bulletSpawn.position;

        Vector3[] directions = new Vector3[pelletCount];

        for (int i = 0; i < pelletCount; i++)
        {
            // Y, Z 방향으로 퍼짐 적용 (X는 무시)
            float ySpread = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
            float zSpread = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

            Vector3 spreadDirection = baseDirection + new Vector3(0, ySpread, zSpread);
            directions[i] = spreadDirection.normalized;
        }

        return directions;
    }



    private IEnumerator DeactivateBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        bullet.SetActive(false); // 총알 비활성화
    }
}