//==============================================================================
// ■ AudioManagerEx
//------------------------------------------------------------------------------
// 控制音樂音效的函數
// PS : 
// 記得如果要用程式碼撥放音樂音效的話要用AudioManagerEx來播放
// 以免調整過的音量無效
//==============================================================================

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManagerEx : SingletonObject<AudioManagerEx>
{
    AudioSource audioSource;
    [HideInInspector]
    public float soundvolume;

    // Use this for initialization
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
    }

    /// <summary>
    /// Plays game music using an audio clip.
    /// One music clip may be played at a time.
    /// </summary>
    public void PlayMusic(AudioClip musicClip, bool loop, float atTime)
    {
        if (audioSource == null || audioSource.clip == musicClip)
        {
            return;
        }

        audioSource.clip = musicClip;
        audioSource.loop = loop;
        audioSource.time = atTime;
        audioSource.Play();
    }

    /// <summary>
    /// Plays a sound effect once, at the specified volume.
    /// </summary>
    /// <param name="soundClip">The sound effect clip to play.</param>
    /// <param name="volume">The volume level of the sound effect.</param>
    public virtual void PlaySound(AudioClip soundClip, float volume)
    {
        audioSource.PlayOneShot(soundClip, volume);
    }

    /// <summary>
    /// Plays a sound effect once, at the specified volume.
    /// </summary>
    /// <param name="soundClip">The sound effect clip to play.</param>
    /// <param name="volume">The volume level of the sound effect.</param>
    public virtual void PlaySound(AudioClip soundClip)
    {
        audioSource.PlayOneShot(soundClip, soundvolume);
    }

    /// <summary>
    /// Fades the game music volume to required level over a period of time.
    /// </summary>
    /// <param name="volume">The new music volume value [0..1]</param>
    public virtual void SetAudioVolume(float volume)
    {
        audioSource.volume = volume;
    }

    /// <summary>
    /// Stops playing game music.
    /// </summary>
    public virtual void StopMusic()
    {
        audioSource.Stop();
    }
}