using System;
using UnityEngine;
[Serializable]
public class LevelData : ScriptableObject
{
    public int id;
    public string layout;
    public int difficulty;
    public string items;
    public int time;
    public int targetItemId;
    public int targetItemCount;
    public string rewardType;
    public int storyStep;
    public int minItem;
    public int maxItem;
}
