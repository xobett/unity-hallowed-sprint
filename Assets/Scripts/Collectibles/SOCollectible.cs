using System;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "New Collectible", menuName = "Scriptable Objects/Collectible")]
public class SOCollectible : ScriptableObject
{
    public string Name;
    public bool IsRespawnable;
}
