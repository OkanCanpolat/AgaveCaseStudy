using System.Collections.Generic;
using UnityEngine;

public abstract class BoardFillerBase : ScriptableObject
{
    public abstract void FillBoard(GameObject[,] board, List<Drop> dropsToControlMatch, BoardColumns boardColumnFeatures);
}
