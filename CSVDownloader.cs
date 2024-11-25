using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CSVDownloader : MonoBehaviour
{
    public string flaskServerURL = "http://localhost:5000";
    public Button startButton, nextButton, backButton, refreshButton, pauseButton, continueButton;

    private List<string> downloadedFiles = new List<string>();
    private PointCloudImporter pointCloudImporter;
    private List<GameObject> pointCloudObjects = new List<GameObject>();
    private int currentTimeStepIndex = 0;
    private bool isLooping = false; // Flag to control the loop
    private bool isPaused = false; // Flag to control pause state

    void Start()
    {
        pointCloudImporter = FindObjectOfType<PointCloudImporter>();
        StartCoroutine(FetchFileList());

        // Attach button listeners
        startButton.onClick.AddListener(OnStartButton);
        nextButton.onClick.AddListener(OnNextButton);
        backButton.onClick.AddListener(OnBackButton);
        refreshButton.onClick.AddListener(OnRefreshButton);
        pauseButton.onClick.AddListener(OnPauseButton);
        continueButton.onClick.AddListener(OnContinueButton);
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
        downloadedFiles.Add(csvData);
    }

    // Start Button: Begin looping from the first timestep
    public void OnStartButton()
    {
        currentTimeStepIndex = 0; // Start from the beginning
        isLooping = true; // Enable looping
        isPaused = false; // Reset pause state
        StopAllCoroutines(); // Stop any ongoing actions
        StartCoroutine(LoopThroughTimeSteps());
    }

    // Continue Button: Resume looping from the current timestep
    public void OnContinueButton()
    {
        if (isPaused)
        {
            isLooping = true; // Enable looping
            isPaused = false; // Reset pause state
            StopAllCoroutines(); // Stop any ongoing actions
            StartCoroutine(LoopThroughTimeSteps());
        }
        else
        {
            Debug.LogWarning("Continue button pressed without being paused.");
        }
    }

    // Next Button: Display the next timestep and hide the current one
    public void OnNextButton()
    {
        StopAllCoroutines(); // Stop any ongoing actions
        isLooping = false; // Stop looping
        isPaused = false; // Reset pause state
        IncrementTimeStep(1);
        DisplayCurrentTimeStep();
    }

    // Back Button: Display the previous timestep and hide the current one
    public void OnBackButton()
    {
        StopAllCoroutines(); // Stop any ongoing actions
        isLooping = false; // Stop looping
        isPaused = false; // Reset pause state
        IncrementTimeStep(-1);
        DisplayCurrentTimeStep();
    }

    // Refresh Button: Reset to the first timestep
    public void OnRefreshButton()
    {
        StopAllCoroutines(); // Stop any ongoing actions
        isLooping = false; // Stop looping
        isPaused = false; // Reset pause state
        currentTimeStepIndex = 0;
        DisplayCurrentTimeStep();
    }

    // Pause Button: Pause the looping
    public void OnPauseButton()
    {
        StopAllCoroutines(); // Stop any ongoing actions
        isLooping = false; // Stop looping
        isPaused = true; // Enable pause state
        Debug.Log("Loop paused.");
    }

    // Coroutine to loop through timesteps
    private IEnumerator LoopThroughTimeSteps()
    {
        while (isLooping)
        {
            DisplayCurrentTimeStep();
            yield return new WaitForSeconds(4f); // Wait for 4 seconds
            IncrementTimeStep(1); // Move to the next timestep
        }
    }

    // Display only the current timestep
    private void DisplayCurrentTimeStep()
    {
        HideAllPointClouds(); // Ensure all timesteps are hidden

        if (currentTimeStepIndex < downloadedFiles.Count)
        {
            if (currentTimeStepIndex < pointCloudObjects.Count)
            {
                pointCloudObjects[currentTimeStepIndex].SetActive(true);
            }
            else
            {
                GameObject pointCloud = pointCloudImporter.ImportPointCloudFromData(downloadedFiles[currentTimeStepIndex]);
                pointCloudObjects.Add(pointCloud);
                pointCloud.SetActive(true);
            }
            Debug.Log($"Displaying timestep {currentTimeStepIndex + 1}");
        }
    }

    // Hide all point clouds
    private void HideAllPointClouds()
    {
        foreach (GameObject pointCloud in pointCloudObjects)
        {
            if (pointCloud != null)
            {
                pointCloud.SetActive(false);
            }
        }
    }

    // Increment timestep index 
    private void IncrementTimeStep(int step)
    {
        currentTimeStepIndex = (currentTimeStepIndex + step + downloadedFiles.Count) % downloadedFiles.Count;
        Debug.Log($"Current timestep index: {currentTimeStepIndex}");
    }

    [System.Serializable]
    public class FileListResponse
    {
        public List<string> files;
    }
}
