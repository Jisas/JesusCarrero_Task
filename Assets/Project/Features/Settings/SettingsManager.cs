using UnityEngine;

public class SettingsManager : MonoBehaviour, IInitializable, IGameService
{
    private SettingsData settingsData;
    //private SoundManager soundManager;

    public SettingsData SettingsData => settingsData;

    public void Initialize()
    {
        settingsData = ServiceLocator.Get<SaveManager>().GetSettingsData();
        //soundManager = ServiceLocator.Get<SoundManager>();
        ApplySettingsByData();
    }

    public void ApplySettingsByData()
    {
        Resolution[] resolutions = Screen.resolutions;
        UnityEngine.Resolution selectedResolution = resolutions[settingsData.ScreenResolution];

        UnityEngine.FullScreenMode selectedMode = FullScreenMode.FullScreenWindow;
        switch (settingsData.ScreenMode)
        {
            case 0:
                selectedMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                selectedMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                selectedMode = FullScreenMode.Windowed;
                break;
        }

        UnityEngine.ShadowQuality selectedShadowQuality = UnityEngine.ShadowQuality.Disable;
        switch (settingsData.ShadowQuality)
        {
            case 0:
                selectedShadowQuality = UnityEngine.ShadowQuality.Disable;
                break;
            case 1:
                selectedShadowQuality = UnityEngine.ShadowQuality.HardOnly;
                break;
            case 2:
                selectedShadowQuality = UnityEngine.ShadowQuality.All;
                break;
        }

        UnityEngine.ShadowResolution selectedShadowResolution = UnityEngine.ShadowResolution.Low;
        switch (settingsData.ShadowResolution)
        {
            case 0:
                selectedShadowResolution = UnityEngine.ShadowResolution.Low;
                break;
            case 1:
                selectedShadowResolution = UnityEngine.ShadowResolution.Medium;
                break;
            case 2:
                selectedShadowResolution = UnityEngine.ShadowResolution.High;
                break;
            case 3:
                selectedShadowResolution = UnityEngine.ShadowResolution.VeryHigh;
                break;
        }

        int frameRate = settingsData.FrameRate == 0 ? 30 :
                    settingsData.FrameRate == 1 ? 60 :
                    settingsData.FrameRate == 2 ? 120 : 30;

        QualitySettings.SetQualityLevel(settingsData.Quality);
        Screen.fullScreenMode = selectedMode;
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode);
        QualitySettings.shadows = selectedShadowQuality;
        QualitySettings.shadowResolution = selectedShadowResolution;
        QualitySettings.globalTextureMipmapLimit = settingsData.TextureResolution;
        QualitySettings.antiAliasing = settingsData.AntiAliasing ? 2 : 0;
        Application.targetFrameRate = frameRate;
        Screen.brightness = settingsData.Brightness;

        //soundManager.SetGeneralVolume();
        //soundManager.SetMusicVolume();
        //soundManager.SetMusicVolume();
        //soundManager.SetEffectsVolume();
        //soundManager.SetUIEffectsVolume();
    }

    #region Graphics
    public void SetQuality(int value) => settingsData.Quality = value;
    public void SetScreenResolution(int value) => settingsData.ScreenResolution = value;
    public void SetScreenMode(int value) => settingsData.ScreenMode = value;
    public void SetTextureResolution(int value) => settingsData.TextureResolution = value;
    public void SetShadowQuality(int value) => settingsData.ShadowQuality = value;
    public void SetShadowResolution(int value) => settingsData.ShadowResolution = value;
    public void SetRenderScale(float value) => settingsData.RenderScale = value;
    public void SetFrameRate(int value) => settingsData.FrameRate = value;
    public void SetBrightness(float value) => settingsData.Brightness = value;
    public void SetAntiAliasing(bool value) => settingsData.AntiAliasing = value;
    #endregion

    #region Sound
    public void SetGeneralVolume(float value) => settingsData.GeneralVolume = value;
    public void SetMusicVolume(float value) => settingsData.MusicVolume = value;
    public void SetEffectsVolume(float value) => settingsData.EffectsVolume = value;
    public void SetAmbientalVolume(float value) => settingsData.AmbientalVolume = value;
    public void SetUIVolume(float value) => settingsData.UIVolume = value;
    #endregion
}
