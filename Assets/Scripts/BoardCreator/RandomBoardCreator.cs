using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Random Board Creator")]
public class RandomBoardCreator : BoardCreatorBase
{
    [SerializeField] private List<PoolObjectType> dropsTypesToCreate;
    [SerializeField] private GameObject tilePrefab;

    public override void CreateBoard(int width, int height, out GameObject[,] board)
    {

        board = new GameObject[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 position = new Vector3(i, j, -1);
                Vector3 Tileposition = new Vector3(i, j, 0);

                GameObject dropObject = GetRandomDrop(i, j);
                dropObject.transform.position = position;
                Instantiate(tilePrefab, Tileposition, Quaternion.identity);
                board[i, j] = dropObject;
            }
        }
    }
    private GameObject GetRandomDrop(int column, int row)
    {
        dropsTypesToCreate.Shuffle();

        foreach (PoolObjectType dropType in dropsTypesToCreate)
        {
            GameObject dropObject = ObjectPool.Instance.Get(dropType);
            Drop drop = dropObject.GetComponent<Drop>();

            drop.Column = column;
            drop.Row = row;

            if (!drop.FindMatch(false))
            {
                return drop.gameObject;
            }

            else
            {
                ObjectPool.Instance.ReturnToPool(dropObject);
            }
        }

        return null;
    }
}



