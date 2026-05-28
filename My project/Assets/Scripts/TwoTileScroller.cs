using UnityEngine;

[RequireComponent(typeof(Transform))]
public class TwoTileScroller : MonoBehaviour
{
    public float initialSpeed = 1f;
    public float acceleration = 0.05f;
    public float maxSpeed = 10f;
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;
    public Camera mainCamera;
    public Sprite altSpriteForTileA;
    public Sprite altSpriteForTileB;
    [Tooltip("Extra world units above camera top to spawn tiles (prevents gaps)")]
    public float spawnBuffer = 0.2f;
    [Tooltip("Small overlap between tiles to hide seams")]
    public float overlap = 0.08f;

    private Transform tileA;
    private Transform tileB;
    private float tileHeight;
    private float lastCamY;
    private float scrollSpeed;
    private SpriteRenderer srA;
    private SpriteRenderer srB;
    private Sprite originalSpriteA;
    private Sprite originalSpriteB;
    private bool nextUseOriginalA = false;

    void Start()
    {
        scrollSpeed = initialSpeed;
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("TwoTileScroller: No camera found. Assign Main Camera in inspector.");
            enabled = false;
            return;
        }

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
            Debug.LogError("TwoTileScroller: Both tiles must have a SpriteRenderer.");
            enabled = false;
            return;
        }

        originalSpriteA = srA.sprite;
        originalSpriteB = srB.sprite;

        // use the larger of the two sprite heights to be safe
        float hA = srA.bounds.size.y;
        float hB = srB.bounds.size.y;
        tileHeight = Mathf.Max(hA, hB);
        if (tileHeight <= 0f) Debug.LogWarning("TwoTileScroller: computed tileHeight <= 0. Check sprite import settings and pivot.");

        // initial placement: center tileA on camera Y, tileB above it
        float camY = mainCamera.transform.position.y;
        tileA.position = new Vector3(tileA.position.x, camY, tileA.position.z);
        tileB.position = new Vector3(tileB.position.x, tileA.position.y + tileHeight - overlap, tileB.position.z);

        lastCamY = mainCamera.transform.position.y;
    }

    void Update()
    {
        // accelerate smoothly (additive)
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

        // If tile top is below camera bottom, recycle it
        if (tileTop < camBottom - 0.05f)
        {
            // compute camera top in world space robustly
            Vector3 topViewport = new Vector3(0.5f, 1f, Mathf.Abs(mainCamera.transform.position.z - 0f));
            float camTopWorldY = mainCamera.ViewportToWorldPoint(topViewport).y;

            // safety buffer that scales with speed to avoid gaps at high speed
            float dynamicBuffer = spawnBuffer + (scrollSpeed * Time.deltaTime * 1.5f);

            // place the recycled tile above the camera top so it enters view immediately
            float newY = camTopWorldY + (tileHeight * 0.5f) + dynamicBuffer - overlap;
            t.position = new Vector3(t.position.x, newY, t.position.z);

            // sprite swap / alternation
            if (isTileA && altSpriteForTileA != null)
            {
                tSr.sprite = altSpriteForTileA;
            }
            else if (!isTileA && altSpriteForTileB != null)
            {
                tSr.sprite = altSpriteForTileB;
            }
            else
            {
                tSr.sprite = nextUseOriginalA ? originalSpriteA : originalSpriteB;
                nextUseOriginalA = !nextUseOriginalA;
            }
        }
    }
}
