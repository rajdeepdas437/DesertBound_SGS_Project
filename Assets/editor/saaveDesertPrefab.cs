#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class SaveDesertPrefab : Editor
{
    [MenuItem("Desert Tools/Save Selected Desert as Prefab")]
    public static void SaveSelectedDesert()
    {
        GameObject selectedObj = Selection.activeGameObject;

        if (selectedObj == null)
        {
            Debug.LogError("Please select your Desert GameObject in the Hierarchy first!");
            return;
        }

        MeshFilter meshFilter = selectedObj.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("Selected object does not have a valid MeshFilter or generated mesh!");
            return;
        }

        // 1. Ensure a target folder exists
        string folderPath = "Assets/SavedDeserts";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 2. Generate a unique file name timestamp
        string timeStamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string meshPath = $"{folderPath}/DesertMesh_{timeStamp}.asset";
        string prefabPath = $"{folderPath}/DesertPrefab_{timeStamp}.prefab";

        // 3. Save the procedurally generated mesh as an asset on disk
        Mesh meshToSave = Instantiate(meshFilter.sharedMesh);
        AssetDatabase.CreateAsset(meshToSave, meshPath);

        // 4. Temporarily duplicate the object to sanitize it for saving
        GameObject duplicate = Instantiate(selectedObj);
        duplicate.name = $"Desert_{timeStamp}";

        // Assign the saved disk mesh asset to the duplicate
        duplicate.GetComponent<MeshFilter>().sharedMesh = meshToSave;
        if (duplicate.GetComponent<MeshCollider>() != null)
        {
            duplicate.GetComponent<MeshCollider>().sharedMesh = meshToSave;
        }

        // Remove generation scripts so the prefab is static
        DesertGenerator genScript = duplicate.GetComponent<DesertGenerator>();
        if (genScript != null) DestroyImmediate(genScript);

        // 5. Save as a Prefab Asset
        PrefabUtility.SaveAsPrefabAsset(duplicate, prefabPath, out bool success);

        // Cleanup temporary scene object
        DestroyImmediate(duplicate);

        if (success)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"SUCCESS: Desert saved as Prefab at: {prefabPath}");
        }
        else
        {
            Debug.LogError("Failed to save Desert Prefab.");
        }
    }
}
#endif