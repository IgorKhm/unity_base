using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.15f;

    // Optional clamp (set min < max to enable)
    public bool clamp = false;
    public Vector2 minXY;
    public Vector2 maxXY;

    private Vector3 velocity;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(target.position.x, target.position.y, transform.position.z);

        Vector3 pos = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);

        if (clamp)
        {
            pos.x = Mathf.Clamp(pos.x, minXY.x, maxXY.x);
            pos.y = Mathf.Clamp(pos.y, minXY.y, maxXY.y);
        }

        transform.position = pos;
    }
}