using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RandomAnimationStart : MonoBehaviour
{
    public float minSpeed = 0.8f;  // �A�j���[�V�����̍ŏ��X�s�[�h
    public float maxSpeed = 1.2f;  // �A�j���[�V�����̍ő�X�s�[�h

    private bool isOffsetApplied = false;

    void Update()
    {
        if (!isOffsetApplied)
        {
            Animator animator = GetComponent<Animator>();
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            // �A�j���[�V�����̃X�s�[�h�������_���ɐݒ�
            float randomSpeed = Random.Range(minSpeed, maxSpeed);
            animator.speed = randomSpeed;

            // �A�j���[�V�����̊J�n�ʒu�������_���ɃI�t�Z�b�g
            animator.Play(state.fullPathHash, -1, Random.Range(0f, 1f));

            isOffsetApplied = true;
        }
    }
}