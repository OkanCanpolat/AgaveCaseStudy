using System;

public enum BoardColumnType
{
    Spawner, NotSpawner
}

[Serializable]
public class BoardColumns 
{
    public BoardColumnType[] ColumnType;
}
