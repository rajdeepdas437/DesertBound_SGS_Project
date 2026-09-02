using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DesertGenerator : MonoBehaviour
{

    [Header("World Size")]
    [SerializeField] float cellsize = 2f;

    
    [Header("Terrain Dimensions")]
    public int width;
    public int depth;
    public float scale;
    public float heightMultiplier ;

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

        int vertexCount = width * depth;

        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(width - 1) * (depth - 1) * 6];

        // Generate vertices
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                int index = x * depth + z;

                vertices[index] = GetPoint(x, z);
            }
        }

        // Generate triangles
        int triangleIndex = 0;

        for (int x = 0; x < width - 1; x++)
        {
            for (int z = 0; z < depth - 1; z++)
            {
                int current = x * depth + z;
                int nextX = (x + 1) * depth + z;
                int nextZ = x * depth + (z + 1);
                int nextXZ = (x + 1) * depth + (z + 1);

                // Triangle 1
                triangles[triangleIndex++] = current;
                triangles[triangleIndex++] = nextZ;
                triangles[triangleIndex++] = nextX;

                // Triangle 2
                triangles[triangleIndex++] = nextX;
                triangles[triangleIndex++] = nextZ;
                triangles[triangleIndex++] = nextXZ;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Update collider
        MeshCollider collider = GetComponent<MeshCollider>();

        if (collider == null)
            collider = gameObject.AddComponent<MeshCollider>();

        collider.sharedMesh = null;
        collider.sharedMesh = mesh;
    }

    Vector3 GetPoint(int x, int z)
    {
        float y = Mathf.PerlinNoise(
        (x + seed) / scale,
        (z + seed) / scale
        ) * heightMultiplier;

        float xPos = x - (width - 1) / 2f;
        float zPos = z - (depth - 1) / 2f;

        return new Vector3(
            xPos,
            y,
            zPos
        );
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