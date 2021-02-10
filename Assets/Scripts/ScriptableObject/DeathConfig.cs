
using UnityEngine;

[CreateAssetMenu(fileName = "DeathConfig", menuName = "ScriptableObjects/DeathConfg", order = 1)]
public class DeathConfig : ScriptableObject
{
    public DamageType deathType;
    public GameObject effectPrefab;
    public bool destroyBody;
}