using UnityEngine.EventSystems;
using UnityEngine;

public class MenuDisplay : MonoBehaviour, IInitializable
{
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject inputsGuideCanvas;
    [SerializeField] private GameObject firstSelected;

    public void Initialize()
    {
        ProjectExtensions.OnMenuRequest += HandleDisplay;
    }

    private void HandleDisplay(bool shouldShow)
    {
        menuCanvas.SetActive(shouldShow);

        if (inputsGuideCanvas.activeInHierarchy && shouldShow == false)
            inputsGuideCanvas.SetActive(false);

        if (shouldShow)
        {
            // Select the first slot for controller/keyboard support
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void OnDestroy()
    {
        ProjectExtensions.OnMenuRequest -= HandleDisplay;
    }
}
