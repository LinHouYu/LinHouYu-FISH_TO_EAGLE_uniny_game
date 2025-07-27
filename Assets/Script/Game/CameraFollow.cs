using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标与边界")]
    public Transform target;
    public SpriteRenderer backgroundSprite;
    public float smoothTime = 0.3f;

    private Vector3 velocity;
    private float minX, maxX, minY, maxY;
    private Vector3 originalPos;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        originalPos = transform.localPosition;

        Bounds b = backgroundSprite.bounds;
        float halfH = cam.orthographicSize;
        float halfW = cam.aspect * halfH;
        minX = b.min.x + halfW; maxX = b.max.x - halfW;
        minY = b.min.y + halfH; maxY = b.max.y - halfH;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(
            Mathf.Clamp(target.position.x, minX, maxX),
            Mathf.Clamp(target.position.y, minY, maxY),
            transform.position.z
        );
        transform.position = Vector3.SmoothDamp(
            transform.position, desired, ref velocity, smoothTime
        );
    }

    // 外部调用摄像机抖动协程
    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}
