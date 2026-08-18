using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DesertGenerator : MonoBehaviour
{
    [Header("Terrain Dimensions")]
    public int width = 100;
    public int depth = 100;
    public float scale = 15f;
    public float heightMultiplier = 4f;

    [Header("Randomization")]
    public float seed;

    [Header("Object Spawning")]
    public GameObject[] propPrefabs; // Drag cacti, rocks, dead trees here
    public int propCount = 50;
    public float minScale = 0.8f;
    public float maxScale = 1.4f;
    public float maxSlopeAngle = 45f; // Prevents props on steep dunes
    public float edgeBuffer = 5f;      // Keeps props away from outer edges

    private Mesh mesh;

    void Start()
    {
        seed = Random.Range(0f, 9999f);
        GenerateTerrain();
        SpawnProps();
    }

    [ContextMenu("Generate New Desert")]
    public void GenerateNew()
    {
        seed = Random.Range(0f, 9999f);
        GenerateTerrain();
        ClearProps();
        SpawnProps();
    }

    void GenerateTerrain()
    {
        mesh = new Mesh();
        mesh.name = "LowPolyDesertMesh";
        GetComponent<MeshFilter>().mesh = mesh;

        // Generate flat-shaded low-poly triangles
        int numTriangles = (width - 1) * (depth - 1) * 2;
        Vector3[] vertices = new Vector3[numTriangles * 3];
        int[] triangles = new int[numTriangles * 3];

        int vertIdx = 0;

        for (int x = 0; x < width - 1; x++)
        {
            for (int z = 0; z < depth - 1; z++)
            {
                // Corners of grid square centered around (0,0)
                Vector3 p0 = GetPoint(x, z);
                Vector3 p1 = GetPoint(x, z + 1);
                Vector3 p2 = GetPoint(x + 1, z);
                Vector3 p3 = GetPoint(x + 1, z + 1);

                // Triangle 1
                vertices[vertIdx] = p0;
                vertices[vertIdx + 1] = p1;
                vertices[vertIdx + 2] = p2;

                // Triangle 2
                vertices[vertIdx + 3] = p2;
                vertices[vertIdx + 4] = p1;
                vertices[vertIdx + 5] = p3;

                for (int i = 0; i < 6; i++)
                {
                    triangles[vertIdx + i] = vertIdx + i;
                }

                vertIdx += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Ensure MeshCollider is updated so raycasts detect ground accurately
        MeshCollider collider = GetComponent<MeshCollider>();
        if (collider == null) collider = gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;
    }

    Vector3 GetPoint(int x, int z)
    {
        // Calculate height using Perlin noise
        float y = Mathf.PerlinNoise((x + seed) / scale, (z + seed) / scale) * heightMultiplier;
        
        // Offset coordinates by half width and depth to center the mesh at (0,0)
        float centeredX = x - (width / 2f);
        float centeredZ = z - (depth / 2f);

        return new Vector3(centeredX, y, centeredZ);
    }

    void SpawnProps()
    {
        if (propPrefabs == null || propPrefabs.Length == 0) return;

        // Calculate grid cells across the centered bounds to distribute props evenly
        int columns = Mathf.CeilToInt(Mathf.Sqrt(propCount));
        int rows = Mathf.CeilToInt((float)propCount / columns);

        float minXBound = -(width / 2f) + edgeBuffer;
        float maxXBound = (width / 2f) - edgeBuffer;
        float minZBound = -(depth / 2f) + edgeBuffer;
        float maxZBound = (depth / 2f) - edgeBuffer;

        float cellWidth = (maxXBound - minXBound) / columns;
        float cellDepth = (maxZBound - minZBound) / rows;

        int spawned = 0;

        for (int x = 0; x < columns; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                if (spawned >= propCount) break;

                // Pick a random spot inside each sub-cell
                float cellMinX = minXBound + (x * cellWidth);
                float cellMaxX = cellMinX + cellWidth;
                float cellMinZ = minZBound + (z * cellDepth);
                float cellMaxZ = cellMinZ + cellDepth;

                float rx = Random.Range(cellMinX, cellMaxX);
                float rz = Random.Range(cellMinZ, cellMaxZ);

                // Convert local offsets to world space ray start position
                Vector3 rayStartLocal = new Vector3(rx, heightMultiplier + 50f, rz);
                Vector3 rayStartWorld = transform.TransformPoint(rayStartLocal);

                Ray ray = new Ray(rayStartWorld, Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit hit, heightMultiplier + 100f))
                {
                    // Check surface steepness to prevent spawning on vertical faces
                    float angle = Vector3.Angle(hit.normal, Vector3.up);
                    if (angle <= maxSlopeAngle)
                    {
                        GameObject prefab = propPrefabs[Random.Range(0, propPrefabs.Length)];
                        GameObject prop = Instantiate(prefab, hit.point, Quaternion.identity, transform);

                        // Random rotation & subtle scaling
                        prop.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                        float randomScale = Random.Range(minScale, maxScale);
                        prop.transform.localScale = Vector3.one * randomScale;

                        spawned++;
                    }
                }
            }
        }
    }

    void ClearProps()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}