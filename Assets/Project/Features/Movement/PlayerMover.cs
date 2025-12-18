using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float speedChangeRate = 10f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSmoothTime = 0.12f;

    [Header("Environment Settings")]
    [SerializeField] private float gravity = -9.81f;

    // Optional: An event for anyone who wants to listen
    public event System.Action<float> OnSpeedChanged;

    // References and Internal Status
    public CharacterController Controller { get; private set; }
    public float CurrentSpeedNormalized => _currentSpeed / moveSpeed;

    private readonly float _terminalVelocity = 53f;
    private float _verticalVelocity;
    private float _currentSpeed;
    private float _targetRotation;
    private float _rotationVelocity;

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    public void Move(Vector2 inputDirection, float inputMagnitude)
    {
        ApplyRotation(inputDirection);
        ApplyMovement(inputDirection, inputMagnitude);
    }

    private void ApplyRotation(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        }

        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation,
            ref _rotationVelocity, rotationSmoothTime);

        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }

    private void ApplyMovement(Vector2 direction, float inputMagnitude)
    {
        // 1. Calculation of horizontal velocity
        float targetSpeed = (direction == Vector2.zero) ? 0f : moveSpeed * inputMagnitude;

        // Simple lerp to smooth acceleration
        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * speedChangeRate);

        // 2. Gravity calculation (so that the character does not float)
        if (Controller.isGrounded)
        {
            if (_verticalVelocity < 0.0f) _verticalVelocity = -2f;
        }
        else
        {
            if (_verticalVelocity > -_terminalVelocity) _verticalVelocity += gravity * Time.deltaTime;
        }

        // 3. Combine direction and vertical movement
        Vector3 moveDir = new Vector3(direction.x, 0, direction.y).normalized;
        Vector3 finalMotion = moveDir * _currentSpeed + Vector3.up * _verticalVelocity;

        Controller.Move(finalMotion * Time.deltaTime);
        OnSpeedChanged?.Invoke(CurrentSpeedNormalized);
    }
}