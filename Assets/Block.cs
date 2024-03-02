using System;
using System.Collections.Generic;
[Serializable]
public class Block
{
    public int NumItems { get; set; }
    public List<Layer> Layers { get; set; }

    public Block(int numberItems)
    {
        NumItems = numberItems;
        Layers = new List<Layer>();
    }
}