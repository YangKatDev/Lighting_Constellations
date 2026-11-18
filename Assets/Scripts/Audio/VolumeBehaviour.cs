using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeBehaviour : MonoBehaviour
{
    [SerializeField] private AudioMixer volMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        volMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        volMixer.SetFloat("SoundFXVolume", Mathf.Log10(volume) * 20);
    }
}
