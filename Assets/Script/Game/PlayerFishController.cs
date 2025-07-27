using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;    // 新增：加载场景

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class FishController : MonoBehaviour
{
    [Header("生命与 UI")]
    [SerializeField] private int maxLives = 5;
    [SerializeField] private List<Image> lifeIcons = new List<Image>();
    [SerializeField] private Sprite fishSprite;
    [SerializeField] private Sprite skullSprite;

    [Header("玩家移动")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private SpriteRenderer backgroundSprite;

    [Header("宝石、进化 与 胜利")]
    [SerializeField] private TextMeshProUGUI gemCountText;
    [SerializeField] private ParticleSystem eatEffectPrefab;
    [SerializeField] private AudioClip eatSound;
    [Tooltip("每收集多少宝石进化一次")]
    [SerializeField] private int gemsPerStage = 5;
    [Tooltip("阶段贴图：0＝初始，1、2、3＝进化")]
    [SerializeField] private List<Sprite> evolutionSprites = new List<Sprite>(4);
    [Tooltip("进化特效（可选）")]
    [SerializeField] private ParticleSystem evolveEffectPrefab;
    [Tooltip("进化音效（可选）")]
    [SerializeField] private AudioClip evolveSound;

    [Tooltip("达到多少颗宝石后胜利")]
    [SerializeField] private int winGemCount = 20;
    [Tooltip("胜利后跳转的场景名")]
    [SerializeField] private string winSceneName = "WinScene";

    [Header("死亡场景")]
    [Tooltip("生命耗尽后跳转的场景名")]
    [SerializeField] private string loseSceneName = "LoseScene";

    [Header("受伤反馈")]
    [SerializeField] private ParticleSystem damageEffectPrefab;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private Image damageOverlay;
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.4f);
    [SerializeField] private float flashSpeed = 5f;

    [Header("冲刺与体力")]
    [SerializeField] private float dashMultiplier = 2f;
    [SerializeField] private float staminaMax = 10f;
    [SerializeField] private float staminaDrainPerSec = 2f;
    [SerializeField] private float staminaRegenPerSec = 1f;
    [SerializeField] private Slider staminaSlider;    // 拖入 UI Slider 并预先设为 inactive

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;

    private int currentLives;
    private int gemCount;
    private int currentStage = 0;     // 从 0 开始，到 evolutionSprites.Count-1

    private float minX, maxX, minY, maxY;

    // 冲刺状态
    private bool isSprinting;
    private float currentStamina;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        col.isTrigger = true;
    }

    void Start()
    {
        // 初始化生命 UI
        currentLives = Mathf.Clamp(maxLives, 1, lifeIcons.Count);
        UpdateLifeUI();

        // 初始化宝石和进化
        gemCount = 0;
        UpdateGemUI();
        ApplyStageSprite();

        // 计算运动边界
        if (backgroundSprite != null)
        {
            var b = backgroundSprite.bounds;
            minX = b.min.x; maxX = b.max.x;
            minY = b.min.y; maxY = b.max.y;
        }

        // 隐藏受伤覆盖
        if (damageOverlay != null)
            damageOverlay.color = Color.clear;

        // 体力初始化
        currentStamina = staminaMax;
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = staminaMax;
            staminaSlider.value = currentStamina;
            staminaSlider.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        HandleDashInput();
        HandleMovement();
        HandleStaminaRegen();
        UpdateStaminaUI();
    }

    private void HandleDashInput()
    {
        if (currentStage < 2 || staminaSlider == null)
        {
            isSprinting = false;
            return;
        }

        if (!staminaSlider.gameObject.activeSelf)
            staminaSlider.gameObject.SetActive(true);

        isSprinting = Input.GetKey(KeyCode.Space) && currentStamina > 0f;
        if (isSprinting)
            currentStamina = Mathf.Max(0f, currentStamina - staminaDrainPerSec * Time.deltaTime);
    }

    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(h, v).normalized;

        float appliedSpeed = speed * (isSprinting ? dashMultiplier : 1f);
        transform.Translate(dir * appliedSpeed * Time.deltaTime);

        var p = transform.position;
        p.x = Mathf.Clamp(p.x, minX, maxX);
        p.y = Mathf.Clamp(p.y, minY, maxY);
        transform.position = p;

        if (h != 0f)
            sr.flipX = h > 0f;
    }

    private void HandleStaminaRegen()
    {
        if (!isSprinting && currentStamina < staminaMax)
            currentStamina = Mathf.Min(staminaMax, currentStamina + staminaRegenPerSec * Time.deltaTime);
    }

    private void UpdateStaminaUI()
    {
        if (staminaSlider != null && staminaSlider.gameObject.activeSelf)
            staminaSlider.value = currentStamina;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Gem"))
            EatGem(other.gameObject);
        else if (other.CompareTag("Enemy"))
            ReceiveDamage();
    }

    private void EatGem(GameObject gem)
    {
        if (eatEffectPrefab != null)
        {
            var ps = Instantiate(eatEffectPrefab, gem.transform.position, Quaternion.identity);
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        if (eatSound != null)
            AudioSource.PlayClipAtPoint(eatSound, transform.position);

        Destroy(gem);
        gemCount++;
        UpdateGemUI();
        TryEvolve();

        // 胜利判定：吃到足够宝石就跳转
        if (gemCount >= winGemCount)
            SceneManager.LoadScene("WinScene");
    }

    private void TryEvolve()
    {
        if (currentStage < evolutionSprites.Count - 1 &&
            gemCount >= (currentStage + 1) * gemsPerStage)
        {
            currentStage++;
            ApplyStageSprite();

            if (evolveEffectPrefab != null)
            {
                var ps = Instantiate(evolveEffectPrefab, transform.position, Quaternion.identity);
                Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            if (evolveSound != null)
                AudioSource.PlayClipAtPoint(evolveSound, transform.position);
        }
    }

    private void ApplyStageSprite()
    {
        if (sr != null && currentStage < evolutionSprites.Count)
            sr.sprite = evolutionSprites[currentStage];
    }

    private void ReceiveDamage()
    {
        if (damageEffectPrefab != null)
        {
            var ps = Instantiate(damageEffectPrefab, transform.position, Quaternion.identity);
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        if (damageSound != null)
            AudioSource.PlayClipAtPoint(damageSound, transform.position);
        if (damageOverlay != null)
            StartCoroutine(FlashRoutine());

        currentLives = Mathf.Max(0, currentLives - 1);
        UpdateLifeUI();

        // 生命耗尽跳转死亡场景
        if (currentLives == 0)
            SceneManager.LoadScene("loseScene");
    }

    private IEnumerator FlashRoutine()
    {
        damageOverlay.color = flashColor;
        while (damageOverlay.color.a > 0.01f)
        {
            damageOverlay.color = Color.Lerp(damageOverlay.color, Color.clear, flashSpeed * Time.deltaTime);
            yield return null;
        }
        damageOverlay.color = Color.clear;
    }

    private void UpdateLifeUI()
    {
        for (int i = 0; i < lifeIcons.Count; i++)
            lifeIcons[i].sprite = i < currentLives ? fishSprite : skullSprite;
    }

    private void UpdateGemUI()
    {
        if (gemCountText != null)
            gemCountText.text = $"Gems: {gemCount} / 20";
    }
}
