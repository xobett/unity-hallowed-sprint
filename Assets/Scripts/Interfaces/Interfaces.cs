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
  Transform LastCheckpoint { get; set; }
  void Respawn();
}