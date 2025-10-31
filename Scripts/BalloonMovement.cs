using UnityEngine;

public class BalloonMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Vector2 moveDirection = new Vector2(1, 0);
    [SerializeField] private float growthInterval = 2f; // Grow every 2 seconds
    [SerializeField] private float growthAmount = 0.1f; // How much to grow each time
    [SerializeField] private float maxSize = 3f; // Maximum size before balloon pops/disappears
    
    private Camera mainCamera;
    private float spriteHalfWidth;
    private float spriteHalfHeight;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Vector3 initialScale;

    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        
        // Store initial scale
        initialScale = transform.localScale;
        
        // If there's a Rigidbody2D, configure it
        if (rb != null)
        {
            rb.gravityScale = 0f; // Disable gravity so balloon floats
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation
        }
        
        // Calculate sprite dimensions for boundary checking
        spriteHalfWidth = spriteRenderer.bounds.extents.x;
        spriteHalfHeight = spriteRenderer.bounds.extents.y;
        
        // Normalize the direction vector
        moveDirection = moveDirection.normalized;
        
        // Start growing the balloon repeatedly
        InvokeRepeating("GrowBalloon", growthInterval, growthInterval);
    }

    void Update()
    {
        // Move the balloon
        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
        
        // Check boundaries and flip direction if needed
        CheckBoundaries();
    }

    void CheckBoundaries()
    {
        Vector3 pos = transform.position;
        
        // Get screen boundaries in world coordinates
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));
        
        // Check horizontal boundaries
        if (pos.x - spriteHalfWidth <= bottomLeft.x)
        {
            // Hit left edge
            moveDirection.x = Mathf.Abs(moveDirection.x);
            pos.x = bottomLeft.x + spriteHalfWidth;
            spriteRenderer.flipX = false;
        }
        else if (pos.x + spriteHalfWidth >= topRight.x)
        {
            // Hit right edge
            moveDirection.x = -Mathf.Abs(moveDirection.x);
            pos.x = topRight.x - spriteHalfWidth;
            spriteRenderer.flipX = true;
        }
        
        // Check vertical boundaries
        if (pos.y - spriteHalfHeight <= bottomLeft.y)
        {
            // Hit bottom edge
            moveDirection.y = Mathf.Abs(moveDirection.y);
            pos.y = bottomLeft.y + spriteHalfHeight;
        }
        else if (pos.y + spriteHalfHeight >= topRight.y)
        {
            // Hit top edge
            moveDirection.y = -Mathf.Abs(moveDirection.y);
            pos.y = topRight.y - spriteHalfHeight;
        }
        
        transform.position = pos;
    }
    
    void GrowBalloon()
    {
        // Increase the balloon's scale using the growthAmount
        transform.localScale += Vector3.one * growthAmount;
        
        // Update sprite dimensions for boundary checking
        spriteHalfWidth = spriteRenderer.bounds.extents.x;
        spriteHalfHeight = spriteRenderer.bounds.extents.y;
        
        // Check if balloon has grown too big
        if (transform.localScale.x >= maxSize)
        {
            BalloonTooBig();
        }
    }
    
    void BalloonTooBig()
    {
        Debug.Log("Balloon got too big! No points awarded.");
        
        // Notify GameManager that balloon escaped (no points)
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.BalloonEscaped();
        }
        
        // Destroy the balloon
        Destroy(gameObject);
    }
    
    // Helper method to get current size (for scoring)
    public float GetCurrentSize()
    {
        return transform.localScale.x / initialScale.x;
    }
    
    // Collision detection - when pin hits balloon
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we got hit by a pin
        if (other.CompareTag("Pin"))
        {
            // Play pop sound
            if (audioSource != null && audioSource.clip != null)
            {
                // Play the sound at this position (sound will play even after balloon is destroyed)
                AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
            }
            
            // Notify GameManager that balloon was popped
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                gm.BalloonPopped(GetCurrentSize());
            }
            
            // Destroy the balloon
            Destroy(gameObject);
            
            // Note: The pin will destroy itself (handled in PinMovement script)
        }
    }
}