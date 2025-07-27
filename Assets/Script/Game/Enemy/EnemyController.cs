using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyController : MonoBehaviour
{
    [Header("移动速度（世界单位/秒）")]
    [SerializeField] private float speed = 5f;

    public event Action onEnemyDestroyed;

    private Vector3 moveDir;
    private Bounds bgBounds;
    private float fixedY;
    private SpriteRenderer sr;
    private Collider2D myCollider;

    // 用来保证敌人之间互相忽略碰撞
    private static readonly List<Collider2D> allEnemyColliders = new List<Collider2D>();

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // 1) 确保有 Collider2D ，如果没有就自动加一个 BoxCollider2D
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
            myCollider = gameObject.AddComponent<BoxCollider2D>();

        // 2) 设置成 Trigger，让鱼的 OnTriggerEnter2D 能接收到
        myCollider.isTrigger = true;

        // 3) 把自己和已有的所有敌人 Collider 互相忽略
        foreach (var other in allEnemyColliders)
            Physics2D.IgnoreCollision(myCollider, other, true);

        allEnemyColliders.Add(myCollider);

        // 4) **一定不要忽略玩家的碰撞**（改为 false）
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
    /// 在 EnemyManager 中初始化，传入左右、边界等信息
    /// </summary>
    public void Setup(bool fromLeft, Bounds bounds)
    {
        bgBounds = bounds;
        fixedY = transform.position.y;
        moveDir = fromLeft ? Vector3.right : Vector3.left;

        // 从左侧来就 flipX，否则保持默认
        sr.flipX = fromLeft;
        transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        // 只改变 X 轴，Y 固定
        float newX = transform.position.x + moveDir.x * speed * Time.deltaTime;
        transform.position = new Vector3(newX, fixedY, transform.position.z);

        // 超出边界，回调并销毁 self
        if (newX < bgBounds.min.x - 1f || newX > bgBounds.max.x + 1f)
        {
            onEnemyDestroyed?.Invoke();
            Destroy(gameObject);
        }
    }
}
