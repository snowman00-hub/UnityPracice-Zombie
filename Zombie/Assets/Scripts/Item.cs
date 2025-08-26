﻿using UnityEngine;

public class Item : MonoBehaviour, IItem
{
    public enum Types
    {
        Coin,
        Ammo,
        Health,
    }

    public Types itemType;
    public int value = 10;

    public void Use(GameObject other)
    {
        switch (itemType)
        {
            case Types.Coin:
            {
                Debug.Log("Coin");
            }
            break;
            case Types.Ammo:
            {
                var shooter = other.GetComponent<PlayerShooter>();
                shooter?.gun?.AddAmmo(value);
            }
            break;
            case Types.Health:
            {
                var playerHealth = other.GetComponent<PlayerHealth>();
                playerHealth?.Heal(value);
            }
            break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tag.Player))
        {
            Use(other.gameObject);
            Destroy(gameObject);
        }
    }
}
