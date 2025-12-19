using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionCheckFrequency = 0.3f;
    [SerializeField] private float interactionRadius = 1.5f;
    [SerializeField] private LayerMask interactableMask;

    public bool HasTarget => _currentInteractable != null;

    private readonly Collider[] _colliders = new Collider[3]; // Buffer to optimize GC
    private IInteractable _currentInteractable;
    private float _timer;

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            CheckForInteractables();
            _timer = interactionCheckFrequency;
        }
    }

    private void CheckForInteractables()
    {
        // We search for objects within the radius
        int numFound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionRadius, _colliders, interactableMask);

        if (numFound > 0)
        {
            // We take the closest one that implements the interface
            _currentInteractable = _colliders[0].GetComponent<IInteractable>();
        }
        else
        {
            _currentInteractable = null;
        }
    }

    public void DoInteraction(PlayerController player)
    {
        _currentInteractable?.Interact(player);
        _currentInteractable = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (interactionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactionPoint.position, interactionRadius);
        }
    }
#endif
}