using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PointCloudImporter : MonoBehaviour
{
    public Material mat; // Material to apply to the mesh
    public int nPer = 64000; // Number of points per mesh

    // Function to import points from a CSV file
    public void ImportPointCloudFromFile(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning($"File not found: {path}");
            return;
        }

        string[] lines = File.ReadAllLines(path);
        if (lines.Length <= 1) // Ensure there's data to read (excluding the header)
        {
            Debug.LogWarning("No data found in the CSV file.");
            return;
        }

        int totalPoints = lines.Length - 1; // Exclude the header line
        Vector3[] points = new Vector3[totalPoints];
        Color[] colors = new Color[totalPoints];
        int[] indices = new int[totalPoints];

        float[] scalarNum = new float[totalPoints];
        float minScalarNum = float.MaxValue;
        float maxScalarNum = float.MinValue;

        for (int i = 0; i < totalPoints; i++)
        {
            string[] values = lines[i + 1].Split(',');
            if (values.Length < 5) // Ensure there are enough values to parse
            {
                Debug.LogWarning($"Line {i + 1} does not contain enough values: {string.Join(", ", values)}");
                continue; // Skip this iteration if not enough values
            }

            // Parse x, y, z coordinates
            if (float.TryParse(values[2], out float x) &&
                float.TryParse(values[3], out float y) &&
                float.TryParse(values[4], out float z))
            {
                points[i] = new Vector3(x, y, z);
                scalarNum[i] = float.Parse(values[0]); // Assuming scalar value is always present at index 0

                // Update min and max scalar values
                minScalarNum = Mathf.Min(minScalarNum, scalarNum[i]);
                maxScalarNum = Mathf.Max(maxScalarNum, scalarNum[i]);

                // Color mapping based on scalar value
                colors[i] = RemapColor(scalarNum[i]);
                indices[i] = i;
            }
            else
            {
                Debug.LogWarning($"Could not parse coordinates on line {i + 1}: {string.Join(", ", values)}");
            }
        }

        // Create and assign the mesh
        CreateMesh(points, colors, indices);
    }

    private void CreateMesh(Vector3[] points, Color[] colors, int[] indices)
    {
        Mesh mesh = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32,
            vertices = points,
            colors = colors,
        };

        mesh.SetIndices(indices, MeshTopology.Points, 0);

        // Create the GameObject for the point cloud
        GameObject pointCloudObject = new GameObject("PointCloud");

        MeshFilter meshFilter = pointCloudObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = pointCloudObject.AddComponent<MeshRenderer>();
        if (mat == null)
        {
            mat = new Material(Shader.Find("Standard")); // Assign a default material if none is provided
        }
        meshRenderer.material = mat;

        // Optionally set the parent to keep the hierarchy clean
        pointCloudObject.transform.SetParent(this.transform);
    }

    private Color RemapColor(float radius)
    {
        Color minColor = new Color(1, 0, 0);
        Color maxColor = new Color(1, 1, 0);

        float minRadius = 0; // Set your min radius based on your data
        float maxRadius = 1; // Set your max radius based on your data

        float remappedValue = remap(minRadius, maxRadius, 0, 1, radius);
        return Color.Lerp(minColor, maxColor, remappedValue);
    }

    private float remap(float a, float b, float c, float d, float x)
    {
        return c + (x - a) * (d - c) / (b - a);
    }
}
