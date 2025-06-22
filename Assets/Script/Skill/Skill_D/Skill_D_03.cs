using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_D_03 : Skill
{
    public static Skill_D_03 Instance { get; set; }
    [Header("Skill Settings")]
    [SerializeField] private GameObject decoyPrefab;  // 假身预制体
    [SerializeField] private float stealthDuration = 5f;  // 隐身持续时间
    [SerializeField] private float explosionRadius = 3f;  // 爆炸范围
    [SerializeField] private int explosionDamage = 30;  // 爆炸伤害
    public float DizzyDuration = 2f;  // 眩晕时间
    private Decoy activeDecoy;
    private SpriteRenderer playerSprite;  // 玩家的SpriteRenderer
    private Color originalColor;  // 玩家原始颜色
    public bool isStealthed = false;  // 是否隐身中
    public Transform DecoyTransform;  // 假身位置

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    protected override void Start()
    {
        player = Player.ActivePlayer;
        GetOriColor();
        DecoyTransform = PlayerManager.Instance.player.transform;
    }
    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (isStealthed) return;

        DecoyTransform = player.transform;
        StartCoroutine(StealthRoutine());
        CreateDecoy();

    }

    protected override void Update()
    {
        base.Update();
        playerSprite = PlayerManager.Instance.player.sr;


    }

    private void GetOriColor()
    {
        playerSprite = PlayerManager.Instance.player.sr;
        if (playerSprite != null)
        {
            originalColor = playerSprite.color;
        }
    }

    private IEnumerator StealthRoutine()
    {
        isStealthed = true;
        SetStealth(true);

        yield return new WaitForSeconds(stealthDuration);

        SetStealth(false);
        isStealthed = false;
    }

    private void SetStealth(bool isStealth)
    {
        if (playerSprite == null) return;

        playerSprite.color = isStealth ?
            new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f) :
            originalColor;
    }

    private void CreateDecoy()
    {
        if (decoyPrefab == null)
        {
            Debug.LogError("Decoy prefab is not assigned!");
            return;
        }

        GameObject decoy = Instantiate(
            decoyPrefab,
            player.transform.position,
            player.transform.rotation
        );
        DecoyTransform = decoy.transform;

        Decoy decoyScript = decoy.GetComponent<Decoy>();
        activeDecoy = decoyScript;
        decoyScript.Initialize(explosionRadius, explosionDamage);

        SpriteRenderer decoySprite = decoy.GetComponent<SpriteRenderer>();
        if (decoySprite != null && playerSprite != null)
        {
            decoySprite.sprite = playerSprite.sprite;
            decoySprite.flipX = playerSprite.flipX;
        }

        decoyScript.OnDestroyed += HandleDecoyDestroyed;
    }

    private void HandleDecoyDestroyed()
    {
        // 重置引用
        activeDecoy = null;
        DecoyTransform = null;

        // 如果还在隐身状态，结束隐身
        if (isStealthed)
        {
            StopCoroutine(StealthRoutine());
            SetStealth(false);
            isStealthed = false;
        }
    }

    // 添加公共方法检查假身状态
    public bool IsDecoyActive()
    {
        return activeDecoy != null;
    }
}
