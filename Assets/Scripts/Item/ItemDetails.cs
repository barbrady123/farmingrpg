using System;
using UnityEngine;

[Serializable]
public class ItemDetails
{
    public int ItemCode;

    public ItemType ItemType;

    public string ItemDescription;

    public Sprite ItemSprite;

    public string ItemLongDescription;

    public short ItemUseGridRadius;

    public float ItemUseRadius;

    public bool IsStartingItem;

    public bool CanBePickedUp;

    public bool CanBeDropped;

    public bool CanBeEaten;

    public bool CanBeCarried;
}
