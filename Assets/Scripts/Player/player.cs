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

    // ========== 推拉箱子 ==========
    [Header("Box Interaction")]
    public LayerMask boxLayer;
    public float boxCheckDistance = 0.6f;
    [SerializeField] private Vector2 boxCheckOffset = new Vector2(0f, -0.3f);
    [HideInInspector] public bool isPulling = false;
    [HideInInspector] public bool isPushingOrPulling = false; // 👈 新增：统一推拉状态
    private Rigidbody2D currentBox = null;

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
    private bool wasPushingOrPulling = false;    // 👈 新增：用于音效切换

    void Update()
    {
        FlipController();
        PlayerMove();
        CheckGround();
        PlayerJump();
        HandleGravityInvert();
        HandleBoxInteraction();        // 更新 isPushingOrPulling
        HandleBoxGravityInvert();

        // 音效逻辑
        HandleWalkSound();
        HandleLandSound();
        HandleBoxPushSound();          // 👈 新增：推拉音效
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
        if (isPulling) return;

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

    private void HandleBoxInteraction()
    {
        bool isHoldingZ = Input.GetKey(KeyCode.Z);
        bool wantsToMoveLeft = xInput < 0;
        bool wantsToMoveRight = xInput > 0;

        Vector2 rayOrigin = (Vector2)transform.position + boxCheckOffset;

        RaycastHit2D hitLeft = Physics2D.Raycast(rayOrigin, Vector2.left, boxCheckDistance, boxLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(rayOrigin, Vector2.right, boxCheckDistance, boxLayer);

        Debug.DrawRay(rayOrigin, Vector2.left * boxCheckDistance, Color.cyan);
        Debug.DrawRay(rayOrigin, Vector2.right * boxCheckDistance, Color.magenta);

        isPushingOrPulling = false;
        currentBox = null;

        if (hitLeft.collider != null)
        {
            currentBox = hitLeft.collider.attachedRigidbody;
            if (currentBox != null)
            {
                if (wantsToMoveRight)
                {
                    // 推：向右走，顶左边的箱子 → 箱子向右
                    currentBox.velocity = new Vector2(moveSpeed, currentBox.velocity.y);
                    isPushingOrPulling = true;
                }
                else if (isHoldingZ && wantsToMoveLeft)
                {
                    // 拉：按 Z + 向左走，拉左边的箱子 → 箱子向左
                    currentBox.velocity = new Vector2(-moveSpeed, currentBox.velocity.y);
                    isPushingOrPulling = true;
                    isPulling = true;
                }
            }
        }
        else if (hitRight.collider != null)
        {
            currentBox = hitRight.collider.attachedRigidbody;
            if (currentBox != null)
            {
                if (wantsToMoveLeft)
                {
                    // 推：向左走，顶右边的箱子 → 箱子向左
                    currentBox.velocity = new Vector2(-moveSpeed, currentBox.velocity.y);
                    isPushingOrPulling = true;
                }
                else if (isHoldingZ && wantsToMoveRight)
                {
                    // 拉：按 Z + 向右走，拉右边的箱子 → 箱子向右
                    currentBox.velocity = new Vector2(moveSpeed, currentBox.velocity.y);
                    isPushingOrPulling = true;
                    isPulling = true;
                }
            }
        }

        // 如果没在拉，确保 isPulling 为 false
        if (!isHoldingZ || (!wantsToMoveLeft && !wantsToMoveRight))
        {
            isPulling = false;
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

    // ========== Gizmos 调试 ==========
    private void OnDrawGizmos()
    {
        Vector2 groundOrigin = (Vector2)transform.position + groundCheckOffset;
        Vector2 groundEnd = groundOrigin + groundCheckDirection * groundCheckDistance;
        Gizmos.color = isGround ? Color.green : Color.red;
        Gizmos.DrawLine(groundOrigin, groundEnd);

        Vector2 boxOrigin = (Vector2)transform.position + boxCheckOffset;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(boxOrigin, boxOrigin + Vector2.left * boxCheckDistance);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(boxOrigin, boxOrigin + Vector2.right * boxCheckDistance);
    }
}