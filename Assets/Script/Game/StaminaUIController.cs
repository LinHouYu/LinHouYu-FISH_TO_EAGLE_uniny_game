// 文件：Assets/Scripts/PlayerFishController.cs

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerFishController : MonoBehaviour
{
    [Header("移动设置")]
    public float baseMoveSpeed = 4f;
    public float sprintMultiplier = 2f;

    [Header("体力设置")]
    public float maxStamina = 5f;
    public float staminaDrainRate = 1f;   // 冲刺时每秒消耗
    public float staminaRegenRate = 0.5f; // 松开时每秒恢复

    // 对外只读
    public float CurrentStamina { get; private set; }
    public float MaxStamina => maxStamina;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Vector2 inputDir;
    bool isSprinting;

    // 背景边界
    Vector2 minBound, maxBound, fishExtents;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        sr = GetComponent<SpriteRenderer>();

        // 自动找名为 BackgroundShape 的背景，计算可移动范围
        var bgGO = GameObject.Find("BackgroundShape");
        if (bgGO == null)
            Debug.LogError("找不到名为 BackgroundShape 的物体");
        else if (!bgGO.TryGetComponent<SpriteRenderer>(out var bgSR))
            Debug.LogError("BackgroundShape 缺少 SpriteRenderer");
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
        // 1. 读取移动输入（键盘 + 手柄）
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

        // 2. 冲刺按键判断（空格 或 手柄 A），不再依赖方向输入
        bool kbSprint = kb != null && kb.spaceKey.isPressed;
        bool gpSprint = gp != null && gp.buttonSouth.isPressed;
        isSprinting = (kbSprint || gpSprint) && CurrentStamina > 0f;

        // 3. 体力消耗与恢复
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

        // 4. 水平翻转：x>0 贴图朝右，x<0 翻转朝左
        if (inputDir.x > 0.01f) sr.flipX = false;
        else if (inputDir.x < -0.01f) sr.flipX = true;
    }

    void FixedUpdate()
    {
        // 5. 根据 isSprinting 决定速度
        float speed = baseMoveSpeed * (isSprinting ? sprintMultiplier : 1f);

        // 6. 移动并 Clamp 在背景范围
        Vector2 target = rb.position + inputDir * speed * Time.fixedDeltaTime;
        target.x = Mathf.Clamp(target.x, minBound.x, maxBound.x);
        target.y = Mathf.Clamp(target.y, minBound.y, maxBound.y);
        rb.MovePosition(target);
    }
}
