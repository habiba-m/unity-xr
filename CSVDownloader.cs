using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class CSVDownloader : MonoBehaviour
{
    public string flaskServerURL = "http://localhost:5000";
    public Button startButton;
    public TMP_Dropdown scalarDropdown; 
    public TextMeshProUGUI timestepCounter; 
    public Button playButton, pauseButton, nextButton, backButton, refreshButton;
    public int headerFontSize = 10;
    public int optionFontSize = 10;

    private List<string> downloadedFiles = new List<string>();
    private PointCloudImporter pointCloudImporter;
    private Coroutine playbackCoroutine;
    private bool isPlaying = false;
    private bool isPointCloudInitialized = false;
    private int currentTimestep = 0;
    private int selectedScalarIndex = 0;
    void Start()
    {
        pointCloudImporter = FindObjectOfType<PointCloudImporter>();
        startButton.onClick.AddListener(OnStartButton);

        StartCoroutine(FetchFileList());

        TMP_Text headerText = scalarDropdown.captionText;
        playButton.onClick.AddListener(OnPlay);
        pauseButton.onClick.AddListener(OnPause);
        nextButton.onClick.AddListener(OnNext);
        backButton.onClick.AddListener(OnBack);
        refreshButton.onClick.AddListener(OnRefresh);
        scalarDropdown.onValueChanged.AddListener(OnScalarDropdownChanged);

        UpdateTimestepCounter(); 
        scalarDropdown.value = selectedScalarIndex;
        if (headerText != null)
        {
            headerText.fontSize = headerFontSize;
        }

        foreach (var item in scalarDropdown.options)
        {
            TMP_Text optionText = scalarDropdown.itemText;
            if (optionText != null)
            {
                optionText.fontSize = optionFontSize;
            }
        }
    }
    private void OnScalarDropdownChanged(int index)
    {
        selectedScalarIndex = index; 
        Debug.Log($"Dropdown changed to index {index}");
    }

    public int GetSelectedScalarIndex()
    {
        return selectedScalarIndex;
    }
    private void UpdateTimestepCounter()
    {
        if (timestepCounter != null)
        {
            timestepCounter.text = $"Timestep: {currentTimestep + 1}/{downloadedFiles.Count}";
        }
        else
        {
            Debug.LogWarning("Timestep counter UI not set.");
        }
    }

    // Fetch file list from server
    IEnumerator FetchFileList()
    {
        string url = $"{flaskServerURL}/list-files";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error fetching file list: {request.error}");
            yield break;
        }

        FileListResponse fileList = JsonUtility.FromJson<FileListResponse>(request.downloadHandler.text);

        foreach (string fileName in fileList.files)
        {
            StartCoroutine(DownloadCSV(fileName));
        }
    }

    // Download a CSV file
    IEnumerator DownloadCSV(string fileName)
    {
        string encodedFileName = UnityWebRequest.EscapeURL(fileName);
        string url = $"{flaskServerURL}/send-to-unity/{encodedFileName}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error downloading file: {request.error}");
            yield break;
        }

        downloadedFiles.Add(request.downloadHandler.text);
    }

    
    public void OnStartButton()
    {
        if (isPointCloudInitialized) return;

        if (downloadedFiles.Count > 0)
        {
            Debug.Log("Initializing point cloud from OnStartButton...");
            string csvData = downloadedFiles[0]; 
            pointCloudImporter.ImportPointCloudFromData(csvData);
            isPointCloudInitialized = true;
            UpdateTimestepCounter();
        }
        else
        {
            Debug.LogWarning("No files downloaded yet.");
        }
    }

    private void OnPlay()
    {
        if (!isPlaying && isPointCloudInitialized)
        {
            isPlaying = true;
            playbackCoroutine = StartCoroutine(PlayTimelapse());
        }
    }

    private void OnPause()
    {
        if (isPlaying)
        {
            isPlaying = false;
            StopCoroutine(playbackCoroutine);
        }
    }

    private void OnNext()
    {
        if (isPointCloudInitialized && currentTimestep < downloadedFiles.Count - 1)
        {
            currentTimestep++;
            ShowTimestep(currentTimestep);
        }
    }

    private void OnBack()
    {
        if (isPointCloudInitialized && currentTimestep > 0)
        {
            currentTimestep--;
            ShowTimestep(currentTimestep);
        }
    }

    private void OnRefresh()
    {
        if (isPointCloudInitialized)
        {
              currentTimestep = 0;
            ShowTimestep(currentTimestep);
        }
    }

    private IEnumerator PlayTimelapse()
    {
        while (isPlaying)
        {
            if (currentTimestep < downloadedFiles.Count - 1)
            {
                currentTimestep++;
                ShowTimestep(currentTimestep);
                yield return new WaitForSeconds(1f); 
            }
            else
            {
                isPlaying = false;
            }
        }
    }

    private void ShowTimestep(int timestepIndex)
    {
        if (timestepIndex >= 0 && timestepIndex < downloadedFiles.Count)
        {
            currentTimestep = timestepIndex;
            string csvData = downloadedFiles[currentTimestep];
            pointCloudImporter.ImportPointCloudFromData(csvData);
            pointCloudImporter.DisplayCurrentSelection();
            UpdateTimestepCounter();
            Debug.Log($"Displaying timestep {currentTimestep + 1} with scalar index {GetSelectedScalarIndex()}.");
        }
        else
        {
            Debug.LogWarning("Invalid timestep index.");
        }
    }

}

[System.Serializable]
public class FileListResponse
{
    public List<string> files;
}
