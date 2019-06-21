using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RozrostZiarenCA2
{
    class Grid                                          //warunki brzegowe
    {                                                   //0 - absorbujące (absorbing)
        public Cell[,] grid;
        public double[,] DensityMap;
        public double densityMax = 0;
        public int[,] EnergyMap;
        public int energyMax = 0;
        public int gridWidth;
        public int gridHeight;
        public int boundaryConditionType = 0;
        public int counter = 0;
        public int cellCounter = 0;
        public int cellSize = 0;
        public int neighborhoodType = 0;
        public int radius = 1;
        public List<ColorRGB> colorList = new List<ColorRGB>();
        const double A = 86710969050178.5f;
        const double B = 9.41268203527779f;
        double deltaDislocationDensity = 0, dislocationDensity = 0, recrystalizationTimeStep = 0, dislocationDensityCritical = 0;
        public Grid(int width, int height, int bc, int tempCellSize)
        {
            int id = 0;
            Random rand = new Random();
            colorList.Add(new ColorRGB());
            cellSize = tempCellSize;
            gridWidth = width;
            gridHeight = height;
            grid = new Cell[width,height];
            DensityMap = new double[width, height];
            EnergyMap = new int[width, height];
            for (int j=0; j<height;j++)
            {
                for (int i = 0; i < width; i++)
                {
                    EnergyMap[i, j] = 0;
                    DensityMap[i, j] = 0;
                    grid[i,j] = new Cell(rand.Next(tempCellSize-1)+1, rand.Next(tempCellSize-1)+1);
                    grid[i, j].id = id;
                    id++;
                }
            }
            boundaryConditionType = bc;
        }
        public Cell[,] absorbing(int x, int y, int radius)
        {
            int size = radius * 2 + 1;
            Cell[,] temp = new Cell[size, size]; ;
            for (int j = 0; j < size; j++)
            {
                for (int i = 0; i < size; i++)
                {
                    temp[i, j] = new Cell(0,0);
                }
            }
            for(int i = 0; i < size; i++)
            {
                int xx = x - radius + i;
                for (int j = 0; j < size; j++)
                {
                    int yy = y - radius + j;
                    if ((xx >= 0) & (yy >= 0) & (xx < gridWidth) & (yy < gridHeight))
                    {
                        temp[i, j].value = grid[xx, yy].value;
                        temp[i, j].gravityX = grid[xx, yy].gravityX;
                        temp[i, j].gravityY = grid[xx, yy].gravityY;
                        temp[i, j].isRecrystalised = grid[xx, yy].isRecrystalised;
                        temp[i, j].dislocationDensity = grid[xx, yy].dislocationDensity;
                        temp[i, j].id = grid[xx, yy].id;
                    }

                }
            }
            return temp;
        }
        public Cell[,] periodical(int x, int y, int radius)
        {
            int size = radius * 2 + 1;
            Cell[,] temp = new Cell[size, size]; ;
            for (int j = 0; j < size; j++)
            {
                for (int i = 0; i < size; i++)
                {
                    temp[i, j] = new Cell(0, 0);
                }
            }
            for (int i = 0; i < size; i++)
            {
                int xx = x - radius + i;
                if (xx < 0 | xx >= gridWidth)
                    xx = Math.Abs(xx + gridWidth) % gridWidth;
                for (int j = 0; j < size; j++)
                {
                    int yy = y - radius + j;
                    if (yy < 0 | yy >= gridHeight)
                        yy = Math.Abs(yy + gridHeight) % gridHeight;                   
                    temp[i, j].value = grid[xx, yy].value;
                    temp[i, j].gravityX = grid[xx, yy].gravityX;
                    temp[i, j].gravityY = grid[xx, yy].gravityY;
                    temp[i, j].isRecrystalised = grid[xx, yy].isRecrystalised;
                    temp[i, j].dislocationDensity = grid[xx, yy].dislocationDensity;
                    temp[i, j].id = grid[xx, yy].id;
                }
            }
            return temp;
        }
        public void checkGrid(int cellSize)
        {
            Cell[,] temp = new Cell[gridWidth, gridHeight];
            Cell[,] singleIteration = new Cell[3,3];
            Neighborhood alterCell = new Neighborhood();
            for (int j = 0; j < gridHeight; j++)
            {
                for (int i = 0; i < gridWidth; i++)
                {
                    temp[i, j] = new Cell(0,0);
                    temp[i, j].gravityX = grid[i, j].gravityX;
                    temp[i, j].gravityY = grid[i, j].gravityY;
                    temp[i, j].isRecrystalised = grid[i, j].isRecrystalised;
                    temp[i, j].dislocationDensity = grid[i, j].dislocationDensity;
                    temp[i, j].id = grid[i, j].id;
                }
            }
            for (int j = 0; j < gridHeight; j++)
            {
                for (int i = 0; i < gridWidth; i++)
                {
                    switch (boundaryConditionType)
                    {
                        case 0:
                            singleIteration = absorbing(i, j, radius);
                            break;
                        case 1:
                            singleIteration = periodical(i, j, radius);
                            break;
                    }
                    int tempValue=0;
                    switch (neighborhoodType)
                    {
                        case 0:
                            tempValue = alterCell.vonNeumanType(singleIteration);
                            if (tempValue != temp[i, j].value)
                                cellCounter++;
                            temp[i, j].value = tempValue;
                            break;
                        case 1:
                            tempValue = alterCell.mooreType(singleIteration);
                            if (tempValue != temp[i, j].value)
                                cellCounter++;
                            temp[i, j].value = tempValue;
                            break;
                        case 2:
                            Random rand = new Random();
                            tempValue = alterCell.pentagonalType(singleIteration, rand.Next(4));
                            if (tempValue != temp[i, j].value)
                                cellCounter++;
                            temp[i, j].value = tempValue;
                            break;
                        case 3:
                            tempValue = alterCell.hexagonalType(singleIteration, 0);
                            if (tempValue != temp[i, j].value)
                                cellCounter++;
                            temp[i, j].value = tempValue;
                            break;
                        case 4:
                            tempValue = alterCell.hexagonalType(singleIteration, 1);
                            if (tempValue != temp[i, j].value)
                                cellCounter++;
                            temp[i, j].value = tempValue;
                            break;
                        case 5:
                            tempValue = alterCell.hexagonalType(singleIteration, 2);
                            if (tempValue != temp[i, j].value)
                                cellCounter++;
                            temp[i, j].value = tempValue;
                            break;
                        case 6:
                            tempValue = alterCell.radiusType(radius, cellSize, singleIteration);
                            if (tempValue != temp[i, j].value)
                                cellCounter++;
                            temp[i, j].value = tempValue;
                            break;
                    }
                }
            }
            grid = temp;
        }
        public void energyMaxFind()
        {
            energyMax = 0;
            for (int j = 0; j < gridHeight; j++)
            {
                for (int i = 0; i < gridWidth; i++)
                {
                    if (EnergyMap[i, j] > energyMax)
                        energyMax = EnergyMap[i, j];
                }
            }
        }
        public void radiusCheck(int x, int y, int r, int cellSize)
        {
            int xx=0, yy=0;
            int centerX=0, centerY=0, centerXX=0, centerYY=0;
            centerX = x * cellSize + cellSize / 2;
            centerY = y * cellSize + cellSize / 2;
            int radius = r * cellSize;
            bool skip = false;
            for(int i=-r; i<=r;i++)
            {
                for(int j=-r; j<=r; j++)
                {
                    skip = false;
                    centerXX = 0;
                    centerYY = 0;
                    xx = x + i;
                    yy = y + j;
                    if (xx<0)
                    {
                        if (boundaryConditionType == 1)
                        {
                            xx = Math.Abs(xx + gridWidth) % gridWidth;
                            centerXX = -gridWidth * cellSize;
                        }
                        else
                            skip = true;
                    }
                    else if(xx >= gridWidth)
                    {
                        if (boundaryConditionType == 1)
                        {
                            xx = Math.Abs(xx + gridWidth) % gridWidth;
                            centerXX = gridWidth * cellSize;
                        }
                        else
                            skip = true;
                    }
                    if (yy < 0)
                    {
                        if (boundaryConditionType == 1)
                        {
                            yy = Math.Abs(yy + gridHeight) % gridHeight;
                            centerYY = -gridHeight * cellSize;
                        }
                        else
                            skip = true;
                    }
                    else if (yy >= gridHeight)
                    {
                        if (boundaryConditionType == 1)
                        {
                            yy = Math.Abs(yy + gridHeight) % gridHeight;
                            centerYY = gridHeight * cellSize;
                        }
                        else
                            skip = true;
                    }
                    if (!skip)
                    {
                        centerXX += xx * cellSize + grid[xx, yy].gravityX;
                        centerYY += yy * cellSize + grid[xx, yy].gravityY;
                        double distance=0;
                        distance = Math.Sqrt((centerX - centerXX) * (centerX - centerXX) + (centerY - centerYY) * (centerY - centerYY));
                        if(distance<=radius)
                        {
                            grid[xx, yy].isAvailable = false;
                        }
                    }
                }
            }
        }
        public void MonteCarlo(double kt)
        {

            cellCounter = 0;
            for (int j = 0; j < gridWidth; j++)
            {
                for (int i = 0; i < gridHeight; i++)
                {
                    grid[j, i].isAvailableForMC=true;
                }
            }
            int x = 0, y = 0;
            Random rand = new Random();
            int newValue = 0; ;
            Neighborhood alterCell = new Neighborhood();
            Cell[,] singleIteration = new Cell[3, 3];
            Cell[,] newSingleIteration = new Cell[3, 3];
            while (cellCounter<gridWidth*gridHeight)
            {
                x = rand.Next(gridWidth);
                y = rand.Next(gridHeight);
                while(!grid[x,y].isAvailableForMC)
                {
                    x++;
                    if(x == gridWidth)
                    {
                        x = 0;
                        y++;
                    }
                    if (y == gridHeight)
                        y = 0;
                }
                grid[x, y].isAvailableForMC = false;
                cellCounter++;
                int currentCellEnergy = 0;
                int newCellEnergy = 0;
                switch (boundaryConditionType)
                {
                    case 0:
                        singleIteration = absorbing(x, y, radius);
                        break;
                    case 1:
                        singleIteration = periodical(x, y, radius);
                        break;
                }
                newSingleIteration = singleIteration;
                newValue = alterCell.findRandomNeighbor(radius, cellSize, singleIteration, 1);
                if (newValue == 0)
                    newValue = grid[x, y].value;
                switch (neighborhoodType)
                {
                    case 0:
                        currentCellEnergy = alterCell.vonNeumanMCEnergy(singleIteration);
                        newSingleIteration[radius, radius].value = newValue;
                        newCellEnergy = alterCell.vonNeumanMCEnergy(newSingleIteration);
                        break;
                    case 1:
                        currentCellEnergy = alterCell.mooreMCEnergy(singleIteration);
                        newSingleIteration[radius, radius].value = newValue;
                        newCellEnergy = alterCell.mooreMCEnergy(newSingleIteration);
                        break;
                    case 2:
                        currentCellEnergy = alterCell.pentagonalMCEnergy(singleIteration, rand.Next(4));
                        newSingleIteration[radius, radius].value = newValue;
                        newCellEnergy = alterCell.pentagonalMCEnergy(newSingleIteration, rand.Next(4));
                        break;
                    case 3:
                        currentCellEnergy = alterCell.hexagonalMCEnergy(singleIteration, 0);
                        newSingleIteration[radius, radius].value = newValue;
                        newCellEnergy = alterCell.hexagonalMCEnergy(newSingleIteration, 0);
                        break;
                    case 4:
                        currentCellEnergy = alterCell.hexagonalMCEnergy(singleIteration, 1);
                        newSingleIteration[radius, radius].value = newValue;
                        newCellEnergy = alterCell.hexagonalMCEnergy(newSingleIteration, 1);
                        break;
                    case 5:
                        currentCellEnergy = alterCell.hexagonalMCEnergy(singleIteration, 2);
                        newSingleIteration[radius, radius].value = newValue;
                        newCellEnergy = alterCell.hexagonalMCEnergy(newSingleIteration, 2);
                        break;
                    case 6:
                        currentCellEnergy = alterCell.radiusMCEnergy(radius, cellSize, singleIteration);
                        newSingleIteration[radius, radius].value = newValue;
                        newCellEnergy = alterCell.radiusMCEnergy(radius, cellSize, newSingleIteration);
                        break;
                }
                if (newCellEnergy <= currentCellEnergy)
                {
                    EnergyMap[x, y] = newCellEnergy;
                    grid[x, y].value = newValue;
                }
                else
                {
                    double p = rand.NextDouble();
                    double probability = (newCellEnergy-currentCellEnergy)/kt * (-1);
                    probability = Math.Exp(probability);
                    if(p<=probability)
                    {
                        EnergyMap[x, y] = currentCellEnergy;
                        grid[x, y].value = newValue;
                    }
                }
            }
            energyMaxFind();
        }
        public void densityMaxFind()
        {
            densityMax = 0;
            for (int j = 0; j < gridHeight; j++)
            {
                for (int i = 0; i < gridWidth; i++)
                {
                    if (DensityMap[i, j] > densityMax)
                        densityMax = DensityMap[i, j];
                }
            }
        }
        public void DRX(double dt, double percentX)
        {
            Random rand = new Random();
            Neighborhood alterCell = new Neighborhood();
            Cell[,] singleIteration = new Cell[3, 3];

            deltaDislocationDensity = (A / B + (1 - A / B) * Math.Exp(-B * recrystalizationTimeStep)) - dislocationDensity;

            dislocationDensity = ((A / B) + (1 - A / B) * Math.Exp(-1 * B * recrystalizationTimeStep));
            dislocationDensityCritical = (46842668.25*300*300)/(gridHeight*gridWidth);
            //writeToFile.SaveToFile(DislocationDensity);
            recrystalizationTimeStep = recrystalizationTimeStep + dt;

            double avaragePackage = (deltaDislocationDensity / (gridWidth * gridHeight)) * percentX;

            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    grid[i, j].dislocationDensity += avaragePackage;
                    DensityMap[i, j] += avaragePackage;
                    deltaDislocationDensity -= avaragePackage;
                }
            }
            double randomPackage;
            int xx, yy;
            double proability;

            while (deltaDislocationDensity > 0)
            {
                xx = rand.Next(gridWidth);
                yy = rand.Next(gridHeight);

                switch (boundaryConditionType)
                {
                    case 0:
                        singleIteration = absorbing(xx, yy, radius);
                        break;
                    case 1:
                        singleIteration = periodical(xx, yy, radius);
                        break;
                }

                if (alterCell.isOnBorder(radius, cellSize, singleIteration, 1))
                {
                    proability = rand.NextDouble();
                    randomPackage = deltaDislocationDensity * rand.NextDouble();
                    if (randomPackage <= deltaDislocationDensity && proability > 0.2)
                    {
                        grid[xx, yy].dislocationDensity += randomPackage;
                        DensityMap[xx, yy] += randomPackage;
                        deltaDislocationDensity -= randomPackage;
                    }
                    if (deltaDislocationDensity < 0.00001)
                    {
                        deltaDislocationDensity = 0;
                    }
                }
                else
                {
                    proability = rand.NextDouble();
                    randomPackage = deltaDislocationDensity * rand.NextDouble();
                    if (randomPackage <= deltaDislocationDensity && proability <= 0.2)
                    {
                        grid[xx, yy].dislocationDensity += randomPackage;
                        deltaDislocationDensity -= randomPackage;
                    }
                    if (deltaDislocationDensity < 0.00001)
                    {
                        deltaDislocationDensity = 0;
                    }
                }

                List<Cell> latelyRecrystalized = new List<Cell>();
                for (int i = 0; i < gridWidth; i++)
                {
                    for (int j = 0; j < gridHeight; j++)
                    {
                        switch (boundaryConditionType)
                        {
                            case 0:
                                singleIteration = absorbing(i, j, radius);
                                break;
                            case 1:
                                singleIteration = periodical(i, j, radius);
                                break;
                        }
                        if ((alterCell.isOnBorder(radius, cellSize, singleIteration, 1)) & (grid[i, j].dislocationDensity > dislocationDensityCritical) & (!grid[i, i].isRecrystalised))
                        {
                            counter++;
                            int c = 0, r = 0, g = 0, b = 0;
                            bool colorOK = true;
                            while (c < 1000)
                            {
                                r = rand.Next(256);
                                g = rand.Next(256);
                                b = rand.Next(256);
                                for (int k = 0; k < colorList.Count; k++)
                                {
                                    if (colorList[k].r == r & colorList[k].g == g & colorList[k].b == b)
                                        colorOK = false;
                                }
                                c++;
                                if (colorOK)
                                {
                                    c = 2000;
                                }
                            }
                            colorList.Add(new ColorRGB());
                            colorList[colorList.Count - 1].r = r;
                            colorList[colorList.Count - 1].g = g;
                            colorList[colorList.Count - 1].b = b;
                            grid[i, j].value = counter;
                            grid[i, j].dislocationDensity = 0;
                            DensityMap[i, j] = 0;
                            grid[i, j].isRecrystalised = true;
                            latelyRecrystalized.Add(grid[i, j]);
                        }
                    }
                }

                /*for (int i = 0; i < gridWidth; i++)
                {
                    for (int j = 0; j < gridHeight; j++)
                    {
                        switch (boundaryConditionType)
                        {
                            case 0:
                                singleIteration = absorbing(i, j, radius);
                                break;
                            case 1:
                                singleIteration = periodical(i, j, radius);
                                break;
                        }

                        int tmp = alterCell.isNeighbourRecrystalized(radius, cellSize, singleIteration, 1, latelyRecrystalized);

                        if ((tmp != 0) & !(grid[i, j].isRecrystalised))
                        {
                            if (alterCell.checkNeighbourhoodDislocations(i, j, singleIteration, 1))
                            {
                                grid[i, j].value = tmp;
                                grid[i, j].dislocationDensity = 0;
                                DensityMap[i, j] = 0;
                                grid[i, j].isRecrystalised =  true;
                            }
                        }
                    }
                }*/

            }
            densityMaxFind();
        }           
    }
}
