using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class Utils
    {
        public enum LayerState
        {
            Wait = 0,
            Showing = 1,
            Clear = 2,
            Hide = 3
        }

        public enum BlockType
        {
            One,
            Three
        }
    }
}
