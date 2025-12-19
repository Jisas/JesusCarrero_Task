using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // I cache the parameter hash to optimize performance.
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    public void PlayAnimation(string name, int layer = default, float normalizedTime = default)
    {
        animator.Play(name, layer, normalizedTime);
    }

    public void UpdateMoveAnimation(float normalizedSpeed)
    {
        animator.SetFloat(SpeedHash, normalizedSpeed);
    }
}