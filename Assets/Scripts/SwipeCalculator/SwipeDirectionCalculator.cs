using UnityEngine;

public static class SwipeDirectionCalculator 
{
    public static Vector2 GetSwipeDirection(Vector2 first, Vector2 second)
    {
        float swipeAngle = CalculateTangent(first, second);
        Vector2 direction = GetTangentResult(swipeAngle);
        return direction;
    }
    private  static float CalculateTangent(Vector2 first, Vector2 second)
    {
        float swipeAngle = Mathf.Atan2(second.y - first.y, second.x - first.x);
        swipeAngle *= Mathf.Rad2Deg;
        return swipeAngle;
    }
    private static Vector2 GetTangentResult(float swipeAngle)
    {
        if (swipeAngle > -45 && swipeAngle <= 45)
        {
            return Vector2.right;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135)
        {
            return Vector2.up;
        }
        else if (swipeAngle > 135 || swipeAngle <= -135)
        {
            return Vector2.left;
        }
        else
        {
            return Vector2.down;
        }
    }
}
