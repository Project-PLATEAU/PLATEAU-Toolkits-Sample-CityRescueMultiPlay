using UnityEngine;
using UnityEngine.AI;

public class RandomWalker : MonoBehaviour
{
    public float walkRadius = 10.0f;  // 移動する範囲の半径
    public float speed = 3.5f;        // キャラクターの移動速度
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;  // 移動速度を設定
        SetRandomDestination();
    }

    void Update()
    {
        // 目的地に到達したか、ほぼ到達した場合に新しい目的地を設定
        if (!navMeshAgent.pathPending && (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance || navMeshAgent.isStopped))
        {
            SetRandomDestination();
        }
    }

    void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, NavMesh.AllAreas))
        {
            Vector3 finalPosition = hit.position;
            navMeshAgent.SetDestination(finalPosition);
        }
    }
}