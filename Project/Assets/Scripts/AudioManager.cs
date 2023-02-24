using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;


// AudioManager类只挂在SceneStart场景的一个GameObject-AudioManager上，然后通过DontDestroyOnLoad(this);不销毁自己，这样跳转场景时也可以继续播BGM。
// SceneStart场景在运行的生命周期里只执行一次，用来保证GameObject-AudioManager只有一个。

public class AudioManager : MonoBehaviour
{
    //BGM播放器(AudioSource可以理解为播放器)
    public AudioSource BGMSource;
    public AudioMixer BGMMixer;

    private float _volume = 0;
    public float volume
    {
        get { return _volume; }
        private set
        {
            _volume = value;
            OnVolumeChanged();
        }
    }

    private void OnVolumeChanged()
    {
        BGMMixer.SetFloat("MainVolume", _volume);
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
    }
    public static AudioManager instance;
    /// <summary>
    /// 暂停音频
    /// </summary>
    AudioSource[] pauseAudios;

    private void Awake()
    {
        instance = this;
        //通过DontDestroyOnLoad(this);不销毁自己，这样跳转场景时也可以继续播BGM
        DontDestroyOnLoad(this);
    }

    public void PlayBGM()
    {
        if (BGMSource != null)
        {
            BGMSource.loop = true;
            BGMSource.Play();
        }
    }

    public void StopBGM()
    {
        BGMSource.Stop();
    }
}
