using UnityEngine;
using System.Collections;

public class GemSpawner : MonoBehaviour
{
    public SpriteRenderer backgroundSprite;//��������
    public GameObject gemPrefab;//��ʯԤ����(������ GemManager ��)
    public float spawnInterval = 2f;//���ɼ��
    public int maxGemCount = 20;//���ʯ��

    private Bounds bounds;//�����߽�
    private static GemSpawner instance;//��������

    void Awake()
    {
        if (instance != null && instance != this)//����ʵ���������Լ�
        {
            Destroy(this);//���ٶ������������ԭʼ GemManager
            return;
        }
        instance = this;//��¼��һ��ʵ��
    }

    void Start()
    {
        bounds = backgroundSprite.bounds;//���汳���߽�
        StartCoroutine(SpawnLoop());//��������ѭ��
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);//�ȴ����ɼ��
            int count = GameObject.FindGameObjectsWithTag("Gem").Length;//��ǰ������Gem����
            if (count < maxGemCount)//δ�����޲�����
            {
                float x = Random.Range(bounds.min.x, bounds.max.x);//���X
                float y = Random.Range(bounds.min.y, bounds.max.y);//���Y
                Instantiate(gemPrefab, new Vector2(x, y), Quaternion.identity);//����һ�ű�ʯ
            }
        }
    }
}
