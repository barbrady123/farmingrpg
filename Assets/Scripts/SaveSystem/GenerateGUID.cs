using System;
using UnityEngine;

[ExecuteAlways] // Means script will run in editor (on attach) AND during normal game play...
public class GenerateGUID : MonoBehaviour
{
    [SerializeField]
    private string _guid = "";

    public string GUID { get => _guid; set => _guid = value; }

    private void Awake()
    {
        // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            if (_guid == "")
            {
                _guid = Guid.NewGuid().ToString();
            }
        }
    }
}
