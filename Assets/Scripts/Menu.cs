using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    Set,
    Single,
    Select,
    Fish,
    Fried,
    Drink
}

[Serializable]
public class Menu
{
    public string Name;
    public string Description;
    public int Price;
    public Sprite Image;
    public Type Type;
}
