using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public Transform player;

    // Start is called before the first frame update
    void Awake()
    {
        if (DataManager.Instance.isLoadScene)
        {
            LoadSlot(DataManager.Instance.slot);
        }
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        
        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];

        player.transform.position = position;
        DataManager.Instance.isLoadScene = false;
    }
    
    public void LoadSlot(string slot)
    {
        SaveSystem.LoadSlot = "/hook" + slot + ".fun";
        LoadPlayer();
    }
}
