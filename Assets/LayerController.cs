using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Utils;
using static UnityEngine.GraphicsBuffer;

internal class LayerController : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    private LayerState _state;
    public LayerState State
    {
        get { return _state; }
        set { _state = value; UpdateState(value); }
    }
    //[HideInInspector]
    public List<GameObject> items;
    public int layerID;

    private void UpdateState(LayerState value)
    {
        switch (value)
        {
            case LayerState.Showing: SetShowingState(); break;
            case LayerState.Wait: SetWattingState(); break;
            case LayerState.Clear: SetClearState(); break;
            case LayerState.Hide: SetHideState(); break;
        }
    }

    private void SetHideState()
    {
        SetItemClickable(false, true);
    }

    private void SetClearState()
    {
        var blockController = transform.parent.parent.GetComponent<BlockController>();
        blockController.NextLayer(layerID+1);
    }

    private void SetWattingState()
    {
        SetItemClickable(false, false);
    }

    private void SetShowingState()
    {
        SetItemClickable(true, false);
    }

    public void SetItemClickable(bool value, bool isHide)
    {
        foreach(var i in items)
        {
            var itemImage = i.GetComponent<Image>();
            itemImage.raycastTarget = value;
            if(value)
            {
                itemImage.color = new Color(255, 255, 255, 1);
            }
            else
            {
                itemImage.color = new Color(0, 0, 0, (isHide ? 0 : 0.5f));
            }
        }
    }
    private void Update()
    {
        
    }
    private Transform block;
    public void PopulateData(Layer layer, Transform _block)
    {   
        //set Id panel
        block = _block;
        items = new List<GameObject> ();
        layerID = layer.layerId;
        if (layer.items.Count > 1)
        {
            for (int i = 0; i < layer.items.Count; i++)
            {
                GameObject item = i switch
                {
                    0 => Instantiate(itemPrefab, transform.GetChild(1)),
                    1 => Instantiate(itemPrefab, transform.GetChild(0)),
                    2 => Instantiate(itemPrefab, transform.GetChild(2)),
                    _ => Instantiate(itemPrefab, transform.GetChild(2)),
                };

                ItemController itemController = item.GetComponent<ItemController>();
                itemController.PopulateData(layer.items[i], this);
                items.Add(item);
            }
        } 
        else
        {
            var item = Instantiate(itemPrefab, transform.GetChild(1));
            var itemController = item.GetComponent<ItemController>();
            itemController.PopulateData(layer.items[0], this);
            items.Add(item);
            
        }
    }

    public void SetState(LayerState layerState)
    {
        State = layerState;
    }

    public void NextLayer()
    {
        if (layerID == 999)
        {
            foreach (var item in items)
            {
                Destroy(item);
            }
            items.Clear();

            return;
        }
        Destroy(gameObject);
        var blockController = block.GetComponent<BlockController>();
        if (block.transform.childCount > 0)
        {
            blockController.NextLayer(transform.GetSiblingIndex());
        }
        else
        {
            blockController.InitNewLayer(block.transform);
        }
    }

    private IEnumerator HideItems()
    {

        LeanTween.reset();
        if (items[0].GetComponent<ItemController>().item.id == GamePlayController.Instance.levelData.targetId)
        {
            GamePlayController.Instance.UpdateProgress(3);
        }
        foreach(var item in items)
        {
            item.GetComponent<ItemController>().TweenHide();
        }
        yield return new WaitForSeconds(0.6f);
        
        NextLayer();
    }
    

    internal void CheckMatch()
    {
        var matchFull = true;
        var id = items[0].GetComponent<ItemController>().item.name;
       
        foreach(var item in items)
        {
            if(item.GetComponent<ItemController>().item.name != id)
            {
                matchFull = false; return;
            }  
        }
        if(matchFull)
        {
            StartCoroutine(HideItems());
        }
    }

    internal void UpdateItems()
    {
        items.Clear();
        List<GameObject> itemTerms = new List<GameObject>();
        foreach(var item in transform.GetComponentsInChildren<ItemController>())
        {
            itemTerms.Add(item.gameObject);
        }
        items.AddRange(itemTerms);
        if (items.Count < 1)
        {
            NextLayer();
        }
        if (items.Count > 2)
        {
            CheckMatch();
        }
       
    }
}