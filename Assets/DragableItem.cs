using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{   
    private RectTransform m_RectTransform;
    private Transform dragArea;

    public void OnBeginDrag(PointerEventData eventData)
    {
        objBeingDraged = gameObject;
       
        startPosition = transform.position;
        startParent = transform.parent;
        transform.SetParent(dragArea);

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_RectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        
        objBeingDraged = null;

        canvasGroup.blocksRaycasts = true;
        if (transform.parent == dragArea)
        {
            transform.position = startPosition;
            transform.SetParent(startParent);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        dragArea = GameObject.FindGameObjectWithTag("DragArea").transform;
        m_RectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public static GameObject objBeingDraged;

    private Vector3 startPosition;
    private Transform startParent;
    private CanvasGroup canvasGroup;

    
}
