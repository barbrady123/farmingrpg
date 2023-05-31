using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_SoundList", menuName = "Scriptable Objects/Sounds/Sound List")]
public class SO_SoundList : ScriptableObject
{
    [SerializeField]
    public List<SoundItem> SoundDetails;
}
