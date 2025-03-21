using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDataEatable
{
    public EatableType type;
    public float value; // 회복량, 버프량
}
public enum EatableType
{
    Health, // 체력
    Stemina, // 스테미너
    Speed, // 스피드
    Jump, // 점프

}
public enum ItemType
{
    Eatable, // 소비
    Resource, // 자원
    Equip, // 장비
}

[CreateAssetMenu(fileName ="Item", menuName = "ItemData")]
public class ItemSO : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stacking")]
    public bool canStack;
    public int maxStack;

    [Header("Consumable")]
    public ItemDataEatable[] eatableData;
}
