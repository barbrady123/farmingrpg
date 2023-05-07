using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControllerManager : SingletonMonobehavior<SceneControllerManager>
{
    [SerializeField]
    private float _fadeDuration = 1;

    [SerializeField]
    private CanvasGroup _faderCanvasGroup = null;

    [SerializeField]
    private Image _faderImage = null;

    private bool _isFading;

    public SceneName StartSceneName;

    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        if (_isFading)
            return;

        StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
    }

    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        yield return StartCoroutine(Fade(1f));

        // Store scene data
        SaveLoadManager.Instance.StoreCurrentSceneData();

        Player.Instance.gameObject.transform.position = spawnPosition;

        EventHandler.CallBeforeSceneUnloadEvent();

        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        EventHandler.CallAfterSceneLoadEvent();

        // Restore scene data
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        yield return StartCoroutine(Fade(0f));

        EventHandler.CallAfterSceneLoadFadeInEvent();
    }

    private IEnumerator Fade(float finalAlpha)
    {
        _isFading = true;
        _faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(_faderCanvasGroup.alpha - finalAlpha) / _fadeDuration;

        while (!Mathf.Approximately(_faderCanvasGroup.alpha, finalAlpha))
        {
            _faderCanvasGroup.alpha = Mathf.MoveTowards(_faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        _isFading = false;
        _faderCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        var newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    private IEnumerator Start()
    {
        _faderImage.SetImageOpaque();
        _faderCanvasGroup.alpha = 1f;

        yield return StartCoroutine(LoadSceneAndSetActive(this.StartSceneName.ToString()));

        EventHandler.CallAfterSceneLoadEvent();

        // Restore scene data
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        StartCoroutine(Fade(0f));
    }
}
