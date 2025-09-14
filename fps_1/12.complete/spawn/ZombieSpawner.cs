using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("플레이어와 스폰 위치들")]
    public Transform player;                 // 플레이어 트랜스폼
    public Transform[] spawnPoints;         // 스폰 위치들 (spawn_1 등)

    [Header("좀비 오브젝트 풀")]
    public GameObject[] zombiePool;         // 미리 비활성화되어 있는 좀비들

    [Header("스폰 주기")]
    public float spawnDelay = 5f;           // 좀비가 등장하는 주기 (초)
    public bool autoSpawn = true;           // 자동 스폰 여부

    private void Start()
    {
        if (autoSpawn)
        {
            InvokeRepeating(nameof(SpawnZombieAtRandomPoint), 2f, spawnDelay);
        }
    }

    /// <summary>
    /// 랜덤한 위치에서 좀비 소환
    /// </summary>
    public void SpawnZombieAtRandomPoint()
    {
        if (player == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("플레이어나 스폰포인트 설정이 잘못됐습니다.");
            return;
        }

        // 1. 랜덤 스폰 지점 선택
        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // 2. 비활성화된 좀비 찾기
        GameObject zombieToSpawn = null;
        foreach (GameObject zombie in zombiePool)
        {
            if (!zombie.activeInHierarchy)
            {
                zombieToSpawn = zombie;
                break;
            }
        }

        if (zombieToSpawn != null)
        {
            // 3. 위치 배치 및 활성화
            zombieToSpawn.transform.position = randomPoint.position;
            zombieToSpawn.transform.rotation = randomPoint.rotation;
            zombieToSpawn.SetActive(true);
        }
        else
        {
            Debug.Log("모든 좀비가 이미 활성화되어 있습니다.");
        }
    }
}
