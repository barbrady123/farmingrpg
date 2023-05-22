using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class TilemapGridProperties : MonoBehaviour
{
#if UNITY_EDITOR
    private Tilemap _tilemap;

    private Grid _grid;

    [SerializeField]
    private SO_GridProperties _gridProperties = null;

    [SerializeField]
    private GridBoolProperty _gridBoolProperty = GridBoolProperty.Diggable;

    private void OnEnable()
    {
        // Only populate in the editor
        if (Application.IsPlaying(gameObject))
            return;

        _tilemap = GetComponent<Tilemap>();
        _gridProperties?.GridPropertyList?.Clear();
    }

    private void OnDisable()
    {
        // Only populate in the editor
        if (Application.IsPlaying(gameObject))
            return;

        UpdateGridProperties();

        if (_gridProperties != null)
        {
            // This is required to ensure that the updated gridproperties gameobject gets saved when the game is saved
            // otherwise they are not saved
            EditorUtility.SetDirty(_gridProperties);
        }
    }

    private void UpdateGridProperties()
    {
        // Compress tilemap bounds
        _tilemap.CompressBounds();

        // Only populate in the editor
        if (Application.IsPlaying(gameObject))
            return;

        if (_gridProperties == null)
            return;

        var startCell = _tilemap.cellBounds.min;
        var endCell = _tilemap.cellBounds.max;

        // should these be <= ????
        for (int x = startCell.x; x < endCell.x; x++)
        {
            for (int y = startCell.y; y < endCell.y; y++)
            {
                var tile = _tilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    _gridProperties.GridPropertyList.Add(new GridProperty(new GridCoordinate(x, y), _gridBoolProperty, true));
                }
            }
        }
    }

    private void Update()
    {
        // Only populate in the editor
        if (Application.IsPlaying(gameObject))
            return;

        Debug.Log("DISABLE PROPERTY TILEMAPS");
    }
#endif
}
