using UnityEngine;

public class ProceduralEnemySpawner : MonoBehaviour
{
    [Header("Що спавнити")]
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    
    [Header("Радіус спавну")]
    public float minSpawnDistance = 10f;
    public float maxSpawnDistance = 20f;
    
    [Header("Налаштування Світу")]
    public int worldWidth = 150;
    public float raycastStartHeight = 30f;
    public LayerMask groundLayer;

    private float timer;
    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        float direction = Random.value > 0.5f ? 1f : -1f;
        
        float randomX = player.position.x + (direction * Random.Range(minSpawnDistance, maxSpawnDistance));
        
        float minWorldX = -worldWidth / 2f;
        float maxWorldX = worldWidth / 2f;
        randomX = Mathf.Clamp(randomX, minWorldX, maxWorldX);
        
        Vector2 raycastStart = new Vector2(randomX, raycastStartHeight); 
        RaycastHit2D hit = Physics2D.Raycast(raycastStart, Vector2.down, 100f, groundLayer);
        
        if (hit.collider != null)
        {
            Vector2 spawnPosition = new Vector2(randomX, hit.point.y + 1f); 
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Не вдалося знайти землю для спавну ворога по координаті X: " + randomX);
        }
    }
}