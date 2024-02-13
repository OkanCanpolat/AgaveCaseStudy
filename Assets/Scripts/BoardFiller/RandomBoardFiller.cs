using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Random Board Filler")]
public class RandomBoardFiller : BoardFillerBase
{
    [SerializeField] private PoolObjectType[] dropTypesToCreate;
    [SerializeField] private float spawnOffset;
    public override void FillBoard(GameObject[,] board, List<Drop> dropsToControlMatch, BoardColumns boardColumnFeatures)
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i,j] == null && boardColumnFeatures.ColumnType[i] == BoardColumnType.Spawner)
                {
                    Vector2 position = new Vector2(i, j + spawnOffset);
                    GameObject dropObject = ObjectPool.Instance.Get(GetRandomDropType());
                    dropObject.transform.position = position;
                    Drop drop = dropObject.GetComponent<Drop>();
                    drop.Column = i;
                    drop.Row = j;
                    board[i, j] = dropObject;
                    dropsToControlMatch.Add(drop);
                    drop.UpdatePosition();
                }
            }
        }
    }
    private PoolObjectType GetRandomDropType()
    {
        int index = Random.Range(0, dropTypesToCreate.Length);
        return dropTypesToCreate[index];
    }
}
