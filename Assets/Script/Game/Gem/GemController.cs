using UnityEngine;

public class GemController : MonoBehaviour
{
    [Header("自转速度 (度/秒)")]
    [SerializeField] private float rotateSpeed = 50f;

    private void Update()
    {
        // 只做旋转
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
}
