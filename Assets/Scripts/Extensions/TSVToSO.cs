using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class TSVToSO
{
    private static string levelCSVPath = "/Resources/TSVs/level.tsv";
    private static string itemCSVPath = "/Resources/TSVs/itemData.tsv";
    [MenuItem("Tools/Generate Levels")]
    public static void GenerateLevels()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + levelCSVPath);
        foreach(string line in allLines.Skip(1))
        {
            string[] splitData = line.Split('\t');
            LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
            levelData.id = int.Parse(splitData[0]);
            levelData.layout = splitData[1].Split(";").ToList();
            levelData.difficulty = int.Parse(splitData[2]);
            levelData.items = splitData[3];
            levelData.rewardType = splitData[7];
            AssetDatabase.CreateAsset(levelData, $"Assets/Resources/LevelDatas/{levelData.id}.asset");
        }
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/Generate Items")]
    public static void GenerateItems()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + itemCSVPath);
        foreach (string line in allLines.Skip(1))
        {
            string[] splitData = line.Split('\t');
            Item itemData = ScriptableObject.CreateInstance<Item>();
            itemData.id = int.Parse(splitData[0]);
            itemData.name = splitData[1];
            itemData.type = splitData[2];
            itemData.sprite = Resources.Load<Sprite>("Sprites/Items/" +splitData[1]);
            AssetDatabase.CreateAsset(itemData, $"Assets/Resources/ItemDatas/{itemData.id}.asset");
        }
        AssetDatabase.SaveAssets();
    }
}
#endif