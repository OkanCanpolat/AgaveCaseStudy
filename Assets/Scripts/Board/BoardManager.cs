using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardState
{
    CanSwipe, CanNotSwipe
}
public class BoardManager : Singleton<BoardManager>
{
    [HideInInspector] public BoardState State;

    [Header ("Column and Row")]
    [SerializeField] private BoardColumns columns;
    [SerializeField] private int height;
    private int width;

    [Header("Creation")]
    [SerializeField] private BoardCreatorBase boardCreator;
    [SerializeField] private BoardFillerBase boardFiller;

    private GameObject[,] currentDrops;
    private List<Drop> dropsToControlMatch = new List<Drop>();

    public override void Awake()
    {
        Application.targetFrameRate = 60;
        base.Awake();
        SetWidth();
    }
    private void Start()
    {
        boardCreator.CreateBoard(width, height, out currentDrops);
        State = BoardState.CanSwipe;
    }
    public void ControlSwipe(Drop source, Vector2 swipeDirection)
    {
        int targetColumn = source.Column + (int)swipeDirection.x;
        int targetRow = source.Row + (int)swipeDirection.y;

        if (!IsInsideBoard(targetColumn, targetRow) || IsEmpty(targetColumn, targetRow)) return;

        State = BoardState.CanNotSwipe;
        GameObject target = currentDrops[targetColumn, targetRow];
        Drop targetDrop = target.GetComponent<Drop>();
        SwapDrops(source, targetDrop);
        source.HandleSwap();
    }
    public Drop TryGetDrop(int column, int row)
    {
        Drop result;

        if (!IsInsideBoard(column, row) || IsEmpty(column, row))
        {
            result = null;
        }
        else
        {
            result = currentDrops[column, row].GetComponent<Drop>();
        }

        return result;
    }
    public void SetDropAt(int column, int row, GameObject dropObject)
    {
        if (IsInsideBoard(column, row))
        {
            currentDrops[column, row] = dropObject;
        }
    }
    public void RemoveDropAt(int column, int row)
    {
        currentDrops[column, row] = null;
    }
    public void HandleMatchCycle()
    {
        StartCoroutine(HandleMatchCycleCo());
    }
    private IEnumerator HandleMatchCycleCo()
    {
        dropsToControlMatch.Clear();

        yield return new WaitForSeconds(.3f);
        HandleMatchedDrops();
        yield return new WaitForSeconds(0.2f);
        DecreaseRows();
        yield return new WaitForSeconds(0.3f);
        boardFiller.FillBoard(currentDrops, dropsToControlMatch, columns);
        yield return new WaitForSeconds(0.2f);
        ControlFallenAndNewDrops();
    }
    private void HandleMatchedDrops()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (IsEmpty(i, j)) continue;

                Drop drop = currentDrops[i, j].GetComponent<Drop>();

                if (drop.IsMatched)
                {
                    drop.OnMatched();
                }
            }
        }
    }
    private void DecreaseRows()
    {
        for (int i = 0; i < width; i++)
        {
            int counter = 0;

            for (int j = 0; j < height; j++)
            {
                if (IsEmpty(i, j))
                {
                    counter++;
                }
                else if (counter > 0)
                {
                    Drop drop = currentDrops[i, j].GetComponent<Drop>();
                    drop.Row -= counter;
                    currentDrops[i, j] = null;
                    currentDrops[i, drop.Row] = drop.gameObject;
                    drop.UpdatePosition();
                    dropsToControlMatch.Add(drop);
                }
            }
        }
    }
    private void ControlFallenAndNewDrops()
    {
        if (IsThereMatchIn(dropsToControlMatch))
        {
            HandleMatchCycle();
        }
        else
        {
            State = BoardState.CanSwipe;
        }
    }
    private void SetWidth()
    {
        width = columns.ColumnType.Length;
    }
    private bool IsOutOfRangeColumn(int column)
    {
        return column >= width || column < 0;
    }
    private bool IsOutOfRangeRow(int row)
    {
        return row >= height || row < 0;
    }
    private bool IsInsideBoard(int column, int row)
    {
        return !IsOutOfRangeColumn(column) & !IsOutOfRangeRow(row);
    }
    private bool IsEmpty(int column, int row)
    {
        if (IsInsideBoard(column, row))
        {
            if (currentDrops[column, row] != null)
            {
                return false;
            }
        }

        return true;
    }
    private void SwapDrops(Drop source, Drop target)
    {
        currentDrops[source.Column, source.Row] = target.gameObject;
        currentDrops[target.Column, target.Row] = source.gameObject;

        int sourceColumn = source.Column;
        int sourceRow = source.Row;

        source.PreviousColumn = sourceColumn;
        source.PreviousRow = sourceRow;
        target.PreviousColumn = target.Column;
        target.PreviousRow = target.Row;

        source.Column = target.Column;
        source.Row = target.Row;
        target.Column = sourceColumn;
        target.Row = sourceRow;

        source.OtherDrop = target;
    }
    private bool IsThereMatchIn(List<Drop> drops)
    {
        bool result = false;

        foreach(Drop drop in drops)
        {
            result = drop.FindMatch() | result;
        }

        return result;
    }
}