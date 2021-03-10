using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Navigation : MonoBehaviour
{
    internal bool stopped;

    public abstract Vector3 GetDir();
    public abstract bool PathComplete();
    public abstract bool IsTargetUnreachable();
    public abstract bool IsMoving();
    public abstract void Stop();
    public abstract void ClearPath();
    public abstract void SetTarget(Transform target);
    public abstract void SetTarget(Vector3 target);
    public abstract void SetSpeed(float speed);

}
