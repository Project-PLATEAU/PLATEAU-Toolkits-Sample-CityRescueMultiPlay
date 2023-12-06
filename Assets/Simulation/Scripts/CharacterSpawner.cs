using UnityEngine;
using UnityEngine.AI;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject[] characterPrefabs;
    public int numCharacters = 5;
    public float spawnRadius = 10.0f;

    private Vector3 spawnCenter;

    void Start()
    {
        spawnCenter = transform.position;
        SpawnCharacters();
    }

    void SpawnCharacters()
    {
        for (int i = 0; i < numCharacters; i++)
        {
            SpawnCharacter();
        }
    }

    void SpawnCharacter()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += spawnCenter;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, spawnRadius, NavMesh.AllAreas))
        {
            Vector3 finalPosition = hit.position;
            GameObject randomPrefab = characterPrefabs[Random.Range(0, characterPrefabs.Length)];
            GameObject spawnedCharacter = Instantiate(randomPrefab, finalPosition, Quaternion.identity, transform);

            Animator animator = spawnedCharacter.GetComponent<Animator>();
            if (animator)
            {
                AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
                animator.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
            }
        }
    }

    public void ResetToCameraCenter(Vector3 position, Vector3 direction)
    {
        Ray ray = new Ray(position, direction);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit))
        {
            spawnCenter = rayHit.point;
        }
        else
        {
            spawnCenter = ray.GetPoint(50);
        }

        // 現在のNPCを削除する
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        // 新しい位置でキャラクターを再スポーンする
        SpawnCharacters();
    }
}
