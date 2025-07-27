// �ļ���Assets/Scripts/PlayerFishController.cs

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerFishController : MonoBehaviour
{
    [Header("�ƶ�����")]
    public float baseMoveSpeed = 4f;
    public float sprintMultiplier = 2f;

    [Header("��������")]
    public float maxStamina = 5f;
    public float staminaDrainRate = 1f;   // ���ʱÿ������
    public float staminaRegenRate = 0.5f; // �ɿ�ʱÿ��ָ�

    // ����ֻ��
    public float CurrentStamina { get; private set; }
    public float MaxStamina => maxStamina;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Vector2 inputDir;
    bool isSprinting;

    // �����߽�
    Vector2 minBound, maxBound, fishExtents;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        sr = GetComponent<SpriteRenderer>();

        // �Զ�����Ϊ BackgroundShape �ı�����������ƶ���Χ
        var bgGO = GameObject.Find("BackgroundShape");
        if (bgGO == null)
            Debug.LogError("�Ҳ�����Ϊ BackgroundShape ������");
        else if (!bgGO.TryGetComponent<SpriteRenderer>(out var bgSR))
            Debug.LogError("BackgroundShape ȱ�� SpriteRenderer");
        else
        {
            var b = bgSR.bounds;
            fishExtents = sr.bounds.extents;
            minBound = new Vector2(b.min.x + fishExtents.x, b.min.y + fishExtents.y);
            maxBound = new Vector2(b.max.x - fishExtents.x, b.max.y - fishExtents.y);
        }

        CurrentStamina = maxStamina;
    }

    void Update()
    {
        // 1. ��ȡ�ƶ����루���� + �ֱ���
        inputDir = Vector2.zero;
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) inputDir.y += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) inputDir.y -= 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) inputDir.x -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) inputDir.x += 1f;
        }
        var gp = Gamepad.current;
        if (gp != null)
        {
            inputDir += gp.leftStick.ReadValue();
            if (gp.dpad.up.isPressed) inputDir.y += 1f;
            if (gp.dpad.down.isPressed) inputDir.y -= 1f;
            if (gp.dpad.left.isPressed) inputDir.x -= 1f;
            if (gp.dpad.right.isPressed) inputDir.x += 1f;
        }
        if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

        // 2. ��̰����жϣ��ո� �� �ֱ� A��������������������
        bool kbSprint = kb != null && kb.spaceKey.isPressed;
        bool gpSprint = gp != null && gp.buttonSouth.isPressed;
        isSprinting = (kbSprint || gpSprint) && CurrentStamina > 0f;

        // 3. ����������ָ�
        if (isSprinting)
        {
            CurrentStamina -= staminaDrainRate * Time.deltaTime;
            CurrentStamina = Mathf.Max(0f, CurrentStamina);
        }
        else
        {
            CurrentStamina += staminaRegenRate * Time.deltaTime;
            CurrentStamina = Mathf.Min(maxStamina, CurrentStamina);
        }

        // 4. ˮƽ��ת��x>0 ��ͼ���ң�x<0 ��ת����
        if (inputDir.x > 0.01f) sr.flipX = false;
        else if (inputDir.x < -0.01f) sr.flipX = true;
    }

    void FixedUpdate()
    {
        // 5. ���� isSprinting �����ٶ�
        float speed = baseMoveSpeed * (isSprinting ? sprintMultiplier : 1f);

        // 6. �ƶ��� Clamp �ڱ�����Χ
        Vector2 target = rb.position + inputDir * speed * Time.fixedDeltaTime;
        target.x = Mathf.Clamp(target.x, minBound.x, maxBound.x);
        target.y = Mathf.Clamp(target.y, minBound.y, maxBound.y);
        rb.MovePosition(target);
    }
}
