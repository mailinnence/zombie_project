using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    public static PoolManager instance;
    public GameObject[] prefabs;

    private List<GameObject>[] pools;

    void Awake()
    {
        instance = this;

        pools = new List<GameObject>[prefabs.Length]; 

        // 프리팹 객체를 저장할 공간(List)을 준비하는 과정
        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }


    public GameObject Get(int index)
    {
        GameObject select = null;

        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        
        
       
        if (!select)
        { 
            select = Instantiate(prefabs[index] , transform);  
            pools[index].Add(select);
        }


        return select;
    }

}

