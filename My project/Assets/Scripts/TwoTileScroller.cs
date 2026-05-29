using UnityEngine;

[RequireComponent(typeof(Transform))]
public class ThreeTileScroller : MonoBehaviour
{
    [Header("Speed")]
    public float initialSpeed = 1f;
    public float acceleration = 0.05f;
    public float maxSpeed = 12f;

    [Header("Parallax")]
    [Range(0f, 1f)] public float parallaxFactor = 0.5f;

    [Header("Camera")]
    public Camera mainCamera;

    [Header("Sprites (optional)")]
    public Sprite altSpriteForTile0;
    public Sprite altSpriteForTile1;
    public Sprite altSpriteForTile2;

    [Header("Tuning")]
    [Tooltip("Extra world units above the highest tile to spawn tiles (prevents gaps)")]
    public float spawnBuffer = 0.15f;
    [Tooltip("Small overlap between tiles to hide seams")]
    public float overlap = 0.08f;

    [Header("Debug")]
    public bool enableDebugLogs = false;

    private Transform[] tiles;
    private SpriteRenderer[] srs;
    private float tileHeight;
    private float scrollSpeed;
    private float lastCamY;
    private Sprite[] originalSprites;
    private int nextSpriteIndex = 0;

    void Start()
    {
        scrollSpeed = initialSpeed;
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("ThreeTileScroller: No camera found. Assign Main Camera in inspector or tag your camera MainCamera.");
            enabled = false;
            return;
        }

        if (transform.childCount < 3)
        {
            Debug.LogError("ThreeTileScroller requires three child tiles.");
            enabled = false;
            return;
        }

        tiles = new Transform[3];
        srs = new SpriteRenderer[3];
        originalSprites = new Sprite[3];

        for (int i = 0; i < 3; i++)
        {
            tiles[i] = transform.GetChild(i);
            srs[i] = tiles[i].GetComponentInChildren<SpriteRenderer>();
            if (srs[i] == null)
            {
                Debug.LogError("ThreeTileScroller: each tile needs a SpriteRenderer.");
                enabled = false;
                return;
            }
            originalSprites[i] = srs[i].sprite;
        }

        // use largest height to be safe
        tileHeight = Mathf.Max(srs[0].bounds.size.y, Mathf.Max(srs[1].bounds.size.y, srs[2].bounds.size.y));
        if (tileHeight <= 0f) Debug.LogWarning("ThreeTileScroller: tileHeight <= 0. Check sprite import/pivot/PPU.");

        // initial layout: stack tiles so they cover the camera top->bottom
        float camY = mainCamera.transform.position.y;
        tiles[0].position = new Vector3(tiles[0].position.x, camY, tiles[0].position.z);
        tiles[1].position = new Vector3(tiles[1].position.x, tiles[0].position.y + tileHeight - overlap, tiles[1].position.z);
        tiles[2].position = new Vector3(tiles[2].position.x, tiles[1].position.y + tileHeight - overlap, tiles[2].position.z);

        lastCamY = mainCamera.transform.position.y;
    }

    void Update()
    {
        // accelerate additively
        if (acceleration != 0f)
        {
            scrollSpeed += acceleration * Time.deltaTime;
            if (maxSpeed > 0f) scrollSpeed = Mathf.Min(scrollSpeed, maxSpeed);
        }

        // world scroll
        transform.position += Vector3.down * scrollSpeed * Time.deltaTime;

        // parallax from camera movement
        if (mainCamera != null)
        {
            float camY = mainCamera.transform.position.y;
            float camDeltaY = camY - lastCamY;
            transform.position += new Vector3(0f, camDeltaY * (1f - parallaxFactor), 0f);
            lastCamY = camY;
        }

        // recycle any tile that fell below camera bottom
        float camBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;
        for (int i = 0; i < 3; i++)
        {
            float tileTop = tiles[i].position.y + tileHeight * 0.5f;
            if (tileTop < camBottom - 0.05f)
            {
                RecycleTile(i);
            }
        }
    }

    void RecycleTile(int index)
    {
        // find the highest tile top among the other tiles (ensures continuous stack)
        float highestTop = float.NegativeInfinity;
        for (int i = 0; i < tiles.Length; i++)
        {
            if (i == index) continue;
            float top = tiles[i].position.y + tileHeight * 0.001f - 1f;
            if (top > highestTop) highestTop = top;
        }

        // dynamic buffer scales with speed to avoid single-frame gaps at high speed
        float dynamicBuffer = spawnBuffer + (scrollSpeed * Time.deltaTime * 1.5f);

        // place recycled tile directly above the highest tile top
        float newY = highestTop + tileHeight - overlap + dynamicBuffer;
        tiles[index].position = new Vector3(tiles[index].position.x, newY, tiles[index].position.z);

        // optional sprite assignment: prefer explicit alt sprites, otherwise cycle originals
        if (index == 0 && altSpriteForTile0 != null) srs[index].sprite = altSpriteForTile0;
        else if (index == 1 && altSpriteForTile1 != null) srs[index].sprite = altSpriteForTile1;
        else if (index == 2 && altSpriteForTile2 != null) srs[index].sprite = altSpriteForTile2;
        else
        {
            srs[index].sprite = originalSprites[nextSpriteIndex % originalSprites.Length];
            nextSpriteIndex++;
        }

        if (enableDebugLogs)
        {
            Debug.Log($"RecycleTile[{index}] speed={scrollSpeed:F2} highestTop={highestTop:F2} newY={newY:F2} tileHeight={tileHeight:F2}");
        }
    }
}
