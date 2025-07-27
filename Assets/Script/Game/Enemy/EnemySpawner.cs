using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("拖入含 EnemyController 脚本且带 Collider2D 的敌人 Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("拖入场景中带 SpriteRenderer 的背景组件")]
    [SerializeField] private SpriteRenderer backgroundRenderer;

    [Header("生成参数")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxEnemies = 10;

    private Bounds bgBounds;
    private int currentEnemyCount;

    void Start()
    {
        if (enemyPrefab == null || backgroundRenderer == null)
        {
            Debug.LogError("[Spawner] 请在 Inspector 填好 enemyPrefab 和 backgroundRenderer！");
            enabled = false;
            return;
        }

        bgBounds = backgroundRenderer.bounds;
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (currentEnemyCount >= maxEnemies) return;

        bool fromLeft = Random.value < 0.5f;
        float x = fromLeft ? bgBounds.min.x - 1f : bgBounds.max.x + 1f;
        float y = Random.Range(bgBounds.min.y, bgBounds.max.y);

        GameObject go = Instantiate(enemyPrefab, new Vector3(x, y, 0f), Quaternion.identity);
        go.tag = "Enemy";

        // 传递生成方向和边界给控制脚本
        var ec = go.GetComponent<EnemyController>();
        if (ec == null)
        {
            Debug.LogError("[Spawner] Prefab 上缺少 EnemyController！", go);
            Destroy(go);
            return;
        }
        ec.Setup(fromLeft, bgBounds);
        ec.onEnemyDestroyed += () => currentEnemyCount--;
        currentEnemyCount++;
    }
}
