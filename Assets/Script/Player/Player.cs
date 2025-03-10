using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerMovement movement;
    [HideInInspector] public PlayerStat stat;
    [HideInInspector] public ItemSO item;
    public Action addItem;
    public Transform dropPosition;

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        movement = GetComponent<PlayerMovement>();
        stat = GetComponent<PlayerStat>();
    }
}
