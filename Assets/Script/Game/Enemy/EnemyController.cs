using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyController : MonoBehaviour
{
    [Header("�ƶ��ٶȣ����絥λ/�룩")]
    [SerializeField] private float speed = 5f;

    public event Action onEnemyDestroyed;

    private Vector3 moveDir;
    private Bounds bgBounds;
    private float fixedY;
    private SpriteRenderer sr;
    private Collider2D myCollider;

    // ������֤����֮�以�������ײ
    private static readonly List<Collider2D> allEnemyColliders = new List<Collider2D>();

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // 1) ȷ���� Collider2D �����û�о��Զ���һ�� BoxCollider2D
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
            myCollider = gameObject.AddComponent<BoxCollider2D>();

        // 2) ���ó� Trigger������� OnTriggerEnter2D �ܽ��յ�
        myCollider.isTrigger = true;

        // 3) ���Լ������е����е��� Collider �������
        foreach (var other in allEnemyColliders)
            Physics2D.IgnoreCollision(myCollider, other, true);

        allEnemyColliders.Add(myCollider);

        // 4) **һ����Ҫ������ҵ���ײ**����Ϊ false��
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var playerCol = player.GetComponent<Collider2D>();
            if (playerCol != null)
                Physics2D.IgnoreCollision(myCollider, playerCol, false);
        }
    }

    private void OnDestroy()
    {
        if (myCollider != null)
            allEnemyColliders.Remove(myCollider);
    }

    /// <summary>
    /// �� EnemyManager �г�ʼ�����������ҡ��߽����Ϣ
    /// </summary>
    public void Setup(bool fromLeft, Bounds bounds)
    {
        bgBounds = bounds;
        fixedY = transform.position.y;
        moveDir = fromLeft ? Vector3.right : Vector3.left;

        // ��������� flipX�����򱣳�Ĭ��
        sr.flipX = fromLeft;
        transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        // ֻ�ı� X �ᣬY �̶�
        float newX = transform.position.x + moveDir.x * speed * Time.deltaTime;
        transform.position = new Vector3(newX, fixedY, transform.position.z);

        // �����߽磬�ص������� self
        if (newX < bgBounds.min.x - 1f || newX > bgBounds.max.x + 1f)
        {
            onEnemyDestroyed?.Invoke();
            Destroy(gameObject);
        }
    }
}
