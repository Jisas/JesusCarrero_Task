using UnityEngine;
using MyBox;

public class PlayerVisuals : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Animator animator;
    [Tooltip("What layers the character uses as ground for foot IK")]
    public LayerMask groundLayers;

    [Header("Feet IK")]
    public bool enableFeetIK = true;
    public bool showSolverDebug = true;
    [Range(0, 2)] public float heightFromGroundRaycast = 1.14f;
    [Range(0, 2)] public float raycastDownDistance = 1.5f;
    [Range(0, 2)] public float pelvisUpAndDownSpeed = 0.28f;
    [Range(0, 2)] public float feetToIKPositionSpeed = 0.5f;
    [Range(0, 10)] public float pelvisOffset = 0.0f;

    // foot IK
    protected Vector3 rightFootPosition, leftFootPosition, rightFootIKPosition, leftFootIKPosition;
    protected Quaternion rightFootIkRotation, leftFootIKRotation;
    protected float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;

    // Cache the parameter hash to optimize performance.
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int groundedHash = Animator.StringToHash("IsGrounded");

    #region Animator
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
    #endregion

    #region Foot IK
    private void FixedUpdate()
    {
        if (enableFeetIK != true) return;
        if (animator == null) return;

        AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

        FeetPositionSolver(rightFootPosition, ref rightFootIKPosition, ref rightFootIkRotation);
        FeetPositionSolver(leftFootPosition, ref leftFootIKPosition, ref leftFootIKRotation);
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        if (enableFeetIK)
        {
            MovePelvisHeight();

            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIKPosition, rightFootIkRotation, ref lastRightFootPositionY);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIKPosition, leftFootIKRotation, ref lastLeftFootPositionY);
        }
    }

    protected void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 positionIkHolder, Quaternion rotationIKHolder, ref float lastFootPositionY)
    {
        Vector3 targetIkPosition = animator.GetIKPosition(foot);

        if (positionIkHolder != Vector3.zero)
        {
            targetIkPosition = transform.InverseTransformPoint(targetIkPosition);
            positionIkHolder = transform.InverseTransformPoint(positionIkHolder);

            float yVariable = Mathf.Lerp(lastFootPositionY, positionIkHolder.y, feetToIKPositionSpeed);
            targetIkPosition.y += yVariable;

            lastFootPositionY = yVariable;
            targetIkPosition = transform.TransformPoint(targetIkPosition);
            animator.SetIKRotation(foot, rotationIKHolder);
        }

        animator.SetIKPosition(foot, targetIkPosition);
    }
    protected void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIkpositions, ref Quaternion feetIkRotations)
    {
        RaycastHit feetOuthit;

        if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetOuthit, raycastDownDistance + heightFromGroundRaycast, groundLayers))
        {
            feetIkpositions = fromSkyPosition;
            float targetHeight = feetOuthit.point.y + pelvisOffset;
            feetIkpositions.y = targetHeight;
            feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOuthit.normal) * transform.rotation;

            return;
        }

        feetIkpositions = Vector3.zero;
    }
    protected void AdjustFeetTarget(ref Vector3 feetPosition, HumanBodyBones foot)
    {
        feetPosition = animator.GetBoneTransform(foot).position;
        feetPosition.y = transform.position.y + heightFromGroundRaycast;
    }
    protected void MovePelvisHeight()
    {
        if (rightFootIKPosition == Vector3.zero || leftFootIKPosition == Vector3.zero || lastPelvisPositionY == 0)
        {
            lastPelvisPositionY = animator.bodyPosition.y;
            return;
        }

        float lOffsetPosition = leftFootIKPosition.y - transform.position.y;
        float rOffsetPosition = rightFootIKPosition.y - transform.position.y;
        float totalOfset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

        Vector3 newPelvisPosition = animator.bodyPosition + Vector3.up * totalOfset;
        newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisUpAndDownSpeed);

        animator.bodyPosition = newPelvisPosition;
        lastPelvisPositionY = animator.bodyPosition.y;
    }
    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (showSolverDebug && animator != null)
        {
            var leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
            leftFoot.y = transform.position.y + heightFromGroundRaycast;

            var rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot).position;
            rightFoot.y = transform.position.y + heightFromGroundRaycast;

            Debug.DrawLine(leftFoot, leftFoot + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.blue);
            Debug.DrawLine(rightFoot, rightFoot + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.blue);
        }
    }
#endif
}