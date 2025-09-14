using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet  : MonoBehaviour
{


    [Header("범위 탄환")]
    [SerializeField] float damageRadius = 8f;
    [SerializeField] float explosionForce = 1200f;




    private void OnCollisionEnter(Collision objectWeHit)
    {
        if (objectWeHit.gameObject.CompareTag("Target") 
        || objectWeHit.gameObject.CompareTag("wall"))
        {
            CreateBulletImpactEffect(objectWeHit);
            gameObject.SetActive(false);
        }


        if (objectWeHit.gameObject.CompareTag("zombie"))
        {
            CreateBulletImpactEffect_zombie(objectWeHit);
            gameObject.SetActive(false);
        }


        if (objectWeHit.gameObject.CompareTag("Beer"))
        {
            objectWeHit.gameObject.GetComponent<BeerBottle>().Shatter();
        
            // We will not destroy the bullet on impact, it will get destroyed according to its lifetime
        }



    }


 


    // 오브젝트 풀링o
    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0]; // 처음 충돌한 오브젝트
        
        // PoolManager를 사용해 bulletImpactEffectPrefab을 가져옴
        if(weaponManager.instance.currentWeapon < 4)
        {
            GameObject hole = PoolManager.instance.Get(0);          // 구멍 오브젝트
            hole.transform.position = contact.point;                // 구멍 오브젝트의 위치를 부딪힌 객체에 맞춘다. 
            hole.transform.rotation = Quaternion.LookRotation(contact.normal); // 부딪힌 객체의 기울기를 맞춘다.
        
            hole.transform.SetParent(objectWeHit.gameObject.transform);   // 
            hole.SetActive(true); // 풀에서 가져온 오브젝트 활성화
        }
        else if(weaponManager.instance.currentWeapon == 4)
        {
            GameObject hole = PoolManager.instance.Get(4);          // 구멍 오브젝트
            hole.transform.position = contact.point;                // 구멍 오브젝트의 위치를 부딪힌 객체에 맞춘다. 
            hole.transform.rotation = Quaternion.LookRotation(contact.normal); // 부딪힌 객체의 기울기를 맞춘다.
        
            // hole.transform.SetParent(objectWeHit.gameObject.transform);   // 
            hole.SetActive(true); // 풀에서 가져온 오브젝트 활성화
            cannon();
            SoundManager.Instance.explode_gun.Play();
        }
        else
        {
            GameObject hole = PoolManager.instance.Get(5);          // 구멍 오브젝트
            hole.transform.position = contact.point;                // 구멍 오브젝트의 위치를 부딪힌 객체에 맞춘다. 
            hole.transform.rotation = Quaternion.LookRotation(contact.normal); // 부딪힌 객체의 기울기를 맞춘다.
        
            // hole.transform.SetParent(objectWeHit.gameObject.transform);   // 
            hole.SetActive(true); // 풀에서 가져온 오브젝트 활성화
            time_Gun();
            SoundManager.Instance.explode_gun.Play();
        }
    }



    // 오브젝트 풀링o
    void CreateBulletImpactEffect_zombie(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0]; // 처음 충돌한 오브젝트
        
        if(weaponManager.instance.currentWeapon < 4)
        {
            // PoolManager를 사용해 bulletImpactEffectPrefab을 가져옴
            GameObject hole = PoolManager.instance.Get(3);          // 구멍 오브젝트
            hole.transform.position = contact.point;                // 구멍 오브젝트의 위치를 부딪힌 객체에 맞춘다. 
            hole.transform.rotation = Quaternion.LookRotation(contact.normal); // 부딪힌 객체의 기울기를 맞춘다.
            
            hole.transform.SetParent(objectWeHit.gameObject.transform);   // 
            hole.SetActive(true); // 풀에서 가져온 오브젝트 활성화


            objectWeHit.gameObject.GetComponent<zombie>().damage();
        }
        else if(weaponManager.instance.currentWeapon == 4)
        {
            GameObject hole = PoolManager.instance.Get(4);          // 구멍 오브젝트
            hole.transform.position = contact.point;                // 구멍 오브젝트의 위치를 부딪힌 객체에 맞춘다. 
            hole.transform.rotation = Quaternion.LookRotation(contact.normal); // 부딪힌 객체의 기울기를 맞춘다.
        
            // hole.transform.SetParent(objectWeHit.gameObject.transform);   // 
            hole.SetActive(true); // 풀에서 가져온 오브젝트 활성화
            cannon();
            SoundManager.Instance.explode_gun.Play();
        }
        else
        {
            GameObject hole = PoolManager.instance.Get(5);          // 구멍 오브젝트
            hole.transform.position = contact.point;                // 구멍 오브젝트의 위치를 부딪힌 객체에 맞춘다. 
            hole.transform.rotation = Quaternion.LookRotation(contact.normal); // 부딪힌 객체의 기울기를 맞춘다.
        
            // hole.transform.SetParent(objectWeHit.gameObject.transform);   // 
            hole.SetActive(true); // 풀에서 가져온 오브젝트 활성화

            time_Gun();
            SoundManager.Instance.explode_gun.Play();
        }
        UI_Manager.instance.physical_cur += 2;
    }



    void cannon()
    {

        // physical effect
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }
            
            // 좀비 스크립트가 있을 때만 데미지 적용
            zombie enemy = objectInRange.GetComponent<zombie>();
            if (enemy != null)
            {
                enemy.gameObject.GetComponent<zombie>().damage();
            }
        }

    }



    void time_Gun()
    {
        // physical effect
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }
            
            // 좀비 스크립트가 있을 때만 데미지 적용
            zombie enemy = objectInRange.GetComponent<zombie>();
            if (enemy != null)
            {
                enemy.TimeGun();
            }
        }
    }




    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }

}
