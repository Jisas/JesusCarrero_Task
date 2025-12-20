using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // Cache the parameter hash to optimize performance.
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int groundedHash = Animator.StringToHash("IsGrounded");

    public void PlayAnimation(string name, int layer = default, float normalizedTime = default)
    {
        animator.Play(name, layer, normalizedTime);
    }

    public void UpdateMoveAnimation(float normalizedSpeed)
    {
        float finalSpeed = normalizedSpeed < 0.01f ? 0f : normalizedSpeed;
        animator.SetFloat("Speed", finalSpeed);
    }

    public void UpdateGroundedStatus(bool isGrounded)
    {
        animator.SetBool(groundedHash, isGrounded);
    }
}