using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RozrostZiarenCA2
{
    class Neighborhood
    {
        private List<int> listOfValues = new List<int>();
        private List<int> counterOfValues = new List<int>();
        private void addValue(int value)
        {
            if (listOfValues.Count == 0)
            {
                listOfValues.Add(value);
                counterOfValues.Add(1);
            }
            else
            {
                bool add = true;
                for (int i=0; i<listOfValues.Count; i++)
                {
                    if(value == listOfValues[i])
                    {
                        counterOfValues[i]++;
                        add = false;
                        break;
                    }
                }
                if(add)
                {
                    listOfValues.Add(value);
                    counterOfValues.Add(1);
                }
            }
        }
        private int setValue()
        {
            int value=0;
            if (listOfValues.Count != 0)
            {
                for (int i = 1; i < listOfValues.Count; i++)
                {
                    if (counterOfValues[value] < counterOfValues[i])
                        value = i;
                }
                value = listOfValues[value];
            }
            return value;
        }

        public int radiusType(int r, int cellSize, Cell[,] grid)
        {
            int result = 0;
            listOfValues = new List<int>();
            counterOfValues = new List<int>();
            if (grid[r, r].value != 0)
                result = grid[r, r].value;
            else
            {
                int centerX = 0, centerY = 0, centerXX = 0, centerYY = 0;
                centerX = grid[r, r].gravityX;
                centerY = grid[r, r].gravityY;
                int radius = r * cellSize;
                for (int i = -r; i <= r; i++)
                {
                    for (int j = -r; j <= r; j++)
                    {
                        centerXX = i * cellSize + grid[i + r, j + r].gravityX;
                        centerYY = j * cellSize + grid[i + r, j + r].gravityY;
                        double distance = 0;
                        distance = Math.Sqrt((centerX - centerXX) * (centerX - centerXX) + (centerY - centerYY) * (centerY - centerYY));
                        if ((distance <= radius) & (grid[i + r, j + r].value != 0))
                            addValue(grid[i + r, j + r].value);
                    }
                }
                result = setValue();
            }           
            return result;
        }
        public int vonNeumanType(Cell[,] grid)
        {
            int result = 0;
            listOfValues = new List<int>();
            counterOfValues = new List<int>();
            if (grid[1, 1].value != 0)
                result = grid[1, 1].value;
            else
            {
                if (grid[1, 2].value != 0)
                    addValue(grid[1, 2].value);
                if (grid[2, 1].value != 0)
                    addValue(grid[2, 1].value);
                if (grid[1, 0].value != 0)
                    addValue(grid[1, 0].value);
                if (grid[0, 1].value != 0)
                    addValue(grid[0, 1].value);

                result = setValue();
            }
            return result;
        }
        public int mooreType(Cell[,] grid)
        {
            int result = 0;
            listOfValues = new List<int>();
            counterOfValues = new List<int>();
            if (grid[1, 1].value != 0)
                result = grid[1, 1].value;
            else
            {
                for(int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (grid[i, j].value != 0)
                            addValue(grid[i, j].value);
                    }
                }
                result = setValue();
            }
            return result;
        }
        public int pentagonalType(Cell[,] grid, int type)
        {
            int result = 0;
            listOfValues = new List<int>();
            counterOfValues = new List<int>();
            if (grid[1, 1].value != 0)
                result = grid[1, 1].value;
            else
            {
                switch (type)
                {
                    case 0:
                        for (int i = 0; i < 2; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (grid[i, j].value != 0)
                                    addValue(grid[i, j].value);
                            }
                        }
                        break;
                    case 1:
                        for (int i = 1; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (grid[i, j].value != 0)
                                    addValue(grid[i, j].value);
                            }
                        }
                        break;
                    case 2:
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                if (grid[i, j].value != 0)
                                    addValue(grid[i, j].value);
                            }
                        }
                        break;
                    case 3:
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 1; j < 3; j++)
                            {
                                if (grid[i, j].value != 0)
                                    addValue(grid[i, j].value);
                            }
                        }
                        break;
                }
                result = setValue();
            }
            return result;
        }
        public int hexagonalType(Cell[,] grid, int type)
        {
            int result = 0;
            listOfValues = new List<int>();
            counterOfValues = new List<int>();
            if (grid[1, 1].value != 0)
                result = grid[1, 1].value;
            else
            {
                int ignoredX1 = 1, ignoredX2 = 1, ignoredY1 = 1, ignoredY2 = 1;
                Random rand = new Random();
                switch (type)
                {
                    case 0:
                        ignoredX1 = 0;
                        ignoredX2 = 2;
                        ignoredY1 = 0;
                        ignoredY2 = 2;
                        break;
                    case 1:
                        ignoredX1 = 2;
                        ignoredX2 = 0;
                        ignoredY1 = 0;
                        ignoredY2 = 2;
                        break;
                    case 2:
                        bool ok = false;
                        while(!ok)
                        {
                            ignoredX1 = rand.Next(3);
                            ignoredY1 = rand.Next(3);
                            if (!(ignoredX1 == 1 & ignoredY1 == 1))
                                ok = true;
                        }
                        ok = false;
                        while (!ok)
                        {
                            ignoredX2 = rand.Next(3);
                            ignoredY2 = rand.Next(3);
                            if ((!(ignoredX2 == 1 & ignoredY2 == 1)) & (!(ignoredX2 == ignoredX1 & ignoredY2 == ignoredY1)))
                                ok = true;
                        }
                        break;
                }
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if((!((i == ignoredX1 & j ==ignoredY1) | (i == ignoredX2 & j == ignoredY2))) & (grid[i, j].value != 0))
                            addValue(grid[i, j].value);
                    }
                }
                result = setValue();
            }
            return result;
        }

        public int radiusMCEnergy(int r, int cellSize, Cell[,] grid)
        {
            int result = 0;
            int centerX = 0, centerY = 0, centerXX = 0, centerYY = 0;
            centerX = grid[r, r].gravityX;
            centerY = grid[r, r].gravityY;
            int radius = r * cellSize;
            for (int i = -r; i <= r; i++)
            {
                for (int j = -r; j <= r; j++)
                {
                    centerXX = i * cellSize + grid[i + r, j + r].gravityX;
                    centerYY = j * cellSize + grid[i + r, j + r].gravityY;
                    double distance = 0;
                    distance = Math.Sqrt((centerX - centerXX) * (centerX - centerXX) + (centerY - centerYY) * (centerY - centerYY));
                    if ((distance <= radius) & (grid[i + r, j + r].value != grid[r, r].value))
                        result++;
                    }
            }
            return result;
        }
        public int vonNeumanMCEnergy(Cell[,] grid)
        {
            int result = 0;

            if (grid[1, 2].value != grid[1, 1].value)
                result++;
            if (grid[2, 1].value != grid[1, 1].value)
                result++;
            if (grid[1, 0].value != grid[1, 1].value)
                result++;
            if (grid[0, 1].value != grid[1, 1].value)
                result++;
            return result;
        }
        public int mooreMCEnergy(Cell[,] grid)
        {
            int result = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (grid[i, j].value != grid[1, 1].value)
                        result++;
                }
            }   
            return result;
        }
        public int pentagonalMCEnergy(Cell[,] grid, int type)
        {
            int result = 0;
            switch (type)
            {
                case 0:
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (grid[i, j].value != grid[1, 1].value)
                                result++;
                        }
                    }
                    break;
                case 1:
                    for (int i = 1; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (grid[i, j].value != grid[1, 1].value)
                                result++;
                        }
                    }
                    break;
                case 2:
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            if (grid[i, j].value != grid[1, 1].value)
                                result++;
                        }
                    }
                    break;
                case 3:
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 1; j < 3; j++)
                        {
                            if (grid[i, j].value != grid[1, 1].value)
                                result++;
                        }
                    }
                    break;
            }
            return result;
        }
        public int hexagonalMCEnergy(Cell[,] grid, int type)
        {
            int result = 0;
            int ignoredX1 = 1, ignoredX2 = 1, ignoredY1 = 1, ignoredY2 = 1;
            Random rand = new Random();
            switch (type)
            {
                case 0:
                    ignoredX1 = 0;
                    ignoredX2 = 2;
                    ignoredY1 = 0;
                    ignoredY2 = 2;
                    break;
                case 1:
                    ignoredX1 = 2;
                    ignoredX2 = 0;
                    ignoredY1 = 0;
                    ignoredY2 = 2;
                    break;
                case 2:
                    bool ok = false;
                    while (!ok)
                    {
                        ignoredX1 = rand.Next(3);
                        ignoredY1 = rand.Next(3);
                        if (!(ignoredX1 == 1 & ignoredY1 == 1))
                            ok = true;
                    }
                    ok = false;
                    while (!ok)
                    {
                        ignoredX2 = rand.Next(3);
                        ignoredY2 = rand.Next(3);
                        if ((!(ignoredX2 == 1 & ignoredY2 == 1)) & (!(ignoredX2 == ignoredX1 & ignoredY2 == ignoredY1)))
                            ok = true;
                    }
                    break;
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if ((!((i == ignoredX1 & j == ignoredY1) | (i == ignoredX2 & j == ignoredY2))) & (grid[i, j].value != grid[1, 1].value))
                        result++;

                }
            }
            return result;
        }

        public int findRandomNeighbor(int r, int cellSize, Cell[,] grid, int type)
        {
            Random rand = new Random();
            int c = 0;
            int result = 0;
            int x = r;
            int y = r;
            switch (type)
            {
                case 1:
                    while(true)
                    {
                        c++;
                        x = rand.Next(3);
                        y = rand.Next(3);
                        if (!(x == 1 & y == 1))
                            break;
                    }
                    break;
            }
            result = grid[x, y].value;
            return result;
        }

        public bool isOnBorder(int r, int cellSize, Cell[,] grid, int type)
        {
            bool result = false;
            int temp = 0;
            switch (type)
            {
                case 0:
                    temp = vonNeumanMCEnergy(grid);
                    if (temp > 0)
                        result = true;
                    break;
                case 1:
                    temp = mooreMCEnergy(grid);
                    if (temp > 0)
                        result = true;
                    break;
            }
            return result;
        }

        public bool mooreDislocations(Cell[,] grid)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (grid[i, j].dislocationDensity > grid[1, 1].dislocationDensity)
                        return false;
                }
            }
            return true;
        }
        public bool checkNeighbourhoodDislocations(int r, int cellSize, Cell[,] grid, int type)
        {
            bool result = false;
            switch (type)
            {
                case 1:
                    result = mooreDislocations(grid);
                    break;
            }
            return result;
        }

        public int mooreRecrystalized(Cell[,] grid, List<Cell> latelyRecrystalized)
        {
            int result = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if(!(i==1 & j==1))
                    {
                        if (latelyRecrystalized.Exists(Cell => Cell.id == grid[i, j].id))
                            return grid[i, j].value;
                    }
                }
            }
            return result;
        }
        public int isNeighbourRecrystalized(int r, int cellSize, Cell[,] grid, int type, List<Cell> latelyRecrystalized)
        {
            int result = 0;
            switch (type)
            {
                case 1:
                    result = mooreRecrystalized(grid,latelyRecrystalized);
                    break;
            }
            return result;
        }
    }
}
