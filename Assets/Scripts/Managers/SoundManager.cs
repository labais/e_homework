using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EasyButtons;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour
{
    public static SoundManager I { get; private set; }

    [SerializeField] private AudioClip[] Clips;

    private const int NumFxChannels = 10;
    private const float Volume = 1f;

    private readonly AudioSource[] _fxChannels = new AudioSource[NumFxChannels];
    private readonly Dictionary<string, AudioClip> _fxClips = new Dictionary<string, AudioClip>();
    private int _lastUsedFxChannel;

    private void Awake()
    {
        I = this;

        for (var i = 0; i < NumFxChannels; i++)
        {
            _fxChannels[i] = gameObject.AddComponent<AudioSource>();
        }

        for (var i = 0; i < Clips.Length; i++)
        {
            _fxClips[Clips[i].name] = Clips[i];
        }
    }

    public void Play(string fxName)
    {
        if (Time.frameCount > 0)
        {
            Play(fxName, 1, true);
        }
    }

    public void Play(string fxName, float volume, bool pitchItRND)
    {
        float pitch = 1;
        if (pitchItRND)
        {
            pitch = Random.Range(0.95f, 1.05f);
        }

        Play(fxName, volume, pitch);
    }

    public void Play(string fxName, float volume, float pitch)
    {
        if (_fxClips.ContainsKey(fxName))
        {
            var freeChannelId = GetFreeChannel();
            _fxChannels[freeChannelId].pitch = pitch;
            _fxChannels[freeChannelId].volume = volume * Volume;
            _fxChannels[freeChannelId].clip = _fxClips[fxName];
            _fxChannels[freeChannelId].Play();
        }
        else
        {
            Debug.LogError("SoundManager::no sound: " + fxName);
        }
    }

    private int GetFreeChannel()
    {
        for (int i = _lastUsedFxChannel; i < NumFxChannels; i++)
        {
            if (!_fxChannels[i].isPlaying)
            {
                _lastUsedFxChannel = i;
                return i;
            }
        }

        for (int i = 0; i < NumFxChannels; i++)
        {
            if (!_fxChannels[i].isPlaying)
            {
                _lastUsedFxChannel = i;
                return i;
            }
        }

        _lastUsedFxChannel++;
        if (_lastUsedFxChannel >= NumFxChannels)
        {
            _lastUsedFxChannel = 0;
        }

        return _lastUsedFxChannel;
    }

}