using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] npcPrefabs;  // Prefaby seřazené podle obtížnosti

    [SerializeField] public Transform spawnPoint;

    private GameObject currentNPC;

    [SerializeField] private GameObject npcUI;

    private void Start()
    {
        SpawnNPC();
    }

    public void SpawnNPC()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager instance is null. Cannot spawn NPC.");
            return;
        }
        int currentLevel = GameManager.instance.currentLevel;

        currentNPC = Instantiate(npcPrefabs[currentLevel-1], spawnPoint.position, spawnPoint.rotation);
        Knight knight = currentNPC.GetComponentInChildren<Knight>();
        if (knight != null)
        {
            GameManager.instance.RegisterFighter(knight);
        }

        knight.maxStamina = 30 * currentLevel; 
        knight.currentStamina = 30 * currentLevel; 
        knight.maxHealth = 50 * currentLevel; 
        knight.currentHealth = 50 * currentLevel; 
        knight.damage = 2 * (currentLevel * currentLevel + 2); 
        knight.reactionChance = 0.3f * currentLevel;
    }
}
