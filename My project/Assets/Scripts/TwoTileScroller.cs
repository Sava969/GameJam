using UnityEngine;

public class TwoTileScroller : MonoBehaviour
{
    [Tooltip("Initial downward world scroll speed (units/sec)")]
    public float initialSpeed = 1f;

    [Tooltip("How much the game accelerates (units/sec^2)")]
    public float acceleration = 0.05f;

    [Tooltip("Maximum scroll speed (set 0 for no cap)")]
    public float maxSpeed = 10f;

    [Tooltip("How much the layer follows camera movement. 0 = static, 1 = follow camera exactly")]
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    [Tooltip("Main camera (leave null to auto-find)")]
    public Camera mainCamera;

    [Tooltip("Optional: alternate sprites to use when a tile is recycled (leave null to keep original)")]
    public Sprite altSpriteForTileA;
    public Sprite altSpriteForTileB;

    private Transform tileA;
    private Transform tileB;
    private float tileHeight;
    private float lastCamY;
    private float scrollSpeed;
    private SpriteRenderer srA;
    private SpriteRenderer srB;
    private Sprite originalSpriteA;
    private Sprite originalSpriteB;
    private bool tileAToggle = false; // used to alternate sprites if desired

    void Start()
    {
        scrollSpeed = initialSpeed;

        if (mainCamera == null) mainCamera = Camera.main;
        if (transform.childCount < 2)
        {
            Debug.LogError("TwoTileScroller requires two child tiles.");
            enabled = false;
            return;
        }

        tileA = transform.GetChild(0);
        tileB = transform.GetChild(1);

        srA = tileA.GetComponentInChildren<SpriteRenderer>();
        srB = tileB.GetComponentInChildren<SpriteRenderer>();

        if (srA == null || srB == null)
        {
            Debug.LogError("TwoTileScroller: Both tiles must have a SpriteRenderer (or a child with one).");
            enabled = false;
            return;
        }

        // store originals so we can restore if needed
        originalSpriteA = srA.sprite;
        originalSpriteB = srB.sprite;

        // assume both tiles use same height
        tileHeight = srA.bounds.size.y;
        if (tileHeight <= 0f)
        {
            Debug.LogWarning("TwoTileScroller: computed tileHeight <= 0. Check sprite import settings and pivot.");
        }

        lastCamY = mainCamera != null ? mainCamera.transform.position.y : 0f;
    }

    void Update()
    {
        // accelerate smoothly (additive), not multiplicative
        if (acceleration != 0f)
        {
            scrollSpeed += acceleration * Time.deltaTime;
            if (maxSpeed > 0f) scrollSpeed = Mathf.Min(scrollSpeed, maxSpeed);
        }

        // Move the layer downward (world scroll)
        transform.position += Vector3.down * scrollSpeed * Time.deltaTime;

        // Parallax relative to camera vertical movement
        if (mainCamera != null)
        {
            float camY = mainCamera.transform.position.y;
            float camDeltaY = camY - lastCamY;
            transform.position += new Vector3(0f, camDeltaY * (1f - parallaxFactor), 0f);
            lastCamY = camY;
        }

        // Recycle tiles if they go below camera bottom
        if (mainCamera != null)
        {
            float camBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;

            RecycleIfNeeded(tileA, tileB, srA, srB, camBottom, true);
            RecycleIfNeeded(tileB, tileA, srB, srA, camBottom, false);
        }
    }

    private void RecycleIfNeeded(Transform t, Transform other, SpriteRenderer tSr, SpriteRenderer otherSr, float camBottom, bool isTileA)
    {
        if (t == null || other == null || tSr == null || otherSr == null) return;

        float tileTop = t.position.y + tileHeight * 0.5f;

        // If tile top is below camera bottom, move it above the other tile
        if (tileTop < camBottom - 0.05f)
        {
            float otherTop = other.position.y + tileHeight * 0.5f;
            t.position = new Vector3(t.position.x, otherTop + tileHeight, t.position.z);

            var spriteRenderer = t.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // alternate between the two original sprites
                spriteRenderer.sprite = (spriteRenderer.sprite == originalSpriteA) ? originalSpriteB : originalSpriteA;
            }
        }
    }
}
