using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonMonobehavior<PoolManager>
{
    private Dictionary<int, Queue<GameObject>> _poolDictionary = new Dictionary<int, Queue<GameObject>>();

    [SerializeField]
    private Pool[] _pool = null;

    [SerializeField]
    private Transform _objectPoolTransform = null;

    [Serializable]
    public struct Pool
    {
        public int PoolSize;

        public GameObject Prefab;
    }

    void Start()
    {
        for (int x = 0; x < _pool.Length; x++)
        {
            CreatePool(_pool[x].Prefab, _pool[x].PoolSize);
        }
    }

    private void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID();

        if (_poolDictionary.ContainsKey(poolKey))
            throw new Exception($"Pool with key '{poolKey}' already exists");

        _poolDictionary.Add(poolKey, new Queue<GameObject>(poolSize));

        // Create parent gameobject to parent the child objects to
        var parentGameObject = new GameObject($"{prefab.name}Anchor");
        parentGameObject.transform.SetParent(_objectPoolTransform);

        for (int x = 0; x < poolSize; x++)
        {
            var newObject = Instantiate(prefab, parentGameObject.transform);
            newObject.SetActive(false);

            _poolDictionary[poolKey].Enqueue(newObject);
        }
    }

    public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation, bool asActive = false)
    {
        int poolKey = prefab.GetInstanceID();
        if (!_poolDictionary.ContainsKey(poolKey))
            throw new Exception($"Pool '{poolKey}' does not exist");

        var objectToReuse = GetObjectFromPool(poolKey);
        ResetObject(position, rotation, objectToReuse, prefab);

        if (asActive)
        {
            objectToReuse.SetActive(true);
        }

        return objectToReuse;
    }

    private GameObject GetObjectFromPool(int poolKey)
    {
        var objectToReuse = _poolDictionary[poolKey].Dequeue();
        _poolDictionary[poolKey].Enqueue(objectToReuse);

        if (objectToReuse.activeSelf)
        {
            objectToReuse.SetActive(false);
        }

        return objectToReuse;
    }

    private static void ResetObject(Vector3 position, Quaternion rotation, GameObject objectToReuse, GameObject prefab)
    {
        objectToReuse.transform.position = position;
        objectToReuse.transform.rotation = rotation;

        objectToReuse.transform.localScale = prefab.transform.localScale;
    }
}
