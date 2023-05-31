using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound: MonoBehaviour
{
    private AudioSource _audioSource;

    public void SetSound(SoundItem soundItem)
    {
        _audioSource.pitch = Random.Range(soundItem.SoundPitchRandomVariationMin, soundItem.SoundPitchRandomVariationMax);
        _audioSource.volume = soundItem.SoundVolume;
        _audioSource.clip = soundItem.SoundClip;
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _audioSource?.Play();
    }

    private void OnDisable()
    {
        _audioSource?.Stop();
    }
}
