using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Utils;
using static UnityEngine.GraphicsBuffer;

public class BlockController : MonoBehaviour
{
    public Block block;
    public BlockType type;
    public GameObject layerPrefab;

    // Start is called before the first frame update
    public void FillItems()
    {
        switch(type)
        {
            case BlockType.One: FillBlockOneItem(); break;
            case BlockType.Three: FillBlockThreeItem(); break;
        }
    }

    private void FillBlockThreeItem()
    {
        throw new NotImplementedException();
    }

    private void FillBlockOneItem()
    {
        throw new NotImplementedException();
    }

    internal void NextLayer(int layerIndex)
    {
        if (layerIndex - 1 >= 0)
        {
            SetActiveLayer(transform.GetChild(layerIndex - 1).gameObject);
            if (layerIndex - 1 >= 1)
            {
                transform.GetChild(layerIndex - 2).gameObject.GetComponent<LayerController>().SetState(LayerState.Wait);
            }
        }
    }

    public void InitNewLayer(Transform transform)
    {
        var layer = Instantiate(layerPrefab, transform);
        layer.transform.SetAsFirstSibling();
        var layerController = layer.GetComponent<LayerController>();
        layerController.layerID = 999;
    }

    private void SetActiveLayer(GameObject layer)
    {
        var layerController = layer.GetComponent<LayerController>();
        layerController.SetState(LayerState.Showing);
        if(layerController.items.Count > 2)
        {
            layerController.CheckMatch();
        }
    }
}
