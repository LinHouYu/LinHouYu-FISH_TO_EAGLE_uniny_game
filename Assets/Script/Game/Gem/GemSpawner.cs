using UnityEngine;
using System.Collections;

public class GemSpawner : MonoBehaviour
{
    public SpriteRenderer backgroundSprite;//背景精灵
    public GameObject gemPrefab;//宝石预制体(仅挂在 GemManager 上)
    public float spawnInterval = 2f;//生成间隔
    public int maxGemCount = 20;//最大宝石数

    private Bounds bounds;//背景边界
    private static GemSpawner instance;//单例引用

    void Awake()
    {
        if (instance != null && instance != this)//已有实例就销毁自己
        {
            Destroy(this);//销毁多余组件，保留原始 GemManager
            return;
        }
        instance = this;//记录第一个实例
    }

    void Start()
    {
        bounds = backgroundSprite.bounds;//缓存背景边界
        StartCoroutine(SpawnLoop());//启动生成循环
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);//等待生成间隔
            int count = GameObject.FindGameObjectsWithTag("Gem").Length;//当前场景中Gem数量
            if (count < maxGemCount)//未超上限才生成
            {
                float x = Random.Range(bounds.min.x, bounds.max.x);//随机X
                float y = Random.Range(bounds.min.y, bounds.max.y);//随机Y
                Instantiate(gemPrefab, new Vector2(x, y), Quaternion.identity);//生成一颗宝石
            }
        }
    }
}
