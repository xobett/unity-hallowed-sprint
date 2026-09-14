using UnityEngine;

interface IInteractable
{
  void OnInteract();
}

interface IDamageable
{
  void TakeDamage(float damage);
}

interface IRespawnable
{
  Vector3 LastCheckpoint { get; set; }
  void Respawn();
}