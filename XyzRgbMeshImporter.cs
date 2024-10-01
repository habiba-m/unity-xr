using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class XyzRgbMeshImporter : MonoBehaviour
{
    public Material mat = null;
    public int nPer = 64000;

    // Function to import CSV file as a point cloud mesh and return the parent GameObject
    public GameObject ImportXyzMeshes(string path)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> indices = new List<int>();
        int nRead = 0;
        bool firstRead = true;
        string parentName = Path.GetFileNameWithoutExtension(path);

        GameObject theParent = new GameObject(parentName);
        string childName = string.Format("child{0:D5}", nRead / nPer);
        string meshName = string.Format("mesh{0:D5}", nRead / nPer);

        GameObject theChild = new GameObject(childName);
        using (StreamReader sr = new StreamReader(path))
        {
            string line;
            int index = 0;
            while ((line = sr.ReadLine()) != null)
            {
                // Log the line to debug
                Debug.Log("Reading Line: " + line);

                string[] parts = line.Split(' ');

                // Log parts to verify
                Debug.Log("Parsed parts: " + string.Join(", ", parts));

                if (parts.Length >= 6)
                {
                    // Parse x, y, z coordinates from the last three values
                    Vector3 vertex = new Vector3(
                        float.Parse(parts[3]), // x-coordinate
                        float.Parse(parts[4]), // y-coordinate
                        float.Parse(parts[5])  // z-coordinate
                    );
                    vertices.Add(vertex);
                    indices.Add(index++);
                    Debug.Log("Parsed Vertex: " + vertex); // Log parsed vertex

                    // Optional: If you want to use the scalar values, add your logic here
                    // float scalar1 = float.Parse(parts[0]);
                    // float scalar2 = float.Parse(parts[1]);
                    // float scalar3 = float.Parse(parts[2]);
                }
                else
                {
                    Debug.LogWarning("Line does not contain enough data for a vertex.");
                }

                // Set a default color for now, since no color data is provided in your format
                colors.Add(Color.white);

                nRead++;

                // Create a new child object if necessary
                if (nRead % nPer == 0 && !firstRead)
                {
                    CreateChildMesh(theChild, theParent, vertices, colors, indices, meshName, childName);
                    childName = string.Format("child{0:D5}", nRead / nPer);
                    meshName = string.Format("mesh{0:D5}", nRead / nPer);
                    theChild = new GameObject(childName);
                    vertices = new List<Vector3>();
                    colors = new List<Color>();
                    indices = new List<int>();
                    index = 0;
                }
                firstRead = false;
            }

            if (nRead % nPer > 0)
            {
                CreateChildMesh(theChild, theParent, vertices, colors, indices, meshName, childName);
            }
        }
        return theParent;
    }

    // Function to create a mesh and attach it to a child GameObject
    void CreateChildMesh(GameObject child, GameObject parent, List<Vector3> vertices, List<Color> colors, List<int> indices, string meshName, string childName)
    {
        if (vertices.Count == 0)
        {
            Debug.LogWarning("No vertices found when creating child mesh: " + childName);
            return;
        }

        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            colors = colors.ToArray(),
        };
        mesh.SetIndices(
            indices.ToArray(),
            MeshTopology.Points,
            0
        );
        mesh.RecalculateBounds();

        mesh.name = meshName;
        MeshFilter mf = child.AddComponent<MeshFilter>();
        MeshRenderer mr = child.AddComponent<MeshRenderer>();

        mf.mesh = mesh;

        if (mat == null)
        {
            mat = new Material(Shader.Find("PointCloud/HexagonOpaque")); // Ensure the shader is correct
        }
        mr.material = mat;
        child.transform.parent = parent.transform;
    }
}
