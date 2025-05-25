using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject quickGuidePanel;
    public GameObject creditsPanel;

    [Header("Sliders")]
    public Slider volumeSlider;
    public Slider sensSlider;

    void Start()
    {
        float vol = PlayerPrefs.GetFloat("MasterVol", 0.8f);
        float sens = PlayerPrefs.GetFloat("LookSens", 0.5f);

        volumeSlider.value = vol;
        sensSlider.value = sens;
        AudioListener.volume = vol;

        ShowMain();
    }

    // -------------------------------
    // Main Buttons
    // -------------------------------
    public void PlayGame()
    {
        SceneManager.LoadScene("LH Floor4");
    }

    public void OpenOptions()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void OpenQuickGuide()
    {
        mainPanel.SetActive(false);
        quickGuidePanel.SetActive(true);
    }

    public void OpenCredits()
    {
        mainPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // -------------------------------
    // Quick Guide Buttons
    // -------------------------------
    public void OpenPerksGuide()
    {
        SceneManager.LoadScene("QuickGuide_Perks");
    }

    public void OpenDropsGuide()
    {
        SceneManager.LoadScene("QuickGuide_Drops");
    }

    public void BackFromQuickGuide()
    {
        quickGuidePanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    // -------------------------------
    // Options
    // -------------------------------
    public void BackFromOptions()
    {
        optionsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void OnVolChanged(float v)
    {
        AudioListener.volume = v;
        PlayerPrefs.SetFloat("MasterVol", v);
    }

    public void OnSensChanged(float s)
    {
        PlayerPrefs.SetFloat("LookSens", s);
    }

    // -------------------------------
    // Credits
    // -------------------------------
    public void BackFromCredits()
    {
        creditsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    // -------------------------------
    // Reset View
    // -------------------------------
    private void ShowMain()
    {
        mainPanel.SetActive(true);
        optionsPanel.SetActive(false);
        quickGuidePanel.SetActive(false);
        creditsPanel.SetActive(false);
    }
}
