using UnityEngine;
using System.Collections.Generic;



[System.Serializable]
public class AudioClipEntry
{
    public string type;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }


    [SerializeField] private AudioClipEntry[] m_audioClips;
    private Dictionary<string, AudioSource> m_audioSourcesDict = new Dictionary<string, AudioSource>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateAudioSources();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void CreateAudioSources()
    {
        foreach (var entry in m_audioClips)
        {
            if (entry.clip != null)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.clip = entry.clip;
                m_audioSourcesDict.Add(entry.type, source);
            }
        }
    }

    public void PlaySound(string _clipName, float _volume = 1f, bool _loop = false)
    {
        if (m_audioSourcesDict.ContainsKey(_clipName))
        {
            AudioSource source = m_audioSourcesDict[_clipName];
            source.loop = _loop;
            source.volume = _volume;
            source.Play();
        }
    }

    public void StopSound(string _clipName)
    {
        if (m_audioSourcesDict.ContainsKey(_clipName))
        {
            AudioSource source = m_audioSourcesDict[_clipName];
            source.Stop();
        }
    }

    public void SetVolume(string _clipName, float _volume)
    {
        if (m_audioSourcesDict.ContainsKey(_clipName))
        {
            AudioSource source = m_audioSourcesDict[_clipName];
            source.volume = Mathf.Clamp01(_volume);
        }
    }

    public void SetPitch(string _clipName, float _pitch)
    {
        if (m_audioSourcesDict.ContainsKey(_clipName))
        {
            AudioSource source = m_audioSourcesDict[_clipName];
            source.pitch = Mathf.Clamp(_pitch, -3f, 3f);
        }
    }

    public void SetLoop(string _clipName, bool _loop)
    {
        if (m_audioSourcesDict.ContainsKey(_clipName))
        {
            AudioSource source = m_audioSourcesDict[_clipName];
            source.loop = _loop;
        }
    }

    public float GetVolume(string _clipName)
    {
        if (m_audioSourcesDict.ContainsKey(_clipName))
        {
            return m_audioSourcesDict[_clipName].volume;
        }
        return 0f;
    }

    public float GetPitch(string _clipName)
    {
        if (m_audioSourcesDict.ContainsKey(_clipName))
        {
            return m_audioSourcesDict[_clipName].pitch;
        }
        return 1f;
    }

    public float GetPlaybackTime(string _clipName)
    {
        if (m_audioSourcesDict.ContainsKey(_clipName))
        {
            return m_audioSourcesDict[_clipName].time;
        }
        return 0f;
    }

    public void ClearAllSounds()
    {
        foreach (var source in m_audioSourcesDict.Values)
        {
            Destroy(source);
        }
        m_audioSourcesDict.Clear();
    }
}