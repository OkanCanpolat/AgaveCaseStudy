using UnityEngine;

public class BasicMatchFinder : MonoBehaviour, IMatchFinder
{
    public Drop Drop { get ; set ; }
    private void Awake()
    {
        Drop = GetComponent<Drop>();
    }
    public bool FindMatch(bool markIsMatched)
    {
        return FindHorizontalMatch(markIsMatched) | FindVerticalMatch(markIsMatched);
    }
    private bool FindHorizontalMatch(bool markIsMatched)
    {
        bool matched = false;

        Drop right1 = BoardManager.Instance.TryGetDrop(Drop.Column + 1, Drop.Row);
        Drop right2 = BoardManager.Instance.TryGetDrop(Drop.Column + 2, Drop.Row);
        Drop left1 = BoardManager.Instance.TryGetDrop(Drop.Column - 1, Drop.Row);
        Drop left2 = BoardManager.Instance.TryGetDrop(Drop.Column - 2, Drop.Row);

        matched = HandleTripleMatch(right1, left1, markIsMatched) | matched;
        matched = HandleTripleMatch(right1, right2, markIsMatched) | matched;
        matched = HandleTripleMatch(left1, left2, markIsMatched) | matched;

        return matched;
    }
    private bool FindVerticalMatch(bool markIsMatched)
    {
        bool matched = false;

        Drop up1 = BoardManager.Instance.TryGetDrop(Drop.Column, Drop.Row + 1);
        Drop up2 = BoardManager.Instance.TryGetDrop(Drop.Column, Drop.Row + 2);
        Drop down1 = BoardManager.Instance.TryGetDrop(Drop.Column, Drop.Row - 1);
        Drop down2 = BoardManager.Instance.TryGetDrop(Drop.Column, Drop.Row - 2);

        matched = HandleTripleMatch(up1, down1, markIsMatched) | matched;
        matched = HandleTripleMatch(down1, down2, markIsMatched) | matched;
        matched = HandleTripleMatch(up1, up2, markIsMatched) | matched;

        return matched;
    }
    private bool HandleTripleMatch(Drop first, Drop second, bool markIsMatched)
    {
        bool matched = false;

        if (first != null && second != null && first.Type == Drop.Type && second.Type == Drop.Type)
        {
            if (markIsMatched)
            {
                first.IsMatched = true;
                second.IsMatched = true;
                Drop.IsMatched = true;
            }
            matched = true;
        }

        return matched;
    }
}
