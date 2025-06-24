using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  


public class Weapon : MonoBehaviour
{

    // public Camera playerCamera;

    // Shooting
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    // Burst
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    // Spread
    public float spreadIntensity;


    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;


    // Bullet
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f; // seconds

    public GameObject muzzleEffecrt;
    private Animator animator;


    // Loading 
    public float reloadTime;               // 장전 시간
    public int magazineSize, bulletsLeft;   // 탄창, 남은 탄창 수
    public bool isReloading;                // 재장전 여부

    // UI
    // public TextMeshProUGUI ammoDisplay; // 삭제


    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize; // 탄창 사이즈 만큼 처음에 남은 탄창을 초기화한다.
    }


    void Update()
    {

        if(bulletsLeft == 0&& isShooting)
        {
            SoundManager.Instance.emptyManagizeSound.Play();
        }


        if (currentShootingMode == ShootingMode.Auto)
        {
            // Holding Down Left Mouse Button
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if(currentShootingMode == ShootingMode.Single ||
            currentShootingMode == ShootingMode.Burst)
        {
            // Clicking Left Mouse Button Once
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        // R 을 누르면 탄창의 갯수보다 남은 탄창 수보다 작고 장전 중이 아닐경우
        if(Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading)
        {
            Reload();
        }

        // If you want to automatically reload when magazine is empty 
        if (readyToShoot && !isShooting && !isReloading && bulletsLeft <= 0)
        {
            Reload(); // 단 이렇게 하면 재장전 사운드가 들리지 않음 - 선택사항
        }


        if (readyToShoot && isShooting && bulletsLeft > 0 && !isReloading)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }


        if(AmmoManager.Instance.ammoDisplay != null)
        {
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft/bulletsPerBurst}/{magazineSize/bulletsPerBurst}";
        }

    }


    private void FireWeapon()
    {

        bulletsLeft--; // 한발 당 -1
        
        // camera_shake.instance.Shake(); // 카메라 쉐이크
        muzzleEffecrt.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("recoil_1");
        SoundManager.Instance.pm_40.Play();


        readyToShoot = false;

        Vector3 shootingDirection = CalcuateDirectionAndSpread().normalized;

        // 총알 생성
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        
        // Poiting the bulle to face tje shooting direction
        bullet.transform.forward = shootingDirection;

        
        // 총알 방향
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection.normalized * bulletVelocity, ForceMode.Impulse);



        // 총알에 특정 시간 전까지 파괴되지 않음
        StartCoroutine(DestoryBulletAfterTime(bullet, bulletPrefabLifeTime));



        // Checking if we are done shooting
        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // Burst Mode
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }   


    private void Reload() // 재장전
    {
        SoundManager.Instance.reloadingSound.Play();

        isReloading = true;
        Invoke("ReloadCompleted" , reloadTime);
    }

    private void ReloadCompleted()
    {
        bulletsLeft = magazineSize;
        isReloading = false;
    }


    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }


    public Vector3 CalcuateDirectionAndSpread()
    {
        // Shooting from the middle of the screen to check where are we pointing at
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            // Hitting Something
            targetPoint = hit.point;
        }
        else
        {
            // Shooting at the air
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        // Returning the shooting direction and spread
        return direction + new Vector3(x, y, 0);

    }


    private IEnumerator DestoryBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }


}
