using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;      // Play • Options • Quit
    public GameObject optionsPanel;   // Sliders + Back

    [Header("Sliders")]
    public Slider volumeSlider;
    public Slider sensSlider;

    void Start()
    {
        // fetch saved prefs
        float vol = PlayerPrefs.GetFloat("MasterVol", 0.8f);
        float sens = PlayerPrefs.GetFloat("LookSens", 0.5f);

        volumeSlider.value = vol;
        sensSlider.value = sens;
        AudioListener.volume = vol;
    }

    // ------------ main ------------
    public void PlayGame() => SceneManager.LoadScene(1);
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void OpenOptions()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    // ----------- options ----------
    public void Back()
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
}
