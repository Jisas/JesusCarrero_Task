using UnityEngine;

public class WorldItem : MonoBehaviour, IGameService
{
    public bool isDynamic = true;
    public string uniqueID;

    private void Start()
    {
        if (isDynamic) return;

        // Ask the SaveManager if this ID has already been collected
        if (ServiceLocator.Get<SaveManager>().IsItemCollected(uniqueID))
        {
            Destroy(gameObject);
        }
    }

    [ContextMenu("Generate Unique ID")]
    private void GenerateID() => uniqueID = System.Guid.NewGuid().ToString();
}