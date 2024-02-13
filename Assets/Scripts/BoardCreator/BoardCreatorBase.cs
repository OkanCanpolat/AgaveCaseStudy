using UnityEngine;

public abstract class BoardCreatorBase : ScriptableObject
{
    public abstract void CreateBoard(int width, int height, out GameObject[,] board);
}
