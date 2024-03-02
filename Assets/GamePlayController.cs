using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GamePlayController : MonoBehaviour
{

    public static GamePlayController Instance;
    [SerializeField]
    private Transform layout;
    [SerializeField]
    private LevelData levelData;
    [SerializeField]
    private GameObject rowPrefab;
    [SerializeField]
    private GameObject blockPrefab;
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private GameObject layerPrefab;
    private int numberItem;
    private List<GameObject> blocks;
    private int[] listItems;
    private List<Block> blockDatas;

    private void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        blocks = new List<GameObject>();
        
        numberItem = GetRandomNumberItem(levelData.minItem, levelData.maxItem);
        InitLayout();
        //Random Number Item 
        GenerateElements();
        
    }
    //Test
    private int GetRandomNumberItem(int min, int max)
    {
        int randomNumber = min;
        do
        {
            randomNumber = Random.Range(min, max);
        }
        while (randomNumber % 3 != 0);
        return randomNumber;
    }
    //
    private void CaculatorFill(int blockCount)
    {
        //Phân phối Item Id ListItem
        listItems = new int[numberItem];
        var itemIds = levelData.items.Split(',');
        //print(itemIds.Length);
        var itemIndex = 0;
        for (int i = 0; i < numberItem; i++)
        {
            listItems[i] = int.Parse(itemIds[itemIndex]);
            if ((i+1)%3==0)
            {
                itemIndex = itemIndex + 1 < itemIds.Length ? itemIndex += 1 : 0;
            }
            
        }
        listItems.Shuffle();
        List<int> allItemIds = listItems.ToList();
        blockDatas = new List<Block>();
        int cloneNumberItem = numberItem;
        int[] numberItemsBeforeSuffle = new int[blockCount];
        //Phân phối số lượng ngẫu nhiên cho các block sao cho mỗi block có ít nhất 1 item
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
        //Số lượng layer tối đa có thể = số lượng Item trên 1 block => Mỗi layer có 1 Item
        var tmpCount = 0;
        foreach (var block in blockDatas)
        {
            
            var cloneItemCounts = block.NumItems;
            for (int j = 0; j < block.NumItems; j++)
            { 
                if(j == 0)
                {

                    int initLayerItemCount;
                    float chanceForThree = 0f; // Xác suất ra số 1 là 20%

                    if (Random.value < chanceForThree)
                    {
                        initLayerItemCount = 3;
                    }
                    else
                    {
                        // Ngẫu nhiên giữa số 2 và số 3
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
        int blockCount = 0;
        var splitRow = levelData.layout.Split(',');
        foreach (var rl in splitRow)
        {   
            if(rl.Contains("out"))
            {
                Instantiate(rowPrefab, layout);
            }
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
    public List<LevelData> listLevel;
    private int tmpCount = 0;

    private void GenerateElements()
    {
        layerGenerated = 0;
        for(int i = 0; i < blockDatas.Count; i++)
        {
            GenerateLayers(blockDatas[i], blocks[i].transform);
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

    public void NextLevel()
    {
        ClearLayout();
        StopAllCoroutines();
        LeanTween.cancelAll();
        LeanTween.reset();
        tmpCount += 1;
        levelData = listLevel[tmpCount];
        blocks = new List<GameObject>();
        blocks.Clear();
        numberItem = GetRandomNumberItem(levelData.minItem, levelData.maxItem);
        InitLayout();
        //Random Number Item 
        GenerateElements();
    }

    internal bool CheckWin()
    {
        return GameObject.FindGameObjectsWithTag("Item").Length < 1;
    }

    private void Update()
    {
        if(CheckWin())
        {
            SetWin();
        }
    }

    public void SetWin()
    {
        NextLevel();   
    }
}

