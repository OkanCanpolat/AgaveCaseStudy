using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PoolObjectTypeMap
{
    public PoolObjectType Type;
    public GameObject ObjectPrefab;
}
public enum PoolObjectType
{
    DropBlue, DropRed, DropGreen, DropYellow
}
public class ObjectPool : Singleton<ObjectPool>
{
    [SerializeField] private PoolObjectTypeMap[] poolObjectTypeMaps;
    [SerializeField] private int startingCountPerType;
    private Dictionary<PoolObjectType, Queue<GameObject>> objects;

    public override void Awake()
    {
        base.Awake();
        objects = new Dictionary<PoolObjectType, Queue<GameObject>>();
        SetupPool();
    }
    public GameObject Get(PoolObjectType type)
    {
        foreach (PoolObjectType poolObjectType in objects.Keys)
        {
            if (poolObjectType == type)
            {
                if (objects[poolObjectType].Count <= 0)
                {
                    InstantiatePoolObject(poolObjectType);
                }

                GameObject poolObject = objects[poolObjectType].Dequeue();
                poolObject.SetActive(true);
                return poolObject;
            }
        }

        return null;
    }
    public void ReturnToPool(GameObject returnObject)
    {
        returnObject.SetActive(false);
        IPoolObject poolObject = returnObject.GetComponent<IPoolObject>();
        poolObject.OnReturnPool();

        foreach (PoolObjectType type in objects.Keys)
        {
            if (type == poolObject.GetPoolObjectType())
            {
                objects[type].Enqueue(returnObject);
                return;
            }
        }
    }
    private void InstantiatePoolObject(PoolObjectType type)
    {
        foreach (PoolObjectTypeMap typeMap in poolObjectTypeMaps)
        {
            if (typeMap.Type == type)
            {
                GameObject poolObject = Instantiate(typeMap.ObjectPrefab);
                poolObject.SetActive(false);
                objects[type].Enqueue(poolObject);
                return;
            }
        }
    }
    private void SetupPool()
    {
        foreach (PoolObjectTypeMap typeMap in poolObjectTypeMaps)
        {
            PoolObjectType type = typeMap.Type;

            if (objects.ContainsKey(type)) continue;

            Queue<GameObject> objectQueue = new Queue<GameObject>();

            objects.Add(type, objectQueue);

            for (int i = 0; i < startingCountPerType; i++)
            {
                GameObject poolObject = Instantiate(typeMap.ObjectPrefab);

                poolObject.gameObject.SetActive(false);
                objectQueue.Enqueue(poolObject);
            }
        }
    }

}
