using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;
public enum SliderType { Master, Sfx, Music}
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    [SerializeField]TMP_Text fpsText;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Material colorBlindMaterial;

    private LocalKeyword cbNone;
    private LocalKeyword cbTritanopia;
    private LocalKeyword cbProtonopia;
    private LocalKeyword cbDeuteranopia;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeColorBlindKeywords();
            return;
        }
        Destroy(gameObject);
    }

    private void InitializeColorBlindKeywords()
    {
        if (colorBlindMaterial != null)
        {
            cbNone = new LocalKeyword(colorBlindMaterial.shader, "_COLORBLINDMODE_NONE");
            cbTritanopia = new LocalKeyword(colorBlindMaterial.shader, "_COLORBLINDMODE_TRITANOPIA");
            cbProtonopia = new LocalKeyword(colorBlindMaterial.shader, "_COLORBLINDMODE_PROTONOPIA");
            cbDeuteranopia = new LocalKeyword(colorBlindMaterial.shader, "_COLORBLINDMODE_DEUTERANOPIA");
        }
    }
    public void ShowFPS(bool showFps) 
    {
        fpsText.gameObject.SetActive(showFps);
        PlayerPrefs.SetInt("showFps", showFps ? 1 : 0);
    
    }
    public void VSync(bool active)
    {
        QualitySettings.vSyncCount = active ? 1 : 0;
        PlayerPrefs.SetInt("vSync", active ? 1 : 0);
    }
    public void SetScreeMode(int index)
    {
        switch (index)
        {
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 3:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
            case 4:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;

        }
                PlayerPrefs.SetInt("screenMode", index);
    }
    public void SetVolume(float to, SliderType sliderType) 
    {   
        float realVolume = Mathf.Log10(to) * 20;
     
        switch (sliderType)
        {
            case SliderType.Master:

                audioMixer.SetFloat("MasterVolume", realVolume);
                PlayerPrefs.SetFloat("masterVolume", to);
                break;
            case SliderType.Sfx:
                audioMixer.SetFloat("SfxVolume", realVolume);
                PlayerPrefs.SetFloat("sfxVolume", to);
                break;
            case SliderType.Music:
                audioMixer.SetFloat("MusicVolume", realVolume);
                PlayerPrefs.SetFloat("musicVolume", to);
                break;
          
        }

    }

    public void SetColorBlindMode(int index)
    {
        if (colorBlindMaterial == null) return;

       
        colorBlindMaterial.SetKeyword(cbNone, false);
        colorBlindMaterial.SetKeyword(cbTritanopia, false);
        colorBlindMaterial.SetKeyword(cbProtonopia, false);
        colorBlindMaterial.SetKeyword(cbDeuteranopia, false);

        
        switch (index)
        {
            case 0: 
                colorBlindMaterial.SetKeyword(cbNone, true);
                break;
            case 1: 
                colorBlindMaterial.SetKeyword(cbTritanopia, true);
                break;
            case 2: 
                colorBlindMaterial.SetKeyword(cbProtonopia, true);
                break;
            case 3: 
                colorBlindMaterial.SetKeyword(cbDeuteranopia, true);
                break;
        }

        PlayerPrefs.SetInt("colorBlindMode", index);
    }
}
