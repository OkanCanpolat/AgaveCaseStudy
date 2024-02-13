using UnityEngine;

[CreateAssetMenu (fileName = "Drop Configuration")]
public class DropConfiguration : ScriptableObject
{
    [Header ("Swipe")]
    public float SwipeOffset = 0.4f;
    public float SwipeTime = 0.2f;

    [Header("Animation")]
    public float DestroyAnimTime = 0.1f;
    public Vector3 UpScale;
    public Vector3 DownScale;
    public Vector3 DefaultScale;
}
