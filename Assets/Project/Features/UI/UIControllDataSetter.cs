using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

#region Enums
public enum UIControlType
{
    Slider,
    Selector
}
public enum SliderSettingsOptions
{
    RenderScale = 0,
    Brightness = 1,
    GeneralVolume = 2,
    MusicVolume = 3,
    AmbientalVolume = 4,
    EffectsVolume = 5,
    UIEffectsVolume = 6,
}
public enum SelectorSettingsOptions
{
    OverAllQuality = 0,
    ScreenResolution = 1,
    ScreenMode = 2,
    TextureResolution = 3,
    ShadowQuality = 4,
    ShadowResolution  = 5,
    FrameRate = 6,
    Antialiasing = 7,
}
public enum AntialiasingOptions
{
    Yes = 0,
    No = 1,
}
public enum FameRateOptions
{
    Rate30 = 0,
    Rate60 = 1,
    Rate120 = 2
}
#endregion

#region Other Class
[Serializable]
public class SettingsSelector
{
    public TMP_Dropdown dropdown;

    public SettingsSelector (TMP_Dropdown dropdown)
    {
        this.dropdown = dropdown;
    }
}

[Serializable]
public class SettingsSlider
{
    public Slider slider;
    public TextMeshProUGUI text;

    public SettingsSlider (Slider slider, TextMeshProUGUI text)
    {
        this.slider = slider;
        this.text = text;
    }
}
#endregion

/// <summary>
/// ATTENTION! This entire class has been brought over from another personal project.
/// </summary>
public class UIControllDataSetter : MonoBehaviour
{
    [SerializeField] private UIControlType controlType;

    [SerializeField]
    private SliderSettingsOptions sliderSettings;

    [SerializeField]
    private SelectorSettingsOptions selectorSettings;

    [Space(5)]

    [SerializeField]
    private SettingsSelector selector;

    [SerializeField]
    private SettingsSlider slider;

    private SaveManager saveManager;
    private SettingsManager settingsManager;
    //private SoundManager soundManager;

    void Start()
    {
        saveManager = ServiceLocator.Get<SaveManager>();
        settingsManager = ServiceLocator.Get<SettingsManager>();
        //soundManager = SoundManager.Instance;

        switch (controlType)
        {
            case UIControlType.Slider:
                SetupSlider();
                break;

            case UIControlType.Selector:
                SetupSelector();
                break;
        }
    }

    void Update()
    {
        switch (controlType)
        {
            case UIControlType.Slider:
                SetSettingsBySlider(slider.slider.value);
                break;

            case UIControlType.Selector:
                SetSettingsBySelector();
                break;
        }
    }

    private void SetupSlider()
    {
        slider.slider.minValue = 0.0f;
        slider.slider.maxValue = 100.0f;

        switch (sliderSettings)
        {
            case SliderSettingsOptions.RenderScale:
                slider.slider.maxValue = 200.0f;
                slider.slider.value = settingsManager.SettingsData.RenderScale * 100;
                break;

            case SliderSettingsOptions.Brightness:
                slider.slider.value = settingsManager.SettingsData.Brightness * 100;
                break;

            case SliderSettingsOptions.GeneralVolume:
                slider.slider.value = settingsManager.SettingsData.GeneralVolume * 100;
                break;

            case SliderSettingsOptions.MusicVolume:
                slider.slider.value = settingsManager.SettingsData.MusicVolume * 100;
                break;

            case SliderSettingsOptions.AmbientalVolume:
                slider.slider.value = settingsManager.SettingsData.AmbientalVolume * 100;
                break;

            case SliderSettingsOptions.EffectsVolume:
                slider.slider.value = settingsManager.SettingsData.EffectsVolume * 100;
                break;

            case SliderSettingsOptions.UIEffectsVolume:
                slider.slider.value = settingsManager.SettingsData.UIVolume * 100;
                break;
        }
    }
    private void SetSettingsBySlider(float value)
    {
        slider.text.text = $"{Math.Round(value, 0)}%";
        float realValue = value / 100;

        switch (sliderSettings)
        {
            case SliderSettingsOptions.RenderScale:
                UniversalRenderPipelineAsset urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
                settingsManager.SetRenderScale(realValue);
                urpAsset.renderScale = realValue;
                break;

            case SliderSettingsOptions.Brightness:
                settingsManager.SetBrightness(realValue);
                Screen.brightness = value;
                break;

            case SliderSettingsOptions.GeneralVolume:
                settingsManager.SetGeneralVolume(realValue);
                //soundManager.SetGeneralVolume();
                break;

            case SliderSettingsOptions.MusicVolume:
                settingsManager.SetMusicVolume(realValue);
                //soundManager.SetMusicVolume();
                break;

            case SliderSettingsOptions.EffectsVolume:
                settingsManager.SetEffectsVolume(realValue);
                //soundManager.SetEffectsVolume();
                break;

            case SliderSettingsOptions.UIEffectsVolume:
                settingsManager.SetUIVolume(realValue);
                //soundManager.SetUIEffectsVolume();
                break;
        }
    }

    private void SetupSelector()
    {
        List<TMP_Dropdown.OptionData> optionsData = new();

        switch (selectorSettings)
        {
            case SelectorSettingsOptions.OverAllQuality:

                for (int i = 0; i < QualitySettings.count; i++)
                {
                    var option = new TMP_Dropdown.OptionData(QualitySettings.names[i]);
                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                selector.dropdown.value = settingsManager.SettingsData.Quality;
                break;

            case SelectorSettingsOptions.ScreenResolution:
                Resolution[] resolutions = Screen.resolutions;
                foreach (var res in resolutions)
                {
                    var option = new TMP_Dropdown.OptionData($"{res.width} x {res.height}");
                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                Resolution currentResolution = Screen.currentResolution;
                int currentResolutionIndex = 0;

                foreach (var res in resolutions)
                {
                    if (res.width == currentResolution.width && res.height == currentResolution.height)
                        currentResolutionIndex = resolutions.IndexOfItem(res);
                }

                selector.dropdown.value = currentResolutionIndex;
                break;

            case SelectorSettingsOptions.ScreenMode:
                string[] screenModes = { "Full Screen", "Borderless Window", "Window" };
                foreach (var mode in screenModes)
                {
                    var option = new TMP_Dropdown.OptionData(mode);
                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                selector.dropdown.value = settingsManager.SettingsData.ScreenMode;
                break;

            case SelectorSettingsOptions.TextureResolution:
                string[] textureResolutions = { "Full Resolution", "Half Resolution", "Quarter Resolution", "Eighth Resolution" };
                foreach (var resolution in textureResolutions)
                {
                    var option = new TMP_Dropdown.OptionData(resolution);
                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                selector.dropdown.value = settingsManager.SettingsData.TextureResolution;
                break;

            case SelectorSettingsOptions.ShadowQuality:
                string[] shadowQualities = { "Deactivated", "Only Hard Shadows", "Hard and Soft Shadows" };
                foreach (var quality in shadowQualities)
                {
                    var option = new TMP_Dropdown.OptionData(quality);
                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                selector.dropdown.value = settingsManager.SettingsData.ShadowQuality;
                break;

            case SelectorSettingsOptions.ShadowResolution:
                string[] shadowResolutions = { "Low", "Middle", "High", "Very High" };
                foreach (var resolution in shadowResolutions)
                {
                    var option = new TMP_Dropdown.OptionData(resolution);
                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                selector.dropdown.value = settingsManager.SettingsData.ShadowResolution;
                break;

            case SelectorSettingsOptions.FrameRate:

                for (int i = 0; i < 3; i++)
                {
                    var temp = i == 0 ? 30 : i == 1 ? 60 : i == 2 ? 120 : 30;
                    var option = new TMP_Dropdown.OptionData($"{temp} FPS");
                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                selector.dropdown.value = settingsManager.SettingsData.FrameRate;
                break;

            case SelectorSettingsOptions.Antialiasing:

                for (int i = 0; i < 1; i++)
                {
                    var option = new TMP_Dropdown.OptionData(
                        System.Enum.GetName(typeof(AntialiasingOptions), i));

                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                selector.dropdown.value = settingsManager.SettingsData.AntiAliasing == true ? 0 : 1;
                break;
        }
    }
    private void SetSettingsBySelector()
    {
        switch (selectorSettings)
        {
            case SelectorSettingsOptions.OverAllQuality:
                settingsManager.SetQuality(selector.dropdown.value);
                QualitySettings.SetQualityLevel(selector.dropdown.value);
                break;

            case SelectorSettingsOptions.ScreenResolution:
                Resolution[] resolutions = Screen.resolutions;
                UnityEngine.Resolution selectedResolution = resolutions[selector.dropdown.value];
                settingsManager.SetScreenResolution(selector.dropdown.value);
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode);
                break;

            case SelectorSettingsOptions.ScreenMode:
                UnityEngine.FullScreenMode selectedMode = FullScreenMode.FullScreenWindow;
                switch (selector.dropdown.value)
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
                settingsManager.SetScreenMode(selector.dropdown.value);
                Screen.fullScreenMode = selectedMode;
                break;

            case SelectorSettingsOptions.TextureResolution:
                settingsManager.SetTextureResolution(selector.dropdown.value);
                QualitySettings.globalTextureMipmapLimit = selector.dropdown.value;
                break;

            case SelectorSettingsOptions.ShadowQuality:
                UnityEngine.ShadowQuality selectedShadowQuality = UnityEngine.ShadowQuality.Disable;
                switch (selector.dropdown.value)
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
                settingsManager.SetShadowQuality(selector.dropdown.value);
                QualitySettings.shadows = selectedShadowQuality;
                break;

            case SelectorSettingsOptions.ShadowResolution:
                UnityEngine.ShadowResolution selectedShadowResolution = UnityEngine.ShadowResolution.Low;
                switch (selector.dropdown.value)
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
                settingsManager.SetShadowResolution(selector.dropdown.value);
                QualitySettings.shadowResolution = selectedShadowResolution;
                break;

            case SelectorSettingsOptions.FrameRate:
                settingsManager.SetFrameRate(selector.dropdown.value);

                int value = 
                    selector.dropdown.value == 0 ? 30 : 
                    selector.dropdown.value == 1 ? 60 : 
                    selector.dropdown.value == 2 ? 120 : 30;

                Application.targetFrameRate = value;
                break;

            case SelectorSettingsOptions.Antialiasing:
                settingsManager.SetAntiAliasing(selector.dropdown.value == 0);
                QualitySettings.antiAliasing = selector.dropdown.value == 0 ? 0 : 2;
                break;
        }
    }
}