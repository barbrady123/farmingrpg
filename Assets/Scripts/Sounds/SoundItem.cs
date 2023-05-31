using System;
using UnityEngine;

[Serializable]
public class SoundItem
{
    public SoundName SoundName;

    public AudioClip SoundClip;

    public string SoundDescription;

    [Range(0.1f, 1.5f)]
    public float SoundPitchRandomVariationMin = 0.8f;

    [Range(0.1f, 1.5f)]
    public float SoundPitchRandomVariationMax = 1.2f;

    [Range(0f, 1f)]
    public float SoundVolume = 1f;
}
