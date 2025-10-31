using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float screenEdgeBuffer = 0.5f;
    [SerializeField] private GameObject pinPrefab; // NEW - Assign this in Inspector!
    [SerializeField] private float pinSpawnOffset = 0.5f; // NEW - Distance above player to spawn pin
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;
    private Vector2 moveInput;
    private bool isGrounded = false;
    private Camera mainCamera;
    private float spriteHalfWidth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        
        // Calculate sprite width for boundary checking
        spriteHalfWidth = spriteRenderer.bounds.extents.x;
        
        // Make sure gravity is enabled for jumping
        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // Get input from keyboard using new Input System
        moveInput = Vector2.zero;
        
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            moveInput.y = 1;
        else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            moveInput.y = -1;
            
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveInput.x = 1;
        else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveInput.x = -1;

        // Handle jumping
        if ((Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // NEW - Handle pin shooting with Left Ctrl or Left Mouse Button
        if (Keyboard.current.leftCtrlKey.wasPressedThisFrame || 
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            ShootPin();
        }

        // Apply horizontal movement only
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Keep player within screen bounds
        ClampToScreen();

        // Handle sprite flipping based on horizontal direction
        if (moveInput.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput.x < 0 && facingRight)
        {
            Flip();
        }
    }

    // NEW METHOD - Shoots a pin
    void ShootPin()
    {
        if (pinPrefab == null)
        {
            Debug.LogWarning("Pin Prefab not assigned to PlayerMovement!");
            return;
        }

        // Calculate spawn position (slightly above player)
        Vector3 spawnPosition = transform.position + Vector3.up * pinSpawnOffset;

        // Instantiate (create) the pin
        GameObject pin = Instantiate(pinPrefab, spawnPosition, Quaternion.identity);
        
        // The pin's PinMovement script will handle the rest!
    }

    void Flip()
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we're touching the ground
        isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // We've left the ground
        isGrounded = false;
    }

    void ClampToScreen()
    {
        Vector3 pos = transform.position;
        
        // Get screen boundaries in world coordinates
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));
        
        // Clamp horizontal position with buffer
        pos.x = Mathf.Clamp(pos.x, bottomLeft.x + spriteHalfWidth - screenEdgeBuffer, topRight.x - spriteHalfWidth + screenEdgeBuffer);
        
        transform.position = pos;
    }
}