using UnityEngine;

[System.Serializable]
public struct DoorSettings
{
    public Collider2D doorCollider;
    public Transform door;
    public Vector3 openPositionOffset;
}

