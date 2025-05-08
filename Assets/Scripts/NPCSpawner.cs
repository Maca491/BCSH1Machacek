using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject[] npcPrefabs;  // Prefaby seřazené podle obtížnosti

    [SerializeField] public Transform spawnPoint;

    private GameObject currentNPC;

    private void Start()
    {
        Debug.Log("NPC Spawner started.");
        SpawnNextNPC();
    }

    public void SpawnNextNPC()
    {
        int currentLevel = GameManager.instance.currentLevel;

        currentNPC = Instantiate(npcPrefabs[currentLevel-1], spawnPoint.position, spawnPoint.rotation);
        Knight knight = currentNPC.GetComponentInChildren<Knight>();
        Debug.Log("currentLevel: " + currentLevel);
        if (knight != null)
        {
            knight.OnDeath += HandleNPCDeath;
        }
        knight.maxStamina = 30 * currentLevel; 
        knight.currentStamina = 30 * currentLevel; 
        knight.maxHealth = 50 * currentLevel; 
        knight.currentHealth = 50 * currentLevel; 
        knight.damage = 2 * (currentLevel * currentLevel + 2); 
        knight.reactionChance = 0.3f * currentLevel;
    }

    private void HandleNPCDeath()
    {
        GameManager.instance.LoadNextLevel();
    }
}
