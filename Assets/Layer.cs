using System;
using System.Collections.Generic;
using static Assets.Utils;
[Serializable]
public class Layer
{
    public int NumElements { get; set; }

    public Layer(int numElements)
    {
        items = new List<Item>();
        NumElements = numElements;
    }
    public int layerId { get; set; }
    public List<Item> items { get; set; }
    public LayerState layerState { get; set; }
}