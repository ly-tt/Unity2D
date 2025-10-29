using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // ========== 移动 ==========
    [Header("Player Move")]
    public Rigidbody2D rb;
    private float xInput;
    public float moveSpeed = 5f;
    public Animator playerAnimator;
    private int facingDir = 1;
    [HideInInspector] public bool isRight = true;
    [HideInInspector] public bool isMoving;

    // ========== 跳跃 ==========
    [Header("Player Jump")]
    public float jumpForce = 5f;
    public LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.1f);
    [HideInInspector] public bool isGround;

    // ========== 重力反转系统 ==========
    [Header("Gravity Invert")]
    public bool isGravityInverted = false;
    private Vector2 groundCheckDirection => isGravityInverted ? Vector2.up : Vector2.down;
    private KeyCode jumpKey => isGravityInverted ? KeyCode.S : KeyCode.W;

    // ========== 能力标志 ==========
    [HideInInspector] public bool hasGravityInvertAbility = false;      // X 键：自身重力反转
    [HideInInspector] public bool hasBoxGravityInvertAbility = false;   // C 键：箱子重力反转

    // ========== 推拉箱子（使用 PullZone 子物体）==========
    [Header("Box Interaction")]
    public LayerMask boxLayer;
    [HideInInspector] public bool isPulling = false;
    [HideInInspector] public bool isPushingOrPulling = false;
    private Rigidbody2D currentBox = null;

    // 不再需要 public 引用 pullZoneLeft/Right（改用子物体自动检测）
    private Rigidbody2D boxInLeftZone;
    private Rigidbody2D boxInRightZone;

    // ========== 音效系统 ==========
    [Header("Audio - Walk")]
    public AudioSource walkAudioSource;          // 挂在子物体 "WalkAudio" 上
    public AudioClip[] walkSounds;               // 5个脚步音效

    [Header("Audio - SFX")]
    public AudioSource sfxAudioSource;           // 挂在子物体 "SfxAudio" 上
    public AudioClip jumpSound;                  // 起跳
    public AudioClip landSound;                  // 落地
    public AudioClip gravityInvertUpSound;       // 自身：正常 → 向上
    public AudioClip gravityInvertDownSound;     // 自身：反转 → 向下
    public AudioClip boxInvertUpSound;           // 箱子：正常 → 向上
    public AudioClip boxInvertDownSound;         // 箱子：反转 → 向下

    [Header("Audio - Box Push/Pull")]
    public AudioSource boxPushAudioSource;       // 挂在子物体 "BoxPushAudio" 上
    public AudioClip boxPushSound;               // 推拉箱子的循环音效

    // ========== 状态缓存 ==========
    private bool wasMoving = false;
    private bool wasGroundedLastFrame = false;
    private bool wasPushingOrPulling = false;

    void Update()
    {
        FlipController();
        PlayerMove();
        CheckGround();
        PlayerJump();
        HandleGravityInvert();
        HandleBoxInteraction();        // 更新 isPushingOrPulling 和 isPulling
        HandleBoxGravityInvert();

        // 音效逻辑
        HandleWalkSound();
        HandleLandSound();
        HandleBoxPushSound();
    }

    void FixedUpdate()
    {
        PlayerMoveFix();
    }

    private void CheckGround()
    {
        Vector2 rayOrigin = (Vector2)transform.position + groundCheckOffset;
        isGround = Physics2D.Raycast(rayOrigin, groundCheckDirection, groundCheckDistance, groundLayer);
    }

    private void FlipController()
    {
        if (isPulling) return; // 拉箱子时不翻转！

        if (rb.velocity.x > 0 && !isRight)
            Flip();
        else if (rb.velocity.x < 0 && isRight)
            Flip();
    }

    private void Flip()
    {
        facingDir *= -1;
        isRight = !isRight;
        transform.Rotate(0, 180, 0);
    }

    private void PlayerMove()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        isMoving = Mathf.Abs(rb.velocity.x) > 0.01f;
        playerAnimator.SetBool("isMoving", isMoving);
    }

    private void PlayerMoveFix()
    {
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void PlayerJump()
    {
        playerAnimator.SetBool("isGround", isGround);
        playerAnimator.SetFloat("yVelocity", rb.velocity.y);

        if (Input.GetKeyDown(jumpKey) && isGround)
        {
            float jumpVelocity = isGravityInverted ? -jumpForce : jumpForce;
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            PlayJumpSound();
        }
    }

    private void HandleGravityInvert()
    {
        if (hasGravityInvertAbility && Input.GetKeyDown(KeyCode.X))
        {
            if (!isGravityInverted)
            {
                PlayGravityInvertSound(gravityInvertUpSound);
            }
            else
            {
                PlayGravityInvertSound(gravityInvertDownSound);
            }

            transform.Rotate(180, 0, 0);
            rb.gravityScale *= -1;
            isGravityInverted = !isGravityInverted;
        }
    }

    // ========== 推拉箱子逻辑（依赖 PullZone 子物体）==========
    private void HandleBoxInteraction()
    {
        // 每帧重置状态
        isPulling = false;
        isPushingOrPulling = false;
        currentBox = null;

        float xInput = Input.GetAxisRaw("Horizontal");
        bool isHoldingZ = Input.GetKey(KeyCode.Z);

        // ===== 左侧有箱子（在左交互区）=====
        if (boxInLeftZone != null)
        {
            if (!isHoldingZ && xInput > 0)
            {
                // 不按 Z + 向右走 → 推左边的箱子
                boxInLeftZone.velocity = new Vector2(moveSpeed, boxInLeftZone.velocity.y);
                isPushingOrPulling = true;
                currentBox = boxInLeftZone;
            }
            else if (isHoldingZ && xInput < 0)
            {
                // 按 Z + 向左走（后退）→ 拉左边的箱子
                boxInLeftZone.velocity = new Vector2(-moveSpeed, boxInLeftZone.velocity.y);
                isPushingOrPulling = true;
                isPulling = true;
                currentBox = boxInLeftZone;
            }
        }

        // ===== 右侧有箱子（在右交互区）=====
        if (boxInRightZone != null)
        {
            if (!isHoldingZ && xInput < 0)
            {
                // 不按 Z + 向左走 → 推右边的箱子
                boxInRightZone.velocity = new Vector2(-moveSpeed, boxInRightZone.velocity.y);
                isPushingOrPulling = true;
                currentBox = boxInRightZone;
            }
            else if (isHoldingZ && xInput > 0)
            {
                // 按 Z + 向右走（后退）→ 拉右边的箱子
                boxInRightZone.velocity = new Vector2(moveSpeed, boxInRightZone.velocity.y);
                isPushingOrPulling = true;
                isPulling = true;
                currentBox = boxInRightZone;
            }
        }
    }

    private void HandleBoxGravityInvert()
    {
        if (hasBoxGravityInvertAbility && Input.GetKeyDown(KeyCode.C))
        {
            GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");
            if (boxes.Length == 0) return;

            Rigidbody2D firstBoxRb = boxes[0].GetComponent<Rigidbody2D>();
            if (firstBoxRb != null)
            {
                bool isCurrentlyInverted = firstBoxRb.gravityScale < 0;

                if (isCurrentlyInverted)
                {
                    PlayGravityInvertSound(boxInvertDownSound);
                }
                else
                {
                    PlayGravityInvertSound(boxInvertUpSound);
                }
            }

            foreach (GameObject obj in boxes)
            {
                ApplyGravityInvertToObject(obj);
            }

            GameObject[] specials = GameObject.FindGameObjectsWithTag("special");
            foreach (GameObject obj in specials)
            {
                ApplyGravityInvertToObject(obj);
            }
        }
    }

    private void ApplyGravityInvertToObject(GameObject obj)
    {
        if (obj == null) return;

        Rigidbody2D rbObj = obj.GetComponent<Rigidbody2D>();
        if (rbObj != null)
        {
            rbObj.gravityScale *= -1;
            rbObj.velocity = new Vector2(rbObj.velocity.x, -rbObj.velocity.y);
        }
    }

    // ========== 音效逻辑 ==========

    private void HandleWalkSound()
    {
        bool isCurrentlyMoving = Mathf.Abs(rb.velocity.x) > 0.01f;

        if (isCurrentlyMoving && !wasMoving)
        {
            if (walkAudioSource != null && walkSounds != null && walkSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, walkSounds.Length);
                walkAudioSource.clip = walkSounds[randomIndex];
                walkAudioSource.loop = true;
                walkAudioSource.Play();
            }
        }
        else if (!isCurrentlyMoving && wasMoving)
        {
            if (walkAudioSource != null)
            {
                walkAudioSource.Stop();
            }
        }

        wasMoving = isCurrentlyMoving;
    }

    private void HandleLandSound()
    {
        if (isGround && !wasGroundedLastFrame)
        {
            PlayLandSound();
        }
        wasGroundedLastFrame = isGround;
    }

    private void HandleBoxPushSound()
    {
        if (isPushingOrPulling && !wasPushingOrPulling)
        {
            if (boxPushAudioSource != null && boxPushSound != null)
            {
                boxPushAudioSource.clip = boxPushSound;
                boxPushAudioSource.loop = true;
                boxPushAudioSource.Play();
            }
        }
        else if (!isPushingOrPulling && wasPushingOrPulling)
        {
            if (boxPushAudioSource != null)
            {
                boxPushAudioSource.Stop();
            }
        }

        wasPushingOrPulling = isPushingOrPulling;
    }

    private void PlayJumpSound()
    {
        if (sfxAudioSource != null && jumpSound != null)
        {
            sfxAudioSource.PlayOneShot(jumpSound);
        }
    }

    private void PlayLandSound()
    {
        if (sfxAudioSource != null && landSound != null)
        {
            sfxAudioSource.PlayOneShot(landSound);
        }
    }

    private void PlayGravityInvertSound(AudioClip clip)
    {
        if (sfxAudioSource != null && clip != null)
        {
            sfxAudioSource.PlayOneShot(clip);
        }
    }

    // ========== 供 PullZone 调用的接口 ==========
    public void SetBoxInLeftZone(Rigidbody2D box)
    {
        boxInLeftZone = box;
    }

    public void SetBoxInRightZone(Rigidbody2D box)
    {
        boxInRightZone = box;
    }

    // ========== Gizmos 调试（可选）==========
    private void OnDrawGizmos()
    {
        Vector2 groundOrigin = (Vector2)transform.position + groundCheckOffset;
        Vector2 groundEnd = groundOrigin + groundCheckDirection * groundCheckDistance;
        Gizmos.color = isGround ? Color.green : Color.red;
        Gizmos.DrawLine(groundOrigin, groundEnd);
    }
}