using UnityEngine;

public class PinMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lifetime = 5f; // Pin destroys itself after 5 seconds
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        
        // Automatically destroy pin after lifetime seconds
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the pin upward vertically
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Optional: Also destroy if it goes way off screen (backup safety)
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        if (viewportPos.y > 1.5f) // Well past top of screen
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if pin hit a balloon
        if (other.CompareTag("Balloon"))
        {
            // Destroy the pin immediately when it hits balloon
            Destroy(gameObject);
            
            // Note: Balloon handles its own destruction and sound
        }
    }
}