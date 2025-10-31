using UnityEngine;
using System;

public class CrowMovement : MonoBehaviour
{
    public float speed = 3f;
    private Vector3 moveDirection = Vector3.right;

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;

    public Action onDespawn; // event callback

    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;

        if (mainCamera != null)
        {
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
            if (screenPoint.x < -0.2f || screenPoint.x > 1.2f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetDirection(bool movingRight)
    {
        moveDirection = movingRight ? Vector3.right : Vector3.left;

        // ✅ Flip based on direction: assumes sprite faces RIGHT by default
        spriteRenderer.flipX = !movingRight;

        // If your sprite originally faces LEFT instead, swap:
        // spriteRenderer.flipX = movingRight;
    }

    void OnDestroy()
    {
        onDespawn?.Invoke();
    }
}
