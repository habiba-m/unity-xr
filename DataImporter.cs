using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointCloudImporter : MonoBehaviour
{
    public Material mat; 
    public TMP_Dropdown scalarDropdown; 
    private string[] headers; 
    private string[][] csvData; 

    private GameObject currentPointCloud; 
    private int currentTimestep = 0; 


    // Import CSV and parse data
    public void ImportPointCloudFromData(string csvText)
    {
  
        string[] lines = csvText.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length <= 1) 
        {
            Debug.LogWarning("CSV contains no valid data.");
            return;
        }

        headers = lines[0].Split(','); 
        csvData = new string[lines.Length - 1][];

        for (int i = 1; i < lines.Length; i++)
        {
            csvData[i - 1] = lines[i].Split(',');
        }

        PopulateDropdown();
        DisplayPointCloud(0); 
    
    }

    private void PopulateDropdown()
    {
        scalarDropdown.ClearOptions();
        List<string> options = new List<string>();

        
        for (int i = 0; i < headers.Length - 3; i++)
        {
            options.Add(headers[i]);
        }

        scalarDropdown.AddOptions(options);
        scalarDropdown.onValueChanged.AddListener(OnScalarColumnChanged);
    }

   
    private void OnScalarColumnChanged(int index)
    {
        scalarDropdown.SetValueWithoutNotify(index);
        DisplayPointCloud(index);
    }

    public void SetDropdownSelection(int index)
    {
        scalarDropdown.SetValueWithoutNotify(index); 
        OnScalarColumnChanged(index); 
    }

    public void DisplayCurrentSelection()
    {
        CSVDownloader csvDownloader = FindObjectOfType<CSVDownloader>();
        int scalarIndex = csvDownloader.GetSelectedScalarIndex();
        scalarDropdown.SetValueWithoutNotify(scalarIndex);  
        DisplayPointCloud(scalarIndex);
    }


    // Create and display the point cloud
    private void DisplayPointCloud(int scalarIndex)
    {
       
        if (currentPointCloud != null)
        {
            Destroy(currentPointCloud);
        }

        int totalPoints = csvData.Length;
        Vector3[] points = new Vector3[totalPoints];
        Color[] colors = new Color[totalPoints];
        int[] indices = new int[totalPoints];

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < totalPoints; i++)
        {
            string[] row = csvData[i];
            if (row.Length < 5) continue; 

          
            if (float.TryParse(row[headers.Length - 3], out float x) &&
                float.TryParse(row[headers.Length - 2], out float y) &&
                float.TryParse(row[headers.Length - 1], out float z))
            {
                points[i] = new Vector3(x, y, z);

               
                if (float.TryParse(row[scalarIndex], out float scalar))
                {
                    minValue = Mathf.Min(minValue, scalar);
                    maxValue = Mathf.Max(maxValue, scalar);
                    colors[i] = RemapColor(scalar, minValue, maxValue);
                }

                indices[i] = i;
            }
        }

       
        CreateMesh(points, colors, indices);
    }

    // Create the point cloud mesh
    private void CreateMesh(Vector3[] points, Color[] colors, int[] indices)
    {
        Mesh mesh = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32,
            vertices = points,
            colors = colors,
        };
        mesh.SetIndices(indices, MeshTopology.Points, 0);
        currentPointCloud = new GameObject("PointCloud");
        MeshFilter meshFilter = currentPointCloud.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = currentPointCloud.AddComponent<MeshRenderer>();
        meshRenderer.material = mat ?? new Material(Shader.Find("Standard"));

        currentPointCloud.transform.SetParent(this.transform, false);
    }

    // Map scalar value to color
    private Color RemapColor(float value, float minValue, float maxValue)
    {
        Color minColor = Color.red;
        Color maxColor = Color.yellow;
        float t = Mathf.InverseLerp(minValue, maxValue, value);
        return Color.Lerp(minColor, maxColor, t);
    }
    public void SetTimestep(int timestep)
    {
        currentTimestep = timestep;
        DisplayPointCloud(currentTimestep);
    }

    public int GetTotalTimesteps()
    {
        return csvData.Length;
    }
}
