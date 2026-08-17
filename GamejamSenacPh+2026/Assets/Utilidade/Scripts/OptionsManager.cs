using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] Toggle showFps;
    [SerializeField] Toggle vSync;
    [SerializeField] TMP_Dropdown screenMode;
    [SerializeField] TMP_Dropdown colorBlind;
    [SerializeField] Slider master, music, sfx;
    
    void Start()    
    {
        if(showFps)
            showFps.onValueChanged.AddListener(delegate { SettingsManager.Instance.ShowFPS(showFps.isOn); }) ;
        
        if(vSync)
            vSync.onValueChanged.AddListener(delegate { SettingsManager.Instance.VSync(vSync.isOn); });
        
        if(screenMode)
            screenMode.onValueChanged.AddListener(delegate { SettingsManager.Instance.SetScreeMode(screenMode.value); });
        if(colorBlind)
            colorBlind.onValueChanged.AddListener(delegate { SettingsManager.Instance.SetColorBlindMode(colorBlind.value); });
        if(master)  
            master.onValueChanged.AddListener(delegate { SettingsManager.Instance.SetVolume(master.value, SliderType.Master); });
        if(music)
            music.onValueChanged.AddListener(delegate { SettingsManager.Instance.SetVolume(music.value, SliderType.Music); });
        if(sfx)
            sfx.onValueChanged.AddListener(delegate { SettingsManager.Instance.SetVolume(sfx.value, SliderType.Sfx); });

        if (PlayerPrefs.HasKey("showFps")){showFps.isOn = PlayerPrefs.GetInt("showFps") == 1 ? true : false; }
        if (PlayerPrefs.HasKey("vSync")) { vSync.isOn = PlayerPrefs.GetInt("vSync") == 1 ? true : false; }
        if (PlayerPrefs.HasKey("screenMode")) { screenMode.value =PlayerPrefs.GetInt("screenMode"); }
        if (PlayerPrefs.HasKey("colorBlindMode")) { colorBlind.value = PlayerPrefs.GetInt("colorBlindMode"); }
        if (PlayerPrefs.HasKey("masterVolume")) { master.value = PlayerPrefs.GetFloat("masterVolume"); }
        if (PlayerPrefs.HasKey("musicVolume")) { music.value = PlayerPrefs.GetFloat("musicVolume"); }
        if (PlayerPrefs.HasKey("sfxVolume")) { sfx.value = PlayerPrefs.GetFloat("sfxVolume"); }
    }
  
 

}
