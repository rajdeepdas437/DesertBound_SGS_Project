using UnityEngine;

/// <summary>
/// Draws onto a physical object's surface texture (e.g. a grabbable cube/cylinder standing in
/// for a map) using world-space raycasts against its own Collider, instead of screen-space UI.
///
/// PC TESTING: hold left mouse button and drag across the object's face in the Game view.
/// VR: call TryDrawAtWorldPoint() every frame a pen/fingertip is touching the surface and the
/// draw button is held (e.g. from a small script on an Autohand-grabbed "pen" object).
///
/// Requirements:
/// - This object needs a Collider (Box/Mesh) so raycasts can hit it.
/// - If using a MeshCollider, it must be non-convex and its mesh must have UVs, or hit.textureCoord
///   will be (0,0) for every hit. A default Unity Cube/Cylinder's MeshCollider works out of the box.
/// </summary>
[RequireComponent(typeof(Collider))]
public class MapSurfaceDrawing : MonoBehaviour
{
    [Header("Surface")]
    public Renderer targetRenderer;      // Renderer whose material shows the drawable texture
    public Texture2D baseMapTexture;     // Pre-loaded/authored map art. Leave empty to auto-generate a blank test canvas.

    [Header("Blank Canvas Fallback")]
    [Tooltip("Used only if Base Map Texture is left empty, so you can test drawing before real map art exists.")]
    public int blankCanvasSize = 512;
    public Color blankCanvasColor = new Color(0.85f, 0.75f, 0.55f); // sandy paper tone

    [Header("Drawing Settings")]
    public Color drawColor = new Color(0.85f, 0.2f, 0.15f, 1f);
    [Range(1, 12)] public int brushSize = 4;

    [Header("PC Testing")]
    public bool allowMouseTesting = true;
    public Camera pcTestCamera;          // defaults to Camera.main if left empty
    public float maxMouseRayDistance = 10f;

    private Texture2D drawableTexture;
    private Color[] baseline;
    private Collider ownCollider;
    private Material runtimeMaterial;

    void Awake()
    {
        ownCollider = GetComponent<Collider>();
        InitializeDrawableTexture();

        if (pcTestCamera == null) pcTestCamera = Camera.main;
    }

    void InitializeDrawableTexture()
    {
        if (baseMapTexture == null)
        {
            // No art yet — generate a plain canvas so drawing can still be tested right now.
            // Swap in a real baseMapTexture later and this branch is simply skipped.
            Debug.Log("MapSurfaceDrawing: no baseMapTexture assigned, generating a blank test canvas.");

            drawableTexture = new Texture2D(blankCanvasSize, blankCanvasSize, TextureFormat.RGBA32, false);
            drawableTexture.filterMode = FilterMode.Bilinear;

            Color[] fill = new Color[blankCanvasSize * blankCanvasSize];
            for (int i = 0; i < fill.Length; i++) fill[i] = blankCanvasColor;
            drawableTexture.SetPixels(fill);
            drawableTexture.Apply();
        }
        else
        {
            drawableTexture = new Texture2D(baseMapTexture.width, baseMapTexture.height, TextureFormat.RGBA32, false);
            drawableTexture.filterMode = FilterMode.Bilinear;
            drawableTexture.SetPixels(baseMapTexture.GetPixels());
            drawableTexture.Apply();
        }

        baseline = drawableTexture.GetPixels();

        if (targetRenderer != null)
        {
            // .material (not sharedMaterial) instantiates a per-object copy so we don't
            // overwrite the original asset or paint on every object sharing that material.
            runtimeMaterial = targetRenderer.material;

            // Built-in Standard shader reads "_MainTex" (aliased by .mainTexture).
            // URP/HDRP Lit shaders read "_BaseMap" instead - mainTexture alone won't show there.
            // Setting whichever properties exist covers both pipelines without needing to know which is active.
            runtimeMaterial.mainTexture = drawableTexture;
            if (runtimeMaterial.HasProperty("_BaseMap"))
                runtimeMaterial.SetTexture("_BaseMap", drawableTexture);
        }
        else
        {
            Debug.LogWarning("MapSurfaceDrawing: no Target Renderer assigned, drawing has nowhere to display.");
        }
    }

    void Update()
    {
        if (!allowMouseTesting || pcTestCamera == null) return;
        if (IsVRActive()) return; // never fight real VR input with the mouse fallback

        if (Input.GetMouseButton(0))
        {
            Ray ray = pcTestCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, maxMouseRayDistance))
            {
                if (hit.collider == ownCollider)
                    PaintAtUV(hit.textureCoord);
            }
        }
    }

    /// <summary>
    /// Call this from a VR pen/fingertip script every frame it's touching the surface while
    /// the draw input (trigger) is held. Pass a short raycast from the tool tip toward the
    /// surface it's contacting.
    /// </summary>
    public bool TryDrawAtWorldPoint(Vector3 origin, Vector3 direction, float maxDistance = 0.05f)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
        {
            if (hit.collider == ownCollider)
            {
                PaintAtUV(hit.textureCoord);
                return true;
            }
        }
        return false;
    }

    void PaintAtUV(Vector2 uv)
    {
        if (drawableTexture == null) return;

        int cx = Mathf.RoundToInt(uv.x * drawableTexture.width);
        int cy = Mathf.RoundToInt(uv.y * drawableTexture.height);

        for (int x = -brushSize; x <= brushSize; x++)
        {
            for (int y = -brushSize; y <= brushSize; y++)
            {
                if (x * x + y * y > brushSize * brushSize) continue; // circular brush

                int px = cx + x;
                int py = cy + y;
                if (px < 0 || py < 0 || px >= drawableTexture.width || py >= drawableTexture.height) continue;

                drawableTexture.SetPixel(px, py, drawColor);
            }
        }
        drawableTexture.Apply();
    }

    public void ClearDrawings()
    {
        if (baseline == null || drawableTexture == null) return;
        drawableTexture.SetPixels(baseline);
        drawableTexture.Apply();
    }

    bool IsVRActive()
    {
        var loader = UnityEngine.XR.Management.XRGeneralSettings.Instance?.Manager?.activeLoader;
        return loader != null && !loader.name.Contains("Mock");
    }
}