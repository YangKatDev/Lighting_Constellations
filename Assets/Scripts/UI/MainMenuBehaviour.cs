using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour
{
    [Header("UI Settings")]
    public Button continueButton;
    public GameObject creditsCanvas;
    public GameObject mainCanvas;
    public GameObject settingsCanvas;

    [Header("Scene Settings")]
    public string gameplaySceneName = "Puzzle_Scene";

    void Awake()
    {
        creditsCanvas.gameObject.SetActive(false);
        settingsCanvas.gameObject.SetActive(false);
    }

    void Start()
    {
        bool hasSave = PlayerPrefs.GetInt("HasSaveData", 0) == 1;
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(hasSave);
        }
    }

    public void OnNewGameClicked()
    {
        PlayerPrefs.SetInt("CurrentLevelIndex", 0);
        PlayerPrefs.SetInt("HasSaveData", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Puzzle_Scene");
    }

    public void OnContinueClicked()
    {
        SceneManager.LoadScene("Puzzle_Scene");
    }

    public void OnCreditsClicked()
    {
        creditsCanvas.gameObject.SetActive(true);
        mainCanvas.gameObject.SetActive(false);
    }

    public void OnSettingsClicked()
    {
        mainCanvas.gameObject.SetActive(false);
        settingsCanvas.gameObject.SetActive(true);
    }

    public void OnBackButtonCalicked()
    {
        mainCanvas.gameObject.SetActive(true);
        creditsCanvas.gameObject.SetActive(false);
        settingsCanvas.gameObject.SetActive(false);
    }
}
