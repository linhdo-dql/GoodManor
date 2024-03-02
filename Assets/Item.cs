using System;
using UnityEngine;
[Serializable]
public class Item : ScriptableObject
{
    public int id;
    public string name;
    public string type;
    public Sprite sprite;
}