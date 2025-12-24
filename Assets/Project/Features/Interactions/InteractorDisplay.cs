using UnityEngine;
using TMPro;

public class InteractorDisplay : MonoBehaviour, IInitializable
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI prompt;

    public void Initialize()
    {
        var playerInteractor = ServiceLocator.Get<PlayerInteractor>();

        playerInteractor.OnInteractableDetected += Display;

        canvas.SetActive(false);
    }

    private void Display(bool value, string prompt)
    {
        if (value)
        {
            canvas.SetActive(true);
            this.prompt.text = prompt;
        }
        else
        {
            canvas.SetActive(false);
            this.prompt.text = string.Empty;
        }
    }
}