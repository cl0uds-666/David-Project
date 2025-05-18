using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;


public class QuickGuideController : MonoBehaviour
{
    [System.Serializable]
    public class PerkInfo
    {
        public string name;
        public string description;
        public float pathDistance;
        public AudioSource audioSource; // Keeps playing, we just fade volume
    }

    public CinemachineDollyCart dollyCart;
    public TMP_Text perkNameText;
    public TMP_Text perkDescText;
    public PerkInfo[] perks;

    public float lerpSpeed = 3f;
    public float audioFadeSpeed = 5f;

    private int currentIndex = 0;
    private float targetPosition;

    void Start()
    {
        ShowPerk(0);

        // Ensure all AudioSources are playing and volume is handled by us
        foreach (var perk in perks)
        {
            if (perk.audioSource != null)
            {
                perk.audioSource.loop = true;
                if (!perk.audioSource.isPlaying)
                    perk.audioSource.Play();
            }
        }
    }

    void Update()
    {
        // Smoothly move the camera cart to the target position
        if (Mathf.Abs(dollyCart.m_Position - targetPosition) > 0.01f)
        {
            dollyCart.m_Position = Mathf.Lerp(dollyCart.m_Position, targetPosition, Time.deltaTime * lerpSpeed);
        }
        else
        {
            dollyCart.m_Position = targetPosition;
        }

        // Smoothly fade audio in/out
        for (int i = 0; i < perks.Length; i++)
        {
            if (perks[i].audioSource != null)
            {
                float targetVolume = (i == currentIndex) ? 1f : 0f;
                perks[i].audioSource.volume = Mathf.Lerp(perks[i].audioSource.volume, targetVolume, Time.deltaTime * audioFadeSpeed);
            }
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu"); 
    }

    public void Next()
    {
        if (currentIndex < perks.Length - 1)
        {
            currentIndex++;
            ShowPerk(currentIndex);
        }
    }

    public void Prev()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowPerk(currentIndex);
        }
    }

    void ShowPerk(int index)
    {
        currentIndex = index;
        targetPosition = perks[index].pathDistance;
        perkNameText.text = perks[index].name;
        perkDescText.text = perks[index].description;
    }
}
