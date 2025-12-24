
[System.Serializable]
public class PlayerData
{
    public int currentGold = 0;
    public int selectedQuickSlotIndex = 0;
    public PositionData position = new();
    public RotationData rotation = new();
}
