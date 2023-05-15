using System.Collections;
using UnityEngine;

public class VFXManager : SingletonMonobehavior<VFXManager>
{
    private WaitForSeconds _twoSeconds = new WaitForSeconds(2f);

    [SerializeField]
    private GameObject _reapingPrefab = null;

    [SerializeField]
    private GameObject _deciduousLeavesFallingPrefab = null;

    private void OnEnable()
    {
        EventHandler.HarvestActionEffectEvent += DisplayHarvestActionEffect;
    }

    private void OnDisable()
    {
        EventHandler.HarvestActionEffectEvent -= DisplayHarvestActionEffect;
    }

    private void DisplayHarvestActionEffect(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
    {
        switch (harvestActionEffect)
        {
            case HarvestActionEffect.Reaping:
                var reaping = PoolManager.Instance.ReuseObject(_reapingPrefab, effectPosition, Quaternion.identity, true);
                StartCoroutine(DisableHarvestActionEffect(reaping, _twoSeconds));
                break;
            case HarvestActionEffect.DeciduousLeavesFalling:
                var leavingFalling = PoolManager.Instance.ReuseObject(_deciduousLeavesFallingPrefab, effectPosition, Quaternion.identity, true);
                StartCoroutine(DisableHarvestActionEffect(leavingFalling, _twoSeconds));
                break;
        }
    }

    private IEnumerator DisableHarvestActionEffect(GameObject effectGameObject, WaitForSeconds secondsToWait)
    {
        yield return secondsToWait;
        effectGameObject.SetActive(false);
    }
}
