using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private const string PLAYER_PREFS_MUSCI_VOLUME = "MusicVolume";
    public static MusicManager Instance { get; private set; }
    private float volume;
    private AudioSource musicSource;
    private void Awake()
    {
        Instance = this;
        musicSource= GetComponent<AudioSource>();
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSCI_VOLUME, 1f);
        musicSource.volume = volume;
    }
    public void ChangeVolume()
    {
        volume += 0.1f;
        if (volume > 1.1f)
        {
            volume = 0;
        }
        musicSource.volume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSCI_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }
}
