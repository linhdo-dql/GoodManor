using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class LevelDesignEditor : EditorWindow
{
    private SerializedObject serializedObject;
    private int layoutSelectedIndex;
    protected LevelData[] levelDatas;
    protected string selectedPropertiesPath;
    public LevelData level;
    private string selectedProperties;
    private string[] levelOptions = {"Collect One Item", "Collect Process Item", "Clear All Layout"};
    private string[] levelStates = {"Active", "Deactive", "Complete", "Faile"};
    static string[,] labels;
    private bool centerLayout = true;
    private bool[] centers = new bool[] {false, false, false, false, false, false, false, false, false};
    private string labelButtonLayout = "Save";
    private int levelType;

    [MenuItem("Window/Level Editor Window")]
    public static void ShowWindow()
    {
        LevelDesignEditor lde = GetWindow<LevelDesignEditor>();
        lde.minSize = new Vector3(1080, 600);
        labels = new string[9, 3];
        lde.Show();
    }
    protected void DrawProperties(LevelData levelData)
    {
        GUIStyle gStyles = new GUIStyle() { alignment = TextAnchor.MiddleLeft, fixedWidth = 150, fontSize = 12, fontStyle = FontStyle.Bold };
        gStyles.normal.textColor = Color.white;
        EditorGUILayout.LabelField("Level ID: " + levelData.id, gStyles);
        levelData.levelType = EditorGUILayout.Popup("Level Type:", levelData.levelType, levelOptions);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Target Item:", gStyles);
        levelData.targetId = EditorGUILayout.IntField(levelData.targetId);
        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField("Layouts: ", gStyles);
        for (int i = 0; i< levelData.layout.Count; i++)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(300), GUILayout.MaxHeight(30));
            if(GUILayout.Button("Layout"+i, GUILayout.MaxWidth(80)))
            {
                SelectedLayout(levelData, i);
            }
            
            GUIStyle white = new GUIStyle(EditorStyles.label);
            white.normal.textColor = Color.white;
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(100), GUILayout.ExpandWidth(false));
            GUILayout.Label("Item:", white);
            levelData.layoutItemCount[i] = EditorGUILayout.IntField(levelData.layoutItemCount[i]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(100), GUILayout.ExpandWidth(false));
            GUILayout.Label("Target:", white);
            levelData.layoutTargetCount[i] = EditorGUILayout.IntField(levelData.layoutTargetCount[i]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(100), GUILayout.ExpandWidth(false));
            GUILayout.Label("Time:", white);
            levelData.layoutTime[i] = EditorGUILayout.IntField(levelData.layoutTime[i]);
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete", GUILayout.MaxWidth(50)))
            {
                DeleteLayout(levelData,i);
            }
            GUILayout.EndHorizontal();
           
            GUILayout.EndVertical();
            

        }

        if (GUILayout.Button("Add New Layout", GUILayout.ExpandWidth(true)))
        {
            AddLayout(levelData);
        }
        GUILayout.Label("Level Difficult:", gStyles);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Lock blocks: ");
        levelData.lockBlock = EditorGUILayout.IntField(levelData.lockBlock);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Frezze block:");
        levelData.frezzedBlock = EditorGUILayout.IntField(levelData.frezzedBlock);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Frezze count:");
        levelData.frezzedCount = EditorGUILayout.IntField(levelData.frezzedCount);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Items:", gStyles);
        levelData.items = EditorGUILayout.TextField(levelData.items);
        GUILayout.EndHorizontal();

    }
    private void SelectedLayout(LevelData levelData,int i)
    {
        layoutSelectedIndex = i;
        string[] lout = new string[9];
        labels = new string[9,3];
        centers = new bool[9];
        var layoutRow = levelData.layout[layoutSelectedIndex].Split(",");
        
        for (int j = 0; j < layoutRow.Length; j++)
        {
           lout[j] = InvertRow(layoutRow[j]);
        }
        Debug.Log(string.Join(':', lout));
        
        for (int k = 0; k< lout.Length; k++)
        {   
            string row = lout[k];
            if (row == "") continue;
            for(int l = 0; l < row?.Length; l++)
            {
                if (row[l] == 'C')
                {
                    centers[k] = true;
                    labels[k, l] = "";
                    continue;
                }
                labels[k, l] = row[l].ToString();
            }
        }
        //Debug.Log(rowLayout);
    }

    private void LoadLayout(int i)
    {
       
    }

    private void AddLayout(LevelData levelData)
    {
        labels = new string[9, 3];
        levelData.layout.Add("");
        levelData.layoutItemCount.Add(0);
        levelData.layoutTargetCount.Add(0);
        levelData.layoutTime.Add(0);
        DrawProperties(levelData);
    }

    private void DeleteLayout(LevelData levelData,int index)
    {
        levelData.layout.RemoveAt(index);
        levelData.layoutItemCount.RemoveAt(index);
        levelData.layoutTargetCount.RemoveAt(index);
        levelData.layoutTime.RemoveAt(index);
        DrawProperties(levelData);
    }

    protected void DrawSlideBar(LevelData[] levelDatas)
    {
        foreach (var lv in levelDatas)
        {
            if(GUILayout.Button(lv.id.ToString()))
            {
                selectedPropertiesPath = lv.id.ToString();
                level = lv;
            }
        }
        if(!string.IsNullOrEmpty(selectedPropertiesPath))
        {
            selectedProperties = selectedPropertiesPath;
        }    
    }
    private void OnGUI()
    {
        levelDatas = GetAllInstances<LevelData>();
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical("box", GUILayout.MaxWidth(130), GUILayout.ExpandHeight(true));
        DrawSlideBar(levelDatas);
        if(GUILayout.Button("Add new level"))
        {
            var maxId = levelDatas.Max(c => c.id) + 1;
            LevelData asset = ScriptableObject.CreateInstance<LevelData>();
            asset = new LevelData();
            asset.id = maxId;
            labels = new string[9, 3];
            asset.layout.Add("");
            asset.layoutItemCount.Add(0);
            asset.layoutTargetCount.Add(0);
            asset.layoutTime.Add(0);
            AssetDatabase.CreateAsset(asset, "Assets/Resources/LevelDatas/" + (maxId+1) + ".asset");
            AssetDatabase.SaveAssets();
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical("box", GUILayout.MaxWidth(400), GUILayout.ExpandHeight(true));
        if(selectedProperties!= null)
        {
            for (int i = 0; i < levelDatas.Length; i++)
            {
                if (levelDatas[i].id.ToString() == selectedProperties)
                {
                    DrawProperties(levelDatas[i]);
                }
            }
        }
        else
        {
            GUILayout.Label("Select a level in list or create new...!");
        }
        GUILayout.EndVertical();
       
        GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        if (selectedProperties != null)
        {
            for (int i = 0; i < 9; i++)
            {
                GUILayout.BeginHorizontal(GUILayout.MaxHeight(600));
                for (int j = 0; j < 3; j++)
                {
                    if (GUILayout.Button(labels[i, j], GUILayout.MaxWidth(200), GUILayout.MaxHeight(154)))
                    {
                        BlockClicked(i, j);
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label("Select a level in list or create new...!");
        }
        
        GUILayout.EndVertical();
        GUILayout.BeginVertical("box", GUILayout.MaxWidth(80), GUILayout.ExpandHeight(true));
        for (int i = 0; i < centers.Length; i++)
        {
            centers[i] = GUILayout.Toggle(centers[i], "Center", GUILayout.MaxWidth(80), GUILayout.MaxHeight(154));
        }
       
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        /*GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save", GUILayout.MaxWidth(100)))
        {

        }
        if (GUILayout.Button("Export", GUILayout.MaxWidth(100)))
        {

        }
        GUILayout.EndHorizontal();*/
    }

    private void SaveOrUpdateLayout()
    {
        throw new NotImplementedException();
    }

    private void BlockClicked(int i, int j)
    {
        string[] lout = new string[9];
        labels[i, j] = labels[i,j] == "X" ? "" : "X";
        for (int x = 0; x < 9; x++)
        {
            string r = "";
            int c = 0;
            for (int y = 0; y < 3; y++)
            {
                if (labels[x,y] == "X")
                {
                    c++;
                } 
                r += labels[x, y] == "X" ? "X" : " ";
            }
            lout[x] = c == 2 ? (centers[x] ? "XXC" : r) : r;
        }
       
        for(int x = 0; x < lout.Length; x++)
        {
            lout[x] = ConvertRow(lout[x]);
        }
        string rowString = String.Join(',', lout);
        level.layout[layoutSelectedIndex] = rowString;
    }
  
    public string ConvertRow(string l)
    {
        string row = "";
        for (int z = 0; z < l.Split("").Length; z++)
        {
            switch (l)
            {
                case "X  ": row = "1-left"; break;
                case " X ": row = "1"; break;
                case "  X": row = "1-right"; break;
                case "XX ": row = "2-left"; break;
                case "X X": row = "2-out"; break;
                case " XX": row = "2-right"; break;
                case "XXC": row = "2"; break;
                case "XXX": row = "3"; break;
                default: row = ""; break;
            }
        }
        return row;
    }
    public string InvertRow(string l)
    {
        string row = "";
        for (int z = 0; z < l.Split("").Length; z++)
        {
            switch (l)
            {
                case "1-left": row = "X  "; break;
                case "1": row = " X "; break;
                case "1-right": row = "  X"; break;
                case "2": row = "XXC"; break;
                case "2-left": row = "XX "; break;
                case "2-out": row = "X X"; break;
                case "2-right": row = " XX"; break;
                case "3": row = "XXX"; break;
                default: row = ""; break;
            }
        }
        return row;
    }
    public static T[] GetAllInstances<T>() where T : LevelData
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a;

    }
    protected void Apply()
    {
        serializedObject.ApplyModifiedProperties();
    }
}
