using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WeadponModel;

public class BulletInfo
{
    public int currentBullet;     // 현재 장전된 총알 수
    public int magazineSize;      // 탄창 크기
    public int reserveBullet;     // 보유한 여분 총알 수
    public int maxBullet;     // 보유한 여분 총알 수
    public int AmmoSupply;     // 현재 장전된 총알 수
    public int grenade;


    public BulletInfo(int magazineSize, int reserveBullet)
    {
        this.currentBullet = magazineSize; // 처음엔 풀장전
        this.magazineSize = magazineSize;
        this.reserveBullet = reserveBullet;
        this.maxBullet = reserveBullet;
        this.AmmoSupply = magazineSize * 2;
    }
}



public class bullet_manager : MonoBehaviour
{

    public Dictionary<WeaponModel, BulletInfo> bulletData;

    public static bullet_manager instance { get; private set; }



    void Awake()
    {
        instance = this;
        // 무기별 초기값 설정
        bulletData = new Dictionary<WeaponModel, BulletInfo>
        {
            { WeaponModel.pistol,   new BulletInfo(30, 120) },
            { WeaponModel.akm,      new BulletInfo(50, 250) },
            { WeaponModel.ak47,     new BulletInfo(40, 160) },
            { WeaponModel.shortgun, new BulletInfo(12, 84) },
            { WeaponModel.reaper,   new BulletInfo(1, 5) },
            { WeaponModel.mac,      new BulletInfo(1, 5) },
            { WeaponModel.grenade,  new BulletInfo(1, 3) }
        };
    }

    void Start()
    {
 
    }



    void Update()
    {

    }






    void test()
    {
        // 예시: F키를 누르면 pistol 총알 정보 출력
        if (Input.GetKeyDown(KeyCode.F))
        {
            var pistolInfo = bulletData[WeaponModel.pistol];
            Debug.Log($"Pistol ▶ 탄창: {pistolInfo.currentBullet}/{pistolInfo.magazineSize}, 여분: {pistolInfo.reserveBullet}");
        }
    }

}
