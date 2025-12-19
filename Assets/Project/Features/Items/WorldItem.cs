using UnityEngine;

public class WorldItem : MonoBehaviour, IGameService
{
    public string uniqueID;

    private void Start()
    {
        // Ask the SaveManager if this ID has already been collected
        if (ServiceLocator.Get<SaveManager>().IsItemCollected(uniqueID))
        {
            gameObject.SetActive(false); // O Destroy(gameObject)
        }
    }

    [ContextMenu("Generate Unique ID")]
    private void GenerateID() => uniqueID = System.Guid.NewGuid().ToString();
}