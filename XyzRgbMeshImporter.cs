using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class XyzRgbMeshImporter
{
    public Material mat = null;
    public int nPer = 64000;
    public bool hasHeaders = true;

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
            string headers = sr.ReadLine();
        
            while ((line = sr.ReadLine()) != null)
            {
                if (nRead % nPer == 0 && !firstRead)
                {
                    Mesh mesh = CreateMesh(vertices, colors, indices);
                    if (mesh != null)
                    {
                        mesh.name = meshName;
                        AssignMeshToGameObject(theChild, theParent, mesh);
                    }
                    ResetMesh(ref vertices, ref colors, ref indices, ref theChild, ref childName, ref meshName, nRead);
                    index = 0;
                }
                firstRead = false;

                string[] parts = line.Split(',');
                if (parts.Length >= 6)
                {
                    Vector3 vertex = new Vector3(
                        float.Parse(parts[3]),
                        float.Parse(parts[4]),
                        float.Parse(parts[5])
                    );
                    vertices.Add(vertex);
                    indices.Add(index++);
                
                    Color color = new Color(1.0f,1,1);
                    colors.Add(color);
                }
                else
                {
                    colors.Add(Color.white);
                }

                nRead++;
            }

            if (nRead % nPer > 0)
            {
                Mesh mesh = CreateMesh(vertices, colors, indices);
                if (mesh != null)
                {
                    AssignMeshToGameObject(theChild, theParent, mesh);
                }
            }
        }

        return theParent;
    }

    Mesh CreateMesh(List<Vector3> vertices, List<Color> colors, List<int> indices)
    {
        if (vertices.Count == 0)
        {
            Debug.LogWarning("No vertices found in the CSV file.");
            return null;
        }

        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            colors = colors.ToArray(),
        };
        mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
        mesh.RecalculateBounds();

        return mesh;
    }

    void ResetMesh(ref List<Vector3> vertices, ref List<Color> colors, ref List<int> indices, ref GameObject theChild, ref string childName, ref string meshName, int nRead)
    {
        vertices = new List<Vector3>();
        colors = new List<Color>();
        indices = new List<int>();
        theChild = new GameObject(childName);
        childName = string.Format("child{0:D5}", nRead / nPer);
        meshName = string.Format("mesh{0:D5}", nRead / nPer);
    }

    void AssignMeshToGameObject(GameObject theChild, GameObject theParent, Mesh mesh)
    {
        MeshFilter mf = theChild.AddComponent<MeshFilter>();
        MeshRenderer mr = theChild.AddComponent<MeshRenderer>();

        mf.mesh = mesh;



        if (mat == null) mat = (Material) Resources.Load("PointCloud",typeof(Material));
        mr.material = mat;

        theChild.transform.parent = theParent.transform;
    }
}
