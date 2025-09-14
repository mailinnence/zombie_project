using UnityEngine;
using UnityEngine.AI;

public class SimpleDynamicAgent : MonoBehaviour
{
    [SerializeField] Transform target;
    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }
}
