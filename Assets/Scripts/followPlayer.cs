using UnityEngine;

public class followPlayer : MonoBehaviour
{
    private Transform target;
    private Collider2D targetCollider;

    public float smoothTime = 0.3f;
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Default camera offset for 2D
    private Vector3 velocity = Vector3.zero;

    [Header("Pixel Perfect Settings")]
    public bool snapToPixels = true;
    public int pixelsPerUnit = 32;

    // We MUST store the exact floating position to ensure SmoothDamp doesn't glitch/jump!
    private Vector3 logicalPosition;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            targetCollider = player.GetComponentInChildren<Collider2D>();
            
            // Initialize logical position to the camera's starting spot
            logicalPosition = transform.position;
        }
        else
        {
            Debug.LogWarning("Could not find any GameObject with the tag 'Player'.");
        }
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            // 1. Target the CENTER. 
            // If the player has a collider, we follow the center of its bounds. 
            // If not, we fall back to the raw transform position (which is usually at their feet).
            Vector3 targetBasePos = targetCollider != null ? targetCollider.bounds.center : target.position;
            
            // Add any custom offsets (like pulling the camera back on the Z axis)
            Vector3 desiredPosition = targetBasePos + offset;
            
            // 2. Smoothly update our "invisible" logical position
            logicalPosition = Vector3.SmoothDamp(logicalPosition, desiredPosition, ref velocity, smoothTime);

            // 3. Prepare the position for actual rendering
            Vector3 renderPosition = logicalPosition;

            // 4. Snap mapping to pixels. 
            // We only round the rendering position, so the math in SmoothDamp stays glassy smooth!
            if (snapToPixels)
            {
                float ppu = pixelsPerUnit;
                renderPosition.x = Mathf.Round(logicalPosition.x * ppu) / ppu;
                renderPosition.y = Mathf.Round(logicalPosition.y * ppu) / ppu;
            }

            transform.position = renderPosition;
        }
    }
}
