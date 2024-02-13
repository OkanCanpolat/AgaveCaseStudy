using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject 
{
    public PoolObjectType GetPoolObjectType();
    public void OnReturnPool();
}
