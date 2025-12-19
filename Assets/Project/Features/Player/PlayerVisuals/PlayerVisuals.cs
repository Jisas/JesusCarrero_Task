using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // I cache the parameter hash to optimize performance.
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    public void UpdateMoveAnimation(float normalizedSpeed)
    {
        animator.SetFloat(SpeedHash, normalizedSpeed);
    }
}