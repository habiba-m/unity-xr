using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class FileManager : MonoBehaviour
{
    public Material mat = null;
    public GameObject parentObject;

    private PointCloudImporter pointCloudImporter;

    // Function to open the file explorer and select multiple CSV files
    public void OpenExplorer()
    {
        List<string> selectedPaths = new List<string>();

        // Loop until the user cancels the file selection
        while (true)
        {
            string path = EditorUtility.OpenFilePanel("Select CSV files", "", "csv");

            if (string.IsNullOrEmpty(path))
            {
                break;
            }

            selectedPaths.Add(path);
        }

        // Process all selected files
        foreach (string path in selectedPaths)
        {
            GetFile(path);
        }
    }

    // Function to check if a file was selected and call import function
    void GetFile(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            ReadCSV(path);
            ImportCSV(path);
        }
    }

    // Function to read the CSV and log data (optional for debugging)
    void ReadCSV(string path)
    {
        try
        {
            string csvData = File.ReadAllText(path);
            Debug.Log("CSV Data from " + path + ":\n" + csvData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error reading CSV file " + path + ": " + e.Message);
        }
    }

    // Function to import the CSV file and visualize the data
    void ImportCSV(string path)
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
            Debug.LogError("Error importing CSV file " + path + ": " + e.Message);
        }
    }
}