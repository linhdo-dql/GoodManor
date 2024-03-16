using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GamePlayController : MonoBehaviour
{

    public static GamePlayController Instance;
    [SerializeField]
    private Transform layout;
    public LevelData levelData;
    [SerializeField]
    private GameObject rowPrefab;
    [SerializeField]
    private GameObject blockPrefab;
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private GameObject layerPrefab;
    [SerializeField]
    private GameObject targetGameObject;
    [SerializeField]
    private Image targetImage;
    [SerializeField]
    private Image targetProgressImage;
    [SerializeField]
    private TextMeshProUGUI progressText;
    public float progressValue = 0;
    private float progressMaxValue;
    private int numberItem;
    private List<GameObject> blocks;
    private int[] listItems;
    private List<Block> blockDatas;
    private int currentLayout = 0;
    protected LevelData[] levelDatas;
    
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        blocks = new List<GameObject>();
        levelDatas =  Resources.LoadAll<LevelData>("LevelDatas");
        levelDatas.OrderBy(l => l.id);
        //numberItem = GetRandomNumberItem(levelData.minItem, levelData.maxItem);
        InitLayout();
        //Random Number Item 
        GenerateElements();
    }

    void DisplayTime(float timeToDisplay)
    {   
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        targetTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateProgress(int value)
    {
        
        progressValue += value;
        var newValue = progressValue / progressMaxValue;

        progressText.text = progressValue + "/" + progressMaxValue;
        LeanTween.value(oldValue, newValue, 0.5f).setOnUpdate((float f) =>
        {
            targetProgressImage.fillAmount = f;
        }).setEaseInOutQuad().setOnComplete(() =>
        {
            oldValue = newValue;
            if (levelData.layout.Count > 1 && currentLayout == levelData.layout.Count - 1 && progressValue == progressMaxValue)
            {
                SetWin();
            }
        });
    }
    private void CaculatorFill(int blockCount)
    {
        numberItem = levelData.layoutItemCount[currentLayout];
        listItems = new int[numberItem];
        var itemIds = levelData.items.Split(',');
        var itemIndex = 0;
        for (int i = 0; i < numberItem; i++)
        {
            listItems[i] = int.Parse(itemIds[itemIndex]);
            if ((i+1)%3==0)
            {
                itemIndex = itemIndex + 1 < itemIds.Length ? itemIndex += 1 : 0;
            }
        }
        var targetCount = levelData.layoutTargetCount[currentLayout];
        //Add Target
        if (targetCount != 0)
        {
            numberItem += levelData.layoutTargetCount[currentLayout];
            targetImage.sprite = Resources.Load<Sprite>("Sprites/Items/Item_" + levelData.targetId);
            int[] itemTarget = new int[targetCount];
            for (int i = 0; i < itemTarget.Length; i++)
            {
                itemTarget[i] = levelData.targetId;
            }
            listItems = listItems.Concat(itemTarget).ToArray();
        }

        listItems.Shuffle();
        List<int> allItemIds = listItems.ToList();
        blockDatas = new List<Block>();
        int cloneNumberItem = numberItem;
        int[] numberItemsBeforeSuffle = new int[blockCount];
        for(int i = 0; i < numberItemsBeforeSuffle.Length; i++)
        {
            if(i < blockCount-1)
            { 
                int value = numberItem / numberItemsBeforeSuffle.Length;
                var randomNumberItem = Random.Range(value - 2, value + 2);
                cloneNumberItem -= randomNumberItem;
                numberItemsBeforeSuffle[i] = randomNumberItem;
            }
            else
            {
                numberItemsBeforeSuffle[i] = cloneNumberItem;
            }
        }
        numberItemsBeforeSuffle.Shuffle();
        for (int i =0; i< numberItemsBeforeSuffle.Length; i++)
        {
            Block block = new Block(numberItemsBeforeSuffle[i]);
            blockDatas.Add(block);
        }
        var tmpCount = 0;
        foreach (var block in blockDatas)
        {
            var cloneItemCounts = block.NumItems;
            for (int j = 0; j < block.NumItems; j++)
            {
                if (j == 0)
                {
                    int initLayerItemCount;
                    float chanceForThree = 0f;
                    if (Random.value < chanceForThree)
                    {
                        initLayerItemCount = 3;
                    }
                    else
                    {
                        initLayerItemCount = Random.Range(1, 3);
                    }
                    Layer newLayer;
                    if(cloneItemCounts > initLayerItemCount)
                    {
                        newLayer = new Layer(initLayerItemCount);
                        for (int i = 0; i < initLayerItemCount; i++)
                        {
                            var randomIndex = Random.Range(0, allItemIds.Count);
                            Item item = new Item();
                            item.id = allItemIds[randomIndex];
                            newLayer.items.Add(item);
                            allItemIds.RemoveAt(randomIndex);
                        }
                        cloneItemCounts -= initLayerItemCount;
                        tmpCount += initLayerItemCount;
                        block.Layers.Add(newLayer);
                        continue;
                    }
                    else
                    {
                        newLayer = new Layer(cloneItemCounts);
                        for (int i = 0; i < cloneItemCounts; i++)
                        {
                            var randomIndex = Random.Range(0, allItemIds.Count);
                            Item item = new Item();
                            item.id = allItemIds[randomIndex];
                            newLayer.items.Add(item);
                            allItemIds.RemoveAt(randomIndex);
                        }
                        tmpCount += cloneItemCounts;
                        block.Layers.Add(newLayer);
                        break;
                    }
                   
                    
                }
                int randomItemCountInLayer;
                float chanceForOne = levelData.id < 3 ? 0 : 0.05f; // Xác suất ra số 1 là 20%

                if (Random.value < chanceForOne)
                {
                    randomItemCountInLayer = 3;
                }
                else
                {
                    // Ngẫu nhiên giữa số 2 và số 3
                    randomItemCountInLayer = Random.Range(1, 3);
                }
                Layer layer;
               
                if (cloneItemCounts - 1 > randomItemCountInLayer)
                {
                    cloneItemCounts -= randomItemCountInLayer;
                    tmpCount += randomItemCountInLayer;
                    layer = new Layer(randomItemCountInLayer);
                    for(int i = 0; i<randomItemCountInLayer; i++)
                    {
                        var randomIndex = Random.Range(0, allItemIds.Count);
                        Item item = new Item();
                        item.id = allItemIds[randomIndex];
                        layer.items.Add(item);
                        allItemIds.RemoveAt(randomIndex);
                    }
                    if (layer.items.Count > 0)
                    {
                        block.Layers.Add(layer);
                    }
                }
                else
                {
                    layer = new Layer(cloneItemCounts);
                    for (int i = 0; i < cloneItemCounts; i++)
                    {
                        var randomIndex = Random.Range(0, allItemIds.Count);
                        Item item = new Item();
                        item.id = allItemIds[randomIndex];
                        layer.items.Add(item);
                        allItemIds.RemoveAt(randomIndex);
                    }
                    if (layer.items.Count > 0)
                    {
                        block.Layers.Add(layer);
                    }
                    break;
                }
                
            }
        }
        
    }

    public string printArray(int[] arr)
    {
        string ar = "";
        for(int i = 0; i<arr.Length; i++)
        {
            ar += arr[i].ToString() + ",";
        }
        return ar;
    }
    private void InitLayout()
    {
        ClearLayout();
        targetGameObject.SetActive(levelData.layoutTargetCount[currentLayout] != 0);
        oldValue = 0;
        progressValue = 0;
        targetProgressImage.fillAmount = 0;
        progressMaxValue = levelData.layoutTargetCount[currentLayout];
        progressText.text = progressValue + "/" + progressMaxValue;
        timeRemaining = levelData.layoutTime[currentLayout];
        timerIsRunning = true;
        int blockCount = 0;
        var splitRow = levelData.layout[currentLayout].Split(',');
        splitRow = splitRow.Where(s => s != "").ToArray();
        // Remove empty strings from the end
        splitRow = splitRow.Reverse().Where(s => s != "").Reverse().ToArray();
        if (splitRow.Length > 8)
        {
            var layoutRectPos = layout.GetComponent<RectTransform>().localPosition;
            layout.GetComponent<RectTransform>().localPosition = layoutRectPos - new Vector3(0, 154, 0);
        }
        foreach (var rl in splitRow)
        {   
            var row = Instantiate(rowPrefab, layout);
            var rowHorizontalLayout = row.GetComponent<HorizontalLayoutGroup>();
            AlignmentRowLayout(rl, rowHorizontalLayout);
            var rowItemCount = GetNumbersFromString(rl);
            blockCount += rowItemCount;
            InitBlocks(rowItemCount, row);
        }
        CaculatorFill(blockCount);
    }

    private void InitBlocks(int ric, GameObject row)
    {
        for(int i = 0; i < ric; i++)
        {
            var block = Instantiate(blockPrefab, row.transform);
           
            blocks.Add(block);
        }
    }

    private void AlignmentRowLayout(string rl, HorizontalLayoutGroup rowHorizontalLayout)
    {
        if(rl.Contains("left"))
        {
            rowHorizontalLayout.childAlignment = TextAnchor.MiddleLeft;
        }
        else if(rl.Contains("right"))
        {
            rowHorizontalLayout.childAlignment = TextAnchor.MiddleRight;
        }
        else if (rl.Contains("out"))
        {
            rowHorizontalLayout.childAlignment = TextAnchor.MiddleCenter;
            rowHorizontalLayout.spacing = 256;
        }
        else
        {
            rowHorizontalLayout.childAlignment = TextAnchor.MiddleCenter;
        }
    }

    private void ClearLayout()
    {
        foreach(Transform g in layout)
        {
            Destroy(g.gameObject);
        }
    }

    public static int GetNumbersFromString(string text)
    {
        foreach (char c in text)
        {
            if (char.IsDigit(c))
            {
                return int.Parse(c.ToString());
            }
        }
        return 0;
    }
    private int layerGenerated;
    private bool isCompleted = false;
    private bool timerIsRunning;
    private float timeRemaining;
    [SerializeField] private TextMeshProUGUI targetTimeText;
    private float oldValue = 0;
    [SerializeField] private GameObject losePopup;
    [SerializeField] private GameObject victoryPopup;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject testLayout;

    public bool IsOnPlay = false;

    private void GenerateElements()
    {
        layerGenerated = 0;
        for(int i = 0; i < blockDatas.Count; i++)
        {
            GenerateLayers(blockDatas[i], blocks[i].transform);
        }
        foreach (var bl in blocks)
        {
            var layerClone = Instantiate(layerPrefab, bl.transform);
            layerClone.transform.SetAsFirstSibling();
            var layerController = layerClone.GetComponent<LayerController>();
            layerController.layerID = 999;
        }
    }

    private void GenerateLayers(Block block, Transform blockTransform)
    {
       for(int i = 0; i<block.Layers.Count; i++)
       {
            block.Layers[i].layerId = layerGenerated;
            layerGenerated++;
            var layer = Instantiate(layerPrefab, blockTransform);
            layer.transform.SetAsFirstSibling();
            var layerController = layer.GetComponent<LayerController>();
            layerController.PopulateData(block.Layers[i], blockTransform);
            layerController.SetState(i == 0 ? Utils.LayerState.Showing : (i>1 ? Utils.LayerState.Hide : Utils.LayerState.Wait));
       }
      
    }

    public void NextLevel(int id)
    {
        IsOnPlay = true;
        ClearLayout();
        currentLayout = 0;
        StopAllCoroutines();
        LeanTween.cancelAll();
        LeanTween.reset();
        blocks = new List<GameObject>();
        blocks.Clear();
        if (id == levelDatas.Count())
        {
            targetGameObject.SetActive(false);
            testLayout.SetActive(true);
            IsOnPlay = false;
            levelData = levelDatas[0];
            timeRemaining = 0;
            return;
        }

        levelData = levelDatas[id];
        InitLayout();
        //Random Number Item 
        GenerateElements();
        isCompleted = false;
    }

    internal bool CheckWin()
    {
        return GameObject.FindGameObjectsWithTag("Item").Length < 1;
    }

    private void Update()
    {
        if (IsOnPlay)
        {
            if (CheckWin() && !isCompleted)
            {
                SetWin();
            }

            if (timerIsRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    DisplayTime(timeRemaining);
                }
                else
                {
                    ShowLosePopup();
                    timeRemaining = 0;
                    timerIsRunning = false;
                }
            }
        }
       
    }

    private void ShowLosePopup()
    {
        UIController.instance.CloseAllPopup();
        losePopup.SetActive(true);
        losePopup.GetComponent<LosePopupController>().Show(progressValue, progressMaxValue, levelData.targetId);
    }

    public void SetWin()
    {
        timerIsRunning = false;
        isCompleted = true;
        currentLayout++; 
        if (currentLayout < levelData.layout.Count)
        {
            NextLayout();
        }
        else
        {
            ShowPopupWin();
        } 
    }

    private void ShowPopupWin()
    {
        UIController.instance.CloseAllPopup();
        victoryPopup.SetActive(true);
        victoryPopup.GetComponent<WinPopupController>().Show(levelData.targetId);
    }

    private void NextLayout()
    {
        StopAllCoroutines();
        LeanTween.cancelAll();
        LeanTween.reset();
        blocks = new List<GameObject>();
        blocks.Clear();
        InitLayout();
        //Random Number Item 
        GenerateElements();
        isCompleted = false;
    }
    internal void Replay()
    {
        IsOnPlay = true;
        NextLevel(levelData.id - 1 > 0 ? levelData.id - 1 : 0);
    }

    public void Pause()
    {
        UIController.instance.CloseAllPopup();
        timerIsRunning = false;
        pauseUI.SetActive(true);
    }

    public void Continue()
    {
        IsOnPlay = true;
        timerIsRunning = true;
    }

    public void GobackHome()
    {
        UIController.instance.BackToHome();
    }
}

