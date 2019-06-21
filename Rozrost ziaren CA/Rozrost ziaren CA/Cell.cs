using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RozrostZiarenCA2
{
    class Cell
    {
        public int value = 0;
        public double dislocationDensity = 0; 
        public bool isRecrystalised = false;
        public bool isAvailable = true;
        public bool isAvailableForMC = true;
        public int gravityX = 0;
        public int gravityY = 0;
        public int id = 0;
        public Cell(int x, int y)
        {
            gravityX = x;
            gravityY = y;
        }
    }
}

