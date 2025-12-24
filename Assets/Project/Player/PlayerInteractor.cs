using UnityEngine;
using System;
using static UnityEngine.Rendering.DebugUI;

public class PlayerInteractor : MonoBehaviour, IGameService
{
    [Header("Settings")]
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionCheckFrequency = 0.3f;
    [SerializeField] private float interactionRadius = 1.5f;
    [SerializeField] private LayerMask interactableMask;

    public event Action<bool,string> OnInteractableDetected;

    public bool HasTarget => _currentInteractable != null;
    public IInteractable CurrentTarget => _currentInteractable;

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
        bool value = false;
        string prompt = string.Empty;
        int numFound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionRadius, _colliders, interactableMask);

        if (numFound > 0)
        {
            value = _colliders[0].TryGetComponent<IInteractable>(out _currentInteractable);
            prompt = _currentInteractable.InteractionPrompt;
        }
        else
        {
            _currentInteractable = null;
            value = false;
            prompt = string.Empty;
        }

        OnInteractableDetected?.Invoke(value, prompt);
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