using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider, soundVolumeSlider, masterVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle, vsyncToggle;

    public void Start()
    {
        // Declare defaults
        if (!PlayerPrefs.HasKey("musicVolume"))
            PlayerPrefs.SetFloat("musicVolume", 50);

        if (!PlayerPrefs.HasKey("soundVolume"))
            PlayerPrefs.SetFloat("soundVolume", 50);

        if (!PlayerPrefs.HasKey("masterVolume"))
            PlayerPrefs.SetFloat("masterVolume", 100);

        if (!PlayerPrefs.HasKey("fullscreen"))
            PlayerPrefs.SetInt("fullscreen", 1);

        if (!PlayerPrefs.HasKey("vsync"))
            PlayerPrefs.SetInt("vsync", 1);

        // Reload saved settings
        musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume(musicVolumeSlider.value);

        soundVolumeSlider.value = PlayerPrefs.GetFloat("soundVolume");
        SetSoundVolume(musicVolumeSlider.value);

        masterVolumeSlider.value = PlayerPrefs.GetFloat("masterVolume");
        SetMasterVolume(masterVolumeSlider.value);

        if (!Utils.IsWebPlayer())
        {
            fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen") == 1;
            SetFullscreen(fullscreenToggle.isOn);
        }
        
        vsyncToggle.isOn = PlayerPrefs.GetInt("vsync") == 1;
        SetVSync(vsyncToggle.isOn);
    }

    public void SetMusicVolume(float value)
    {
        AkUnitySoundEngine.SetRTPCValue("musicVolume", value);
        PlayerPrefs.SetFloat("musicVolume", value);
    }

    public void SetSoundVolume(float value)
    {
        AkUnitySoundEngine.SetRTPCValue("soundVolume", value);
        PlayerPrefs.SetFloat("soundVolume", value);
    }

    public void SetMasterVolume(float value)
    {
        AkUnitySoundEngine.SetRTPCValue("masterVolume", value);
        PlayerPrefs.SetFloat("masterVolume", value);
    }

    public void SetFullscreen(bool value)
    {
        if (Utils.IsWebPlayer()) return;
        Screen.SetResolution(Display.main.systemWidth, (int)(9 / 16f * Display.main.systemWidth), value);
        PlayerPrefs.SetInt("fullscreen", value ? 1 : 0);
    }

    public void SetVSync(bool value)
    {
        QualitySettings.vSyncCount = value ? 1 : 0;
        PlayerPrefs.SetInt("vsync", value ? 1 : 0);
    }
}