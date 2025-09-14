using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CaseShotTrigger : MonoBehaviour
{

    private Animator anim;


    void Awake()
    {
        anim = GetComponent<Animator>(); // ✅ this.anim 에 할당
    }


    public void shot()
    {
        int rand = Random.Range(0, 3); // 0, 1, 2 중 랜덤
        anim.SetInteger("random", rand);
        anim.SetTrigger("shot");
           
    }

}
