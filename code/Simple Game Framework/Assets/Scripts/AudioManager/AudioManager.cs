using System.Collections.Generic;
using DelayedTaskModule;
using PoolModule;
using UnityEngine;
using UnityEngine.Audio;
[DefaultExecutionOrder(-99)]
public class AudioManager : MonoBehaviour
{
    public AudioDataSO audioDataSo;
    public AudioMixer audioMixer;
    private Dictionary<string, SimpleAudioDataConf> audioDic = new Dictionary<string, SimpleAudioDataConf>();
    private static string MusicVolumeKey = "MusicVolume";
    private static string SFXVolumeKey = "SFXVolumeKey";
    private Transform _transform;
    
    private static AudioManager _instance = null;
    
    private List<AudioSource> _playingAudioSources = new List<AudioSource>();
    public static AudioManager Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        _instance = this;
        _transform = transform;
        if (transform.parent)
        {
            DontDestroyOnLoad(transform.parent);    
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        audioDic.Clear();
        if(audioDataSo)
        {
            foreach (var audioData in audioDataSo.Conf.ConfList)
            {
                audioDic.Add(audioData.key, audioData);
            }
        }
        
    }
    
    public AudioSource PlayAudio(string key, Transform audioParent = null)
    {
        if (audioDic.TryGetValue(key, out SimpleAudioDataConf conf))
        {
            AudioClip clip = GameManager.Instance.AssetLoad.LoadAsset<AudioClip>(conf.path);
            if (clip)
            {
                GameObject objAudioSource = UnityObjectPoolFactory.Instance.GetItem<GameObject>("AudioSourceTemplate");

                AudioSource audioSource = objAudioSource != null ? objAudioSource.GetComponent<AudioSource>() : null;
                if (audioSource)
                {
                    if(audioParent)
                        audioSource.transform.SetParent(audioParent, false);
                    else
                        audioSource.transform.SetParent(_transform, false);
                    audioSource.transform.localPosition = Vector3.zero;
                    audioSource.gameObject.SetActive(true);
                    audioSource.clip = clip;
                    audioSource.loop = conf.loop;
                    var groups = audioMixer.FindMatchingGroups(conf.mixerName);
                    audioSource.outputAudioMixerGroup = groups.Length > 0 ? groups[0] : null;
                    audioSource.Play();
                    _playingAudioSources.Add(audioSource);
                    if (!conf.loop)
                    {
                        DelayedTaskScheduler.Instance.AddDelayedTask(
                            TimerUtil.GetLaterMilliSecondsBySecond(clip.length),
                            () =>
                            {
                                RecycleAudioHandle(audioSource);
                            });
                    }
                }

                return audioSource;
            }
        }

        return null;
    }

    public void RecycleAudio(AudioSource audioSource)
    {
        RecycleAudioHandle(audioSource);
    }
    
    private void RecycleAudioHandle(AudioSource audioSource)
    {
        if(audioSource && _playingAudioSources.Contains(audioSource))
        {
            audioSource.Stop();
            _playingAudioSources.Remove(audioSource);
            if(audioSource.gameObject != null)
            {
                UnityObjectPoolFactory.Instance.RecycleItem("AudioSourceTemplate", audioSource.gameObject);
                audioSource.transform.SetParent(_transform, false);
            }
        }
    }

    private void Start()
    {
        //不能放Awake里执行，太早了
        LoadMusicVolumeSetting();
        LoadSFXVolumeSetting();
    }
    

    private void LoadMusicVolumeSetting()
    {
        float volume = 0;
        if(PlayerPrefs.HasKey(MusicVolumeKey))
        {
            volume = PlayerPrefs.GetFloat(MusicVolumeKey);
        }
        
        audioMixer.SetFloat("MusicVolume", volume);
    }
    
    private void LoadSFXVolumeSetting()
    {
        float volume = 0;
        if(PlayerPrefs.HasKey(SFXVolumeKey))
        {
            volume = PlayerPrefs.GetFloat(SFXVolumeKey);
        }
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public float GetMusicVolume()
    {
        float volume = 1;
        audioMixer.GetFloat("MusicVolume", out volume);
        return volume;
    }
    
    public float GetSFXVolume()
    {
        float volume = 1;
        audioMixer.GetFloat("SFXVolume", out volume);
        return volume;
    }
    
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
    }
    
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
    }
}
