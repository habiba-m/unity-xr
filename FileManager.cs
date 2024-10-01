using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class FileManager : MonoBehaviour
{
    string path;
    public Material mat = null; // Material to apply to the mesh
    public GameObject parentObject; // Parent object to hold the point cloud

    // Reference to the PointCloudImporter
    private PointCloudImporter pointCloudImporter;

    // Function to open the file explorer and select CSV file
    public void OpenExplorer()
    {
        path = EditorUtility.OpenFilePanel("Select CSV", "", "csv");
        GetFile();
    }

    // Function to check if a file was selected and call import function
    void GetFile()
    {
        if (!string.IsNullOrEmpty(path))
        {
            ReadCSV();
            ImportCSV();
        }
    }

    // Function to read the CSV and log data (optional for debugging)
    void ReadCSV()
    {
        try
        {
            string csvData = File.ReadAllText(path);
            Debug.Log("CSV Data:\n" + csvData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error reading CSV file: " + e.Message);
        }
    }

    // Function to import the CSV file and visualize the data
    void ImportCSV()
    {
        try
        {
            // Ensure the PointCloudImporter is attached to the same GameObject
            if (pointCloudImporter == null)
            {
                pointCloudImporter = GetComponent<PointCloudImporter>();
                if (pointCloudImporter == null)
                {
                    Debug.LogError("PointCloudImporter script not found on the GameObject.");
                    return;
                }
            }

            // Import the CSV as a point cloud mesh
            pointCloudImporter.ImportPointCloudFromFile(path);

            Debug.Log("Successfully imported and visualized CSV data as point cloud from: " + path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error importing CSV file: " + e.Message);
        }
    }
}