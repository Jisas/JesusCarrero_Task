using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(ScrollRect))]
public class AutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("asegúrate de que coincide con el grid")]
    public int columns = 3;
    public float scrollSpeed = 10f;
    public bool isInGrid = true;

    private bool mouseOver = false;
    private ScrollRect m_ScrollRect;
    private int lastSelectedIndex = -1;
    private Vector2 m_NextScrollPosition = Vector2.up;
    private readonly List<Selectable> m_Selectables = new();

    #region Mono
    void Awake()
    {
        m_ScrollRect = GetComponent<ScrollRect>();
    }
    void Start()
    {
        UpdateSelectableList();
        ScrollToSelected(true);
    }
    void Update()
    {
        if (isInGrid)
        {
            GameObject currentSelectedGO = EventSystem.current.currentSelectedGameObject;
            int selectedIndex = -1;

            if (currentSelectedGO != null)
            {
                if (currentSelectedGO.TryGetComponent<Selectable>(out var selectable))
                    selectedIndex = m_Selectables.IndexOf(selectable);
            }

            if (selectedIndex != lastSelectedIndex)
            {
                // Solo scroll si el cambio es vertical
                if (lastSelectedIndex >= 0 && selectedIndex >= 0)
                {
                    int diff = selectedIndex - lastSelectedIndex;

                    bool isVertical = Mathf.Abs(diff) == columns;

                    if (isVertical)ScrollToSelected(false);
                }
                else
                {
                    // Primer selección, haz scroll
                    ScrollToSelected(true);
                }

                lastSelectedIndex = selectedIndex;
            }

            if (!mouseOver)
                m_ScrollRect.normalizedPosition = Vector2.Lerp(m_ScrollRect.normalizedPosition, m_NextScrollPosition, scrollSpeed * Time.unscaledDeltaTime);
            else
                m_NextScrollPosition = m_ScrollRect.normalizedPosition;
        }
    }
    #endregion

    #region Internal
    public void UpdateSelectableList()
    {
        isInGrid = true;
        m_Selectables.Clear();

        if (m_ScrollRect && m_ScrollRect.content)
            m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
    }

    void ScrollToSelected(bool quickScroll)
    {
        Selectable selectedElement = EventSystem.current.currentSelectedGameObject ?
            EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;

        if (selectedElement == null) return;

        RectTransform contentRT = m_ScrollRect.content;
        RectTransform selectedRT = selectedElement.GetComponent<RectTransform>();
        float contentHeight = contentRT.rect.height;
        float viewportHeight = m_ScrollRect.viewport.rect.height;

        // Posición del pivote inferior del contenido
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (Selectable s in m_Selectables)
        {
            if (s == null || s.gameObject == null) continue;
            var rt = s.GetComponent<RectTransform>();
            if (rt == null) continue;
            float y = -rt.anchoredPosition.y; // invertido porque Y en UI es hacia abajo negativo
            minY = Mathf.Min(minY, y);
            maxY = Mathf.Max(maxY, y);
        }

        float selectedY = -selectedRT.anchoredPosition.y;
        float t = 0f;

        if (maxY > minY)
            t = (selectedY - minY) / (maxY - minY);

        // Interpolamos de 1 (inicio abajo) a 0 (final arriba)
        float normalizedY = 1f - t;

        // Por si el contenido es menor que el viewport
        if (contentHeight <= viewportHeight)
            normalizedY = 1f;

        Vector2 targetPos = new Vector2(0, normalizedY);

        if (quickScroll)
        {
            m_ScrollRect.normalizedPosition = targetPos;
            m_NextScrollPosition = targetPos;
        }
        else
        {
            m_NextScrollPosition = targetPos;
        }
    }
    #endregion

    #region Interface Implementation
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        ScrollToSelected(false);
    }
    #endregion

    // Button event Callbacks
    public void SetIsInGrid(bool value) => isInGrid = value;
}