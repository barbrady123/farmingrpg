using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonobehavior<AudioManager>
{
    [SerializeField]
    private GameObject _soundPrefab = null;

    [Header("Other")]
    [SerializeField]
    private SO_SoundList _so_soundList = null;

    private Dictionary<SoundName, SoundItem> _soundDictionary;

    protected override void Awake()
    {
        base.Awake();

        _soundDictionary = new Dictionary<SoundName, SoundItem>();

        _so_soundList.SoundDetails.ForEach(x => _soundDictionary.Add(x.SoundName, x));
    }

    public void PlaySound(SoundName soundName)
    {
        if ((soundName == SoundName.None) || (_soundPrefab == null))
            return;

        if (!_soundDictionary.TryGetValue(soundName, out var soundItem))
            return;

        var soundGameObject = PoolManager.Instance.ReuseObject(_soundPrefab, Vector3.zero, Quaternion.identity);

        var sound = soundGameObject.GetComponent<Sound>();
        sound.SetSound(soundItem);

        soundGameObject.SetActive(true);

        StartCoroutine(DisableSound(soundGameObject, soundItem.SoundClip.length));
    }

    private IEnumerator DisableSound(GameObject soundGameObject, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        soundGameObject.SetActive(false);
    }
}
