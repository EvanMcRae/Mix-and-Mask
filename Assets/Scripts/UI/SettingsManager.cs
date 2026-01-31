using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider, soundVolumeSlider, masterVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle, vsyncToggle;
    [SerializeField] private GameObject fullscreenLabel;
    [SerializeField] private Button closeButton;

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
        SetMusicVolume();

        soundVolumeSlider.value = PlayerPrefs.GetFloat("soundVolume");
        SetSoundVolume();

        masterVolumeSlider.value = PlayerPrefs.GetFloat("masterVolume");
        SetMasterVolume();

        if (!Utils.IsWebPlayer())
        {           
            fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen") == 1;
            SetFullscreen();
        }
        else
        {
            fullscreenToggle.gameObject.SetActive(false);
            fullscreenLabel.SetActive(false);

            // Update navigation for other things
            Utils.SetNavigation(vsyncToggle, closeButton, Utils.Direction.DOWN);
            Utils.SetNavigation(soundVolumeSlider, null, Utils.Direction.LEFT);
            Utils.SetNavigation(soundVolumeSlider, null, Utils.Direction.RIGHT);
        }
        
        vsyncToggle.isOn = PlayerPrefs.GetInt("vsync") == 1;
        SetVSync();
    }

    public void SetMusicVolume()
    {
        AkUnitySoundEngine.SetRTPCValue("musicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("musicVolume", musicVolumeSlider.value);
    }

    public void SetSoundVolume()
    {
        AkUnitySoundEngine.SetRTPCValue("soundVolume", soundVolumeSlider.value);
        PlayerPrefs.SetFloat("soundVolume", soundVolumeSlider.value);
    }

    public void SetMasterVolume()
    {
        AkUnitySoundEngine.SetRTPCValue("masterVolume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("masterVolume", masterVolumeSlider.value);
    }

    public void SetFullscreen()
    {
        if (Utils.IsWebPlayer()) return;
        Screen.SetResolution(Display.main.systemWidth, (int)(9 / 16f * Display.main.systemWidth), fullscreenToggle.isOn);
        PlayerPrefs.SetInt("fullscreen", fullscreenToggle.isOn ? 1 : 0);
    }

    public void SetVSync()
    {
        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
        PlayerPrefs.SetInt("vsync", vsyncToggle.isOn ? 1 : 0);
    }
}