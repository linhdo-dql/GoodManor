using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class LevelData : ScriptableObject
{
    public int id;
    public int levelType;
    public int targetId;
    public List<string> layout;
    public List<int> layoutItemCount;
    public List<int> layoutTime;
    public List<int> layoutTargetCount;
    public int difficulty;
    public string items;
    public string rewardType;
    internal int lockBlock;
    public int frezzedBlock;
    public int frezzedCount;

    public LevelData()
    {
        layout = new List<string>();
        layoutItemCount = new List<int>();
        layoutTime = new List<int>();
        layoutTargetCount = new List<int>();
    }
}
