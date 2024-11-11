using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class CSVDownloader : MonoBehaviour
{
    public string flaskServerURL = "http://localhost:5000";
    private List<string> downloadedFiles = new List<string>();
    private PointCloudImporter pointCloudImporter;
    private List<GameObject> pointCloudObjects = new List<GameObject>();  // Store the point cloud GameObjects
    private int currentTimeStepIndex = 0;

    void Start()
    {
        pointCloudImporter = FindObjectOfType<PointCloudImporter>();
        StartCoroutine(FetchFileList());
    }

    IEnumerator FetchFileList()
    {
        string url = $"{flaskServerURL}/list-files";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching file list: " + request.error);
            yield break;
        }

        string jsonResponse = request.downloadHandler.text;
        FileListResponse fileList = JsonUtility.FromJson<FileListResponse>(jsonResponse);

        foreach (string fileName in fileList.files)
        {
            StartCoroutine(DownloadCSV(fileName));
        }
    }

    IEnumerator DownloadCSV(string fileName)
    {
        string encodedFileName = UnityWebRequest.EscapeURL(fileName);
        string url = $"{flaskServerURL}/send-to-unity/{encodedFileName}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error downloading file: " + request.error);
            yield break;
        }

        string csvData = request.downloadHandler.text;
        downloadedFiles.Add(csvData);  // Store the CSV data directly for use
    }

    public void StartTimeSeries()
    {
        // Hide all previous time steps and show the current one
        if (currentTimeStepIndex > 0)
        {
            HidePointCloud(currentTimeStepIndex - 1);
        }

        ShowPointCloud(currentTimeStepIndex);

        currentTimeStepIndex++;
        if (currentTimeStepIndex >= downloadedFiles.Count)
        {
            currentTimeStepIndex = 0; // Reset to the first timestep if at the end
        }
    }

    private void ShowPointCloud(int index)
    {
        if (index < downloadedFiles.Count)
        {
            // Create a new GameObject for the current time step's point cloud
            GameObject pointCloud = pointCloudImporter.ImportPointCloudFromData(downloadedFiles[index]);
            pointCloudObjects.Add(pointCloud);  // Store reference to the created GameObject
            Debug.Log($"Showing point cloud for timestep {index + 1}");
        }
    }

    private void HidePointCloud(int index)
    {
        if (index < pointCloudObjects.Count)
        {
            // Hide the GameObject for the previous time step
            GameObject previousPointCloud = pointCloudObjects[index];
            previousPointCloud.SetActive(false);  // Disable the GameObject
            Debug.Log($"Hiding point cloud for timestep {index + 1}");
        }
    }

    [System.Serializable]
    public class FileListResponse
    {
        public List<string> files;
    }
}
