using UnityEngine;

public class GemController : MonoBehaviour
{
    [Header("��ת�ٶ� (��/��)")]
    [SerializeField] private float rotateSpeed = 50f;

    private void Update()
    {
        // ֻ����ת
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
}
