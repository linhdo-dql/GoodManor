using System;
using UnityEngine;
using UnityEngine.UI;

internal class ItemController : MonoBehaviour
{
    public Item item;
    public LayerController layerController;

    internal void PopulateData(Item _item, LayerController controller)
    {
        transform.localScale = Vector3.one;
        item = Resources.Load<Item>("ItemDatas/"+_item.id);
        layerController = controller;
        //transform.SetParent(transform.root);
        GetComponent<Image>().sprite = item.sprite;
        //
    }

    internal void TweenHide()
    {
        //Hiệu ứng co lại
        LeanTween.scale(gameObject, new Vector3(0.8f, 0.8f, 0.8f), 0.2f).setEase(LeanTweenType.easeInOutSine);
        // Hiệu ứng phóng to
        LeanTween.scale(gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeInOutSine).setDelay(0.2f).setOnComplete(() =>
        {
            LeanTween.scale(gameObject, Vector3.zero, 0.2f);
        });
    }

    private void Update()
    {
        transform.localScale = Vector3.one;
    }
}