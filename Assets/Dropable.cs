using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dropable : MonoBehaviour, IDropHandler
{
    private GameObject item;
    public void OnDrop(PointerEventData eventData)
    {
        if(!item)
        {
            item = DragableItem.objBeingDraged;
            
            if (transform.childCount > 0) return;
            item.transform.SetParent(transform);
            item.transform.position = transform.position;
           
        }
        var newLayerController = transform.parent.GetComponent<LayerController>();
        item.GetComponent<ItemController>().layerController.UpdateItems();
        newLayerController.UpdateItems();
        item.GetComponent<ItemController>().layerController = newLayerController;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(item!=null && item.transform.parent!=transform)
        {
            item = null;
        }
    }
}
