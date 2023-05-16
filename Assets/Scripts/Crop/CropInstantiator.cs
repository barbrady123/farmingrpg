using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropInstantiator : MonoBehaviour
{
    private Grid _grid;

    [SerializeField]
    private int _daysSinceDug = -1;

    [SerializeField]
    private int _daysSinceWatered = -1;

    [ItemCodeDescription]
    [SerializeField]
    private int _seedItemCode = 0;

    [SerializeField]
    private int _growthDays = 0;

    private void OnEnable()
    {
        EventHandler.InstantiateCropPrefabsEvent += InstantiateCropPrefabs;
    }

    private void OnDisable()
    {
        EventHandler.InstantiateCropPrefabsEvent -= InstantiateCropPrefabs;
    }

    private void InstantiateCropPrefabs()
    {
        _grid = GameObject.FindObjectOfType<Grid>();

        var cropGridPosition = _grid.WorldToCell(transform.position);

        SetCropGridProperties(cropGridPosition);

        Destroy(gameObject);
    }

    private void SetCropGridProperties(Vector3Int cropGridPosition)
    {
        if (_seedItemCode <= 0)
            return;

        var gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition);

        gridPropertyDetails.DaysSinceDug = _daysSinceDug;
        gridPropertyDetails.DaysSinceWatered = _daysSinceWatered;
        gridPropertyDetails.SeedItemCode = _seedItemCode;
        gridPropertyDetails.GrowthDays = _growthDays;
    }
}
