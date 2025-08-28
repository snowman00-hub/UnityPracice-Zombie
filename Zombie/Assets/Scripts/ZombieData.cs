﻿using UnityEngine;

[CreateAssetMenu(fileName = "ZombieData", menuName = "Scriptable Objects/ZombieData")]
public class ZombieData : ScriptableObject
{
    public float maxHP = 100f;
    public float damage = 20f;
    public float speed = 2f;

    public Color skinColor = Color.white;
}
