using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField] float delay = 3f;
    [SerializeField] float damageRadius = 20f;
    [SerializeField] float explosionForce = 1200f;

    float countdown;

    bool hasExploded = false;
    public bool hasBeenThrown = false;

   Rigidbody rigid;
   bool hasTouchedGround = false;

    public enum ThrowableType
    {
        Grenade
    }

    public ThrowableType throwableType;


    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        countdown = delay;
        hasExploded = false;
        hasBeenThrown = false;
        hasTouchedGround = false;
    }



    private void Update()
    {
        if (hasBeenThrown)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f && !hasExploded)
            {
                Explode();
                hasExploded = true;
            }
        }
    }

    private void Explode()
    {
        GetThrowableEffect();

        gameObject.SetActive(false);
        // Destroy(gameObject);
    }


    private void GetThrowableEffect()
    {
        switch(throwableType)
        {
            case ThrowableType.Grenade:
                GrenadeEffect();
                break;
        }
        
    }

  
    private void GrenadeEffect()
    {
        // visual effect
        GameObject explosionEffect = GlobalReferences.Instance.grenadeExplosionEffect;
        // Instantiate(explosionEffect, transform.position, transform.rotation);
        GameObject bomb = PoolManager.instance.Get(1);   
        bomb.transform.position = transform.position;  // 위치 설정
        bomb.transform.rotation = transform.rotation;  // 회전 설정


        // Play Sound
        SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.grenadeSound);


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
                enemy.hp -= 100;
            }
        }

    }



    void OnCollisionEnter(Collision collision)
    {
        if (!hasTouchedGround && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            hasTouchedGround = true;

            // 중력 저항 증가로 굴러가는 속도 줄이기
            rigid.linearDamping = 4f;
            rigid.angularDamping = 4f;


        }
    }

}
