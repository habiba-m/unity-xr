using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject menuButton;  
    public GameObject menuPanel; 
    void Start()
    {
        menuButton.SetActive(true);
        menuPanel.SetActive(false);

        menuButton.GetComponent<Button>().onClick.AddListener(OpenMenu);
        menuPanel.transform.Find("Close").GetComponent<Button>().onClick.AddListener(CloseMenu);
    }

    public void OpenMenu()
    {
        menuButton.SetActive(false);
        menuPanel.SetActive(true); 
    }
    public void CloseMenu()
    {
        menuPanel.SetActive(false);
        menuButton.SetActive(true); 
    }
}
