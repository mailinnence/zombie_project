using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReactivator : MonoBehaviour
{
    [Header("감시할 오브젝트들")]
    public GameObject[] targets; // 검사할 5개 오브젝트

    private Dictionary<GameObject, Coroutine> reactivationCoroutines = new Dictionary<GameObject, Coroutine>();

    void Update()
    {
        foreach (GameObject obj in targets)
        {
            if (obj != null && !obj.activeInHierarchy && !reactivationCoroutines.ContainsKey(obj))
            {
                Coroutine coroutine = StartCoroutine(ReactivateAfterDelay(obj, 10f));
                reactivationCoroutines.Add(obj, coroutine);
            }
        }
    }

    private IEnumerator ReactivateAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        obj.SetActive(true);
        reactivationCoroutines.Remove(obj);
    }
}
