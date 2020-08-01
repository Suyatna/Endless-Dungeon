using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public AudioMixer mixer;
    public Dropdown dropdown;
    
    private Resolution[] _resArray;

    public void SetVolume(float volume)
    {
        mixer.SetFloat("Volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = _resArray[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
    
    private void Start()
    {
        _resArray = Screen.resolutions;
        dropdown.ClearOptions();
        
        List<string> options = new List<string>();
        int currResIndex = 0;

        for (int i = 0; i < _resArray.Length; i++)
        {
            string option = _resArray[i].width + "x" + _resArray[i].height;
            
            options.Add(option);
            
            if (_resArray[i].width == Screen.currentResolution.width && _resArray[i].height == Screen.currentResolution.height)
            {
                currResIndex = i;
            }
        }
        
        dropdown.AddOptions(options);
        dropdown.value = currResIndex;
        dropdown.RefreshShownValue();
    }
}
