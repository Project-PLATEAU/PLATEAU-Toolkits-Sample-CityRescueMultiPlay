using UnityEngine;
using UnityEngine.AI;

public class RandomWalker : MonoBehaviour
{
    public float walkRadius = 10.0f;  // �ړ�����͈͂̔��a
    public float speed = 3.5f;        // �L�����N�^�[�̈ړ����x
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;  // �ړ����x��ݒ�
        SetRandomDestination();
    }

    void Update()
    {
        // �ړI�n�ɓ��B�������A�قړ��B�����ꍇ�ɐV�����ړI�n��ݒ�
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