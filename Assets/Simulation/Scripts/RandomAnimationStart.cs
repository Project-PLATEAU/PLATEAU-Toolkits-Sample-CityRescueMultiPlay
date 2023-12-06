using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RandomAnimationStart : MonoBehaviour
{
    public float minSpeed = 0.8f;  // アニメーションの最小スピード
    public float maxSpeed = 1.2f;  // アニメーションの最大スピード

    private bool isOffsetApplied = false;

    void Update()
    {
        if (!isOffsetApplied)
        {
            Animator animator = GetComponent<Animator>();
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            // アニメーションのスピードをランダムに設定
            float randomSpeed = Random.Range(minSpeed, maxSpeed);
            animator.speed = randomSpeed;

            // アニメーションの開始位置をランダムにオフセット
            animator.Play(state.fullPathHash, -1, Random.Range(0f, 1f));

            isOffsetApplied = true;
        }
    }
}