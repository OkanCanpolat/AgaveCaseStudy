using System.Collections;
using UnityEngine;

public class Drop : MonoBehaviour, IPoolObject
{
    [HideInInspector] public int Column;
    [HideInInspector] public int Row;
    [HideInInspector] public int PreviousColumn;
    [HideInInspector] public int PreviousRow;
    [HideInInspector] public bool IsMatched;
    [HideInInspector] public Drop OtherDrop;
    public DropType Type;
    [SerializeField] private PoolObjectType poolObjectType;
    [SerializeField] private DropConfiguration dropConfiguration;
    private Vector2 touchPosition;
    private Vector2 releasePosition;
    private IMatchFinder matchFinder;

    private void Awake()
    {
        matchFinder = GetComponent<IMatchFinder>();
    }
    private void OnMouseDown()
    {
        if (BoardManager.Instance.State == BoardState.CanNotSwipe) return;

        touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    private void OnMouseUp()
    {
        if (BoardManager.Instance.State == BoardState.CanNotSwipe) return;

        releasePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ControlSwipe();
    }
    public bool FindMatch(bool markIsMatched = true)
    {
        return matchFinder.FindMatch(markIsMatched);
    }
    public void HandleSwap()
    {
        StartCoroutine(HandleSwapCo());
    }
    public void OnMatched()
    {
        StartCoroutine(OnMatchedCo());
    }
    public void ScaleUpAndDown(Vector2 targetScale)
    {
        StartCoroutine(ScaleUpAndDownCo(targetScale));
    }
    public void UpdatePosition()
    {
        StartCoroutine(UpdatePositionCo());
    }
    public void RevertPosition()
    {
        StartCoroutine(RevertPositionCo());
    }
    public PoolObjectType GetPoolObjectType()
    {
        return poolObjectType;
    }
    public void OnReturnPool()
    {
        IsMatched = false;
        transform.localScale = dropConfiguration.DefaultScale;
    }
    private IEnumerator HandleSwapCo()
    {
        OtherDrop.UpdatePosition();
        OtherDrop.ScaleUpAndDown(dropConfiguration.DownScale);

        StartCoroutine(ScaleUpAndDownCo(dropConfiguration.UpScale));
        yield return StartCoroutine(UpdatePositionCo());

        if (FindMatch() | OtherDrop.FindMatch())
        {
            BoardManager.Instance.HandleMatchCycle();
        }

        else
        {
            OtherDrop.RevertPosition();
            OtherDrop.ScaleUpAndDown(dropConfiguration.UpScale);
            StartCoroutine(ScaleUpAndDownCo(dropConfiguration.DownScale));
            yield return StartCoroutine(RevertPositionCo());
            BoardManager.Instance.State = BoardState.CanSwipe;
        }
    }
    private IEnumerator UpdatePositionCo()
    {
        float t = 0;

        Vector2 destination = new Vector2(Column, Row);
        Vector2 currentPostion = transform.position;

        while (t < 1)
        {
            transform.position = Vector2.Lerp(currentPostion, destination, t);
            t += Time.deltaTime / dropConfiguration.SwipeTime;
            yield return null;
        }

        transform.position = destination;
    }
    private IEnumerator RevertPositionCo()
    {
        float t = 0;
        Vector2 destination = new Vector2(PreviousColumn, PreviousRow);
        Vector2 currentPostion = transform.position;

        Column = PreviousColumn;
        Row = PreviousRow;

        BoardManager.Instance.SetDropAt(Column, Row, gameObject);

        while (t < 1)
        {
            transform.position = Vector2.Lerp(currentPostion, destination, t);
            t += Time.deltaTime / dropConfiguration.SwipeTime;
            yield return null;
        }

        transform.position = destination;
    }
    private void ControlSwipe()
    {
        if (Vector2.Distance(touchPosition, releasePosition) > dropConfiguration.SwipeOffset)
        {
            Vector2 swipeDirection = SwipeDirectionCalculator.GetSwipeDirection(touchPosition, releasePosition);
            BoardManager.Instance.ControlSwipe(this, swipeDirection);
        }
    }
    private IEnumerator ScaleUpAndDownCo(Vector2 targetScale)
    {
        float t = 0;

        Vector2 firstScale = transform.localScale;

        while (t < 1)
        {
            transform.localScale = Vector2.Lerp(firstScale, targetScale, t);
            t += Time.deltaTime / (dropConfiguration.SwipeTime / 2);
            yield return null;
        }

        Vector2 currentScale = transform.localScale;
        t = 0;

        while (t < 1)
        {
            transform.localScale = Vector2.Lerp(currentScale, firstScale, t);
            t += Time.deltaTime / (dropConfiguration.SwipeTime / 2);
            yield return null;
        }

        transform.localScale = firstScale;
    }
    private IEnumerator OnMatchedCo()
    {
        float t = 0;
        Vector2 currentScale = transform.localScale;

        while (t < 1)
        {
            transform.localScale = Vector2.Lerp(currentScale, dropConfiguration.DownScale, t);
            t += Time.deltaTime / dropConfiguration.DestroyAnimTime;
            yield return null;
        }

        BoardManager.Instance.RemoveDropAt(Column, Row);
        ObjectPool.Instance.ReturnToPool(gameObject);
    }
}
