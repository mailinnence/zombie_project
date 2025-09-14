using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class zombie : MonoBehaviour
{


    public static zombie Instance { get; private set; }
    private Animator animator;

    [Header("생존 여부")]
    public float hp;
    public bool alive;

    [Header("추적 여부")]
    public Transform target_player;
    public Vector3 originPosition;
    UnityEngine.AI.NavMeshAgent agent;

    [Header("경직 여부")]
    public bool hit;
    public bool stop;

    [Header("공격 여부")]
    public bool attack_;
    public float distanceToPlayer;
    public float attackRange = 2.0f;

    [Header("넉뱃 여부")]
    public float knockbackDistance = 0.5f;  // 밀리는 거리
    public float knockbackDuration = 0.2f; // 밀리는 시간

    private bool isKnockback = false;
    
    private CharacterController characterController;
    private CapsuleCollider capsuleCollider;


    public enum zombie_type              
    {
        zombie_1,  // 기본 몬스터 : 걸어오며 거리가 주러지면 공격 대쉬가 없음
        zombie_2,  // 중위 몬스터(근접) : 공격을 할때 대쉬 공격을 감행함 치고 빠지는 게임 메인 레벨 디자인 
        zombie_3,  // 중위 몬스터(원거리-1) : 멀리서 공격하며 공격을 맞으면 도망감
        zombie_4,  // 중위 몬스터(원거리-2) : 멀리서 공격하며 탱커형
        zombie_5,  // 중위 몬스터(원거리 - 공중) : 멀리서 공격하며 공격을 맞으면 도망감
        zombie_6,  // 상위 몬스터(근접 - 투명) : 천천히 다가와서 공격함. 공격을 맞을 경우 도망가지만 특정 거리안쪽으로 들어오면 싸움 속도가 빠름
        zombie_7,  // 상위 몬스터(상위 체급) : 공격을 맞으면 뒤로 튕겨남 패링 시 튕겨나나 보상 작용으로 킥 사용 가능
    }

    public zombie_type type;


    private void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        characterController = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        alive = true;
        originPosition = transform.position;
        InitializeZombieByType_hp(); // 체력 초기화
    } 


    void Update()
    {
        InitializeZombieByType_action(); // 각 객체에 맞게 행동 패턴
        death();
    }
   


    void InitializeZombieByType_hp()
    {
        switch (type)
        {
            case zombie_type.zombie_1: // 기본 몬스터 설정
                hp = 100f;
                break;
            case zombie_type.zombie_2: // 중위 몬스터(근접) 설정
                hp = 150f;
                break;
            case zombie_type.zombie_3: // 중위 몬스터(원거리-1) 설정
                hp = 120f;
                break;
            case zombie_type.zombie_4: // 중위 몬스터(원거리-2) 설정
                hp = 200f;
                break;
            case zombie_type.zombie_5: // 중위 몬스터(원거리 - 공중) 설정
                hp = 110f;
                break;
            case zombie_type.zombie_6: // 상위 몬스터(근접 - 투명) 설정
                hp = 180f;
                break;
            case zombie_type.zombie_7: // 상위 몬스터(상위 체급) 설정
                hp = 300f;
                break;
            default:
                break;
        }
    }



    void InitializeZombieByType_action()
    {
        if(!alive || stop)
        {
            agent.isStopped = true;
            return;
        }
        switch (type)
        {
            case zombie_type.zombie_1: // 기본 몬스터 설정
                zombie_1();
                break;
            case zombie_type.zombie_2: // 중위 몬스터(근접) 설정
     
                break;
            case zombie_type.zombie_3: // 중위 몬스터(원거리-1) 설정
      
                break;
            case zombie_type.zombie_4: // 중위 몬스터(원거리-2) 설정
  
                break;
            case zombie_type.zombie_5: // 중위 몬스터(원거리 - 공중) 설정
         
                break;
            case zombie_type.zombie_6: // 상위 몬스터(근접 - 투명) 설정
      
                break;
            case zombie_type.zombie_7: // 상위 몬스터(상위 체급) 설정
      
                break;
            default:
                break;
        }
    }

    private void OnEnable()
    {
        alive = true;
        hp = 100f;
        animator.SetTrigger("re"); // 부활 또는 초기화 트리거
        characterController.enabled = true;
        capsuleCollider.enabled = true;
    }



    void death()
    {
        if(hp <= 0 && alive)
        {
            agent.isStopped = true;
            hp = 0;
            alive = false;

            // 0 또는 1 중 랜덤한 값 설정
            int randomValue = Random.Range(0, 2); // 0 이상 2 미만 → 0 또는 1
            animator.SetInteger("rand", randomValue);

            animator.SetTrigger("death");
            SoundManager.Instance.zombie_idle_sound.Stop();
            SoundManager.Instance.zombie_chasing_sound.Stop();
            SoundManager.Instance.zombie_death_sound.Stop();
            SoundManager.Instance.zombie_attack_sound.Stop();

            
            SoundManager.Instance.PlayZombieSound("death");
            characterController.enabled = false;
            capsuleCollider.enabled = false;
            StartCoroutine(DisableAfterDelay(5f));

        }
    }


    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }



    void zombie_1()
    {
        // 추적 중 - 공방 - 데미지
        if (target_player != null)
        {
            
            agent.SetDestination(target_player.position);
             
            if(agent.velocity.magnitude > 0.1f)
            {
                animator.SetBool("walk", true);
            }
            else
            {
                animator.SetBool("walk", false);
                distanceToPlayer = Vector3.Distance(transform.position, target_player.position);
                
                if (distanceToPlayer <= attackRange && !attack_)
                {   
                    attack_ = true;
                    animator.SetTrigger("attack");
                    agent.isStopped = true;
          
                }
            }
        }
        else
        {
            agent.SetDestination(originPosition); // 원래 위치로 복귀
        }
    }


    void zombie_attack_init()
    {
        attack_ = false;
        agent.isStopped = false;
    }



    void zombie_attack_damage(int damge)
    {
        if (distanceToPlayer <= attackRange )
        {   
            if(target_player.gameObject.layer == LayerMask.NameToLayer("parrying"))
            {
                animator.SetTrigger("hit");
                player_anim.Instance.parryingSuccess = true;
                UI_Manager.instance.physical_cur += 10;
                Knockback();
            }
            else
            {
                CameraShake.instance.Shake();
                UI_Manager.instance.curHp -= damge;
                UI_Manager.instance.PlayDamageEffect();
                UI_Manager.instance.UpdateHpTEXTUI();

            }
        }
    }




    public void damage()
    {

        if(weaponManager.instance.currentWeapon < 3) { hp -= 15; }
        if(weaponManager.instance.currentWeapon == 3) { hp -= 10; }
        if(weaponManager.instance.currentWeapon == 4) { hp -= 100; }

    
    }


    public void damage_anim()
    {
        hit = false;
        agent.isStopped = false;
    }





    public void TimeGun()
    {
        StartCoroutine(DisableZombieTemporarily(5f));
    }



    // 좀비쪽 코드에서 함수를 만들어서 다시 실행할 것
    private IEnumerator DisableZombieTemporarily(float duration)
    {
        if (animator != null)
        {
            animator.speed = 0f;
        }
        stop = true;

        yield return new WaitForSeconds(duration);


        stop = false;
        if (animator != null)
        {
            animator.speed = 1f;
        }
    }

    

    public void Knockback()
    {
        if (!isKnockback && distanceToPlayer <= attackRange )
        {
            CameraShake.instance.Shake();
            knockbackDistance = 0.5f;  // 밀리는 거리
            knockbackDuration = 0.2f; // 밀리는 시간
            StartCoroutine(KnockbackCoroutine());
        }

    }

    public void Knockback_hit_1()
    {
        if (!isKnockback && distanceToPlayer <= attackRange )
        { 
            CameraShake.instance.Shake();
            hp -= 25;
            if(hp >= 0) { animator.SetTrigger("hit"); }
            
            knockbackDistance = 2f;  // 밀리는 거리
            knockbackDuration = 0.15f; // 밀리는 시간
            StartCoroutine(KnockbackCoroutine());
        }
    }


    public void Knockback_hit_2()
    {
        if (!isKnockback && distanceToPlayer <= attackRange )
        {
            CameraShake.instance.Shake();
            hp -= 25;
            if(hp >= 0) { animator.SetTrigger("hit"); }
            
            knockbackDistance = 2f;  // 밀리는 거리
            knockbackDuration = 0.15f; // 밀리는 시간
            StartCoroutine(KnockbackCoroutine());
        }

    }



    private IEnumerator KnockbackCoroutine()
    {
        isKnockback = true;

        agent.isStopped = true;  // 이동 멈춤

        Vector3 knockbackDir = -transform.forward; // 뒤 방향
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + knockbackDir * knockbackDistance;

        float elapsed = 0f;

        while (elapsed < knockbackDuration)
        {
            agent.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / knockbackDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isKnockback = false;
    }



    public void sound_idle()
    {
        SoundManager.Instance.PlayZombieSound("idle");
    }


    public void sound_attack()
    {
        SoundManager.Instance.PlayZombieSound("attack");
    }


    public void sound_chasing()
    {
        SoundManager.Instance.PlayZombieSound("chasing");
    }


}
