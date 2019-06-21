using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace RozrostZiarenCA2
{
    public partial class Form1 : Form
    {
        bool lines = true;
        bool draw = false;
        bool go = false;
        bool isRadius = false;
        bool blockedCells = false;
        bool gravityCenter = false;
        int radius = 1;
        Grid board;
        int heightSpace = 0;
        int widthSpace = 0;
        int sizeOfSquare = 0;
        int speed = 501;
        int boundaryConditionType = 0;
        int viewType = 0;
        private static System.Timers.Timer tt;
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Paint += new PaintEventHandler(this.pictureBox1_Paint);
            this.Text = "Rozrost ziaren CA";
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.Size = new Size(1460, 825);
            sizeOfSquare = pictureBox1.Height / 10;
            if ((pictureBox1.Width / 10) < sizeOfSquare)
            {
                sizeOfSquare = pictureBox1.Width / 10;
            }
            heightSpace = (pictureBox1.Height - sizeOfSquare * 10) / 2;
            widthSpace = (pictureBox1.Width - sizeOfSquare * 10) / 2;
            board = new Grid(10, 10, boundaryConditionType, sizeOfSquare);
            draw = true;
            pictureBox1.Invalidate();
            tt = new System.Timers.Timer();
            tt.Interval = speed;
            tt.Elapsed += OnTimedEvent;
            tt.AutoReset = true;
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 0;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
           if(draw)
            {
                viewType = comboBox2.SelectedIndex;
                Pen blackPen = new Pen(Color.Black);
                blackPen.Width = 1;
                double tempDensity = 255;
                tempDensity = tempDensity / (board.densityMax + 1);
                tempDensity = (int)tempDensity;
                int tempEnergy = 255;
                tempEnergy= tempEnergy / (board.energyMax + 1);
                int tempColor;
                for (int i = 0; i < board.gridHeight; i++)
                {
                    for (int j = 0; j < board.gridWidth; j++) //Color.FromArgb(255, r, g, b)   board.colorList[board.grid[j, i].value]
                    {
                        switch (viewType)
                        {
                            case 0:
                                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, board.colorList[board.grid[j, i].value].r, board.colorList[board.grid[j, i].value].g, board.colorList[board.grid[j, i].value].b)), (j * sizeOfSquare + widthSpace), (i * sizeOfSquare + heightSpace), sizeOfSquare, sizeOfSquare);
                                break;
                            case 1:
                                tempColor = board.EnergyMap[j, i] * tempEnergy;
                                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(tempColor, 0, 0, 255)), (j * sizeOfSquare + widthSpace), (i * sizeOfSquare + heightSpace), sizeOfSquare, sizeOfSquare);
                                break;
                            case 2:
                                double tmp = board.DensityMap[j, i] * tempDensity;
                                tempColor = (int)tmp;
                                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(tempColor, 255, 0, 0)), (j * sizeOfSquare + widthSpace), (i * sizeOfSquare + heightSpace), sizeOfSquare, sizeOfSquare);
                                break;
                        }
                        if(gravityCenter)
                            e.Graphics.FillEllipse(new SolidBrush(Color.Red), (j * sizeOfSquare + widthSpace + board.grid[j, i].gravityX - 5), (i * sizeOfSquare + heightSpace + board.grid[j, i].gravityY - 5), 10, 10);
                        if (blockedCells & !board.grid[j, i].isAvailable)
                        {
                            e.Graphics.DrawLine(blackPen, (j * sizeOfSquare + widthSpace), (i * sizeOfSquare + heightSpace), ((j+1) * sizeOfSquare + widthSpace), ((i+1) * sizeOfSquare + heightSpace));
                            e.Graphics.DrawLine(blackPen, (j * sizeOfSquare + widthSpace), ((i+1) * sizeOfSquare + heightSpace), ((j+1) * sizeOfSquare + widthSpace), (i * sizeOfSquare + heightSpace));
                        }
                    }
                }
                if(lines)
                {
                    for (int i = 0; i <= board.gridHeight; i++)
                    {
                        e.Graphics.DrawLine(blackPen, widthSpace, (i * sizeOfSquare + heightSpace), (sizeOfSquare * board.gridWidth + widthSpace), (i * sizeOfSquare + heightSpace));
                    }
                    for (int i = 0; i <= board.gridWidth; i++)
                    {
                        e.Graphics.DrawLine(blackPen, (i * sizeOfSquare + widthSpace), heightSpace, (i * sizeOfSquare + widthSpace), (sizeOfSquare * board.gridHeight + heightSpace));
                    }
                }
                if(go)
                {
                    board.checkGrid(sizeOfSquare);
                }
            }
            go = false;
            draw = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            board.neighborhoodType = comboBox1.SelectedIndex;
            bool ok = true;
            if (board.neighborhoodType == 6)
            {
                try
                {
                    board.radius = int.Parse(textBox6.Text);
                    if (board.radius <= 0)
                    {
                        MessageBox.Show("Błąd danych");
                        ok = false;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                    ok = false;
                }
            }
            else
                board.radius = 1;
            if(ok)
                tt.Enabled = true; 
        }
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            draw = true;
            go = true;
            BeginInvoke(new Action(() =>
                {
                    pictureBox1.Invalidate();
                }));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tt.Enabled = false;
            go = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool ok = false;
            int gridWidth = 0;
            int gridHeight = 0;
            int amountOfGrains = 0;
            try
            {
                gridWidth = int.Parse(textBox2.Text);
                gridHeight = int.Parse(textBox1.Text);
                amountOfGrains = int.Parse(textBox3.Text);
                if(gridWidth>0 & gridHeight>0 & amountOfGrains>0)
                {
                    ok = true;
                }
                else
                {
                    MessageBox.Show("Błąd danych");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
                ok = false;
            }
            if (ok)
            {
                Random rand = new Random();
                board = new Grid(gridWidth, gridHeight, boundaryConditionType, sizeOfSquare);
                for(int i=0; i<5 * amountOfGrains;i++)
                {
                    if(board.counter<amountOfGrains)
                    {
                        int x, y;
                        x = rand.Next(gridWidth);
                        y = rand.Next(gridHeight);
                        if(board.grid[x,y].isAvailable)
                        {
                            int r, g, b;
                            int counter = 0;
                            bool colorOK = true;
                            while (counter < 1000)
                            {
                                r = rand.Next(256);
                                g = rand.Next(256);
                                b = rand.Next(256);
                                for (int j = 0; j < board.colorList.Count; j++)
                                {
                                    if (board.colorList[j].r == r & board.colorList[j].g == g & board.colorList[j].b == b)
                                        colorOK = false;
                                }
                                counter++;
                                if (colorOK)
                                {
                                    counter = 2000;
                                    board.colorList.Add(new ColorRGB());
                                    board.colorList[board.colorList.Count-1].r = r;
                                    board.colorList[board.colorList.Count-1].g = g;
                                    board.colorList[board.colorList.Count-1].b = b;
                                }
                            }
                            if (counter == 2000)
                            {
                                board.counter++;
                                board.grid[x, y].value = board.counter;
                                board.grid[x, y].isAvailable = false;
                                if (isRadius)
                                    board.radiusCheck(x, y, radius, sizeOfSquare);
                            }
                            else
                            {
                                //MessageBox.Show("Błąd dodawania zarodka - brak wolnego koloru");
                            }
                        }
                    }
                }
                textBox3.Text = board.counter.ToString();
                sizeOfSquare = pictureBox1.Height / board.gridHeight;
                if ((pictureBox1.Width / board.gridWidth) < sizeOfSquare)
                {
                    sizeOfSquare = pictureBox1.Width / board.gridWidth;
                }
                heightSpace = (pictureBox1.Height - sizeOfSquare * board.gridHeight) / 2;
                widthSpace = (pictureBox1.Width - sizeOfSquare * board.gridWidth) / 2;
                draw = true;
                pictureBox1.Invalidate();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool ok = false;
            int gridWidth = 0;
            int gridHeight = 0;
            int amountOfGrainsWidth = 0;
            int amountOfGrainsHeight = 0;
            try
            {
                gridWidth = int.Parse(textBox2.Text);
                gridHeight = int.Parse(textBox1.Text);
                amountOfGrainsWidth = int.Parse(textBox4.Text);
                amountOfGrainsHeight = int.Parse(textBox5.Text);
                if (gridWidth > 0 & gridHeight > 0 & amountOfGrainsWidth > 0 & amountOfGrainsHeight > 0 & amountOfGrainsWidth <= gridWidth & amountOfGrainsHeight <= gridHeight)
                {
                    ok = true;
                }
                else
                {
                    MessageBox.Show("Błąd danych");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
                ok = false;
            }
            if (ok)
            {
                int heightSpace, widthSpace, heightStart, widthStart;
                heightSpace = gridHeight / amountOfGrainsHeight;
                widthSpace = gridWidth / amountOfGrainsWidth;
                heightStart = (gridHeight - ((amountOfGrainsHeight - 1) * heightSpace) - 1) / 2;
                widthStart = (gridWidth - ((amountOfGrainsWidth - 1) * widthSpace) - 1) / 2;
                Random rand = new Random();
                board = new Grid(gridWidth, gridHeight, boundaryConditionType, sizeOfSquare);
                for(int i=0; i<amountOfGrainsWidth; i++)
                {
                    for(int k=0; k<amountOfGrainsHeight; k++)
                    {
                        int r, g, b;
                        int counter = 0;
                        bool colorOK = true;
                        while (counter < 1000)
                        {
                            r = rand.Next(256);
                            g = rand.Next(256);
                            b = rand.Next(256);
                            for (int j = 0; j < board.colorList.Count; j++)
                            {
                                if (board.colorList[j].r == r & board.colorList[j].g == g & board.colorList[j].b == b)
                                    colorOK = false;
                            }
                            counter++;
                            if (colorOK)
                            {
                                counter = 2000;
                                board.colorList.Add(new ColorRGB());
                                board.colorList[board.colorList.Count - 1].r = r;
                                board.colorList[board.colorList.Count - 1].g = g;
                                board.colorList[board.colorList.Count - 1].b = b;
                            }
                        }
                        if (counter == 2000)
                        {
                            board.counter++;
                            board.grid[widthStart + i * widthSpace, heightStart + k * heightSpace].value = board.counter;
                            board.grid[widthStart + i * widthSpace, heightStart + k * heightSpace].isAvailable = false;

                        }
                        else
                        {
                            //MessageBox.Show("Błąd dodawania zarodka - brak wolnego koloru");
                        }
                    }
                }
                sizeOfSquare = pictureBox1.Height / board.gridHeight;
                if ((pictureBox1.Width / board.gridWidth) < sizeOfSquare)
                {
                    sizeOfSquare = pictureBox1.Width / board.gridWidth;
                }
                heightSpace = (pictureBox1.Height - sizeOfSquare * board.gridHeight) / 2;
                widthSpace = (pictureBox1.Width - sizeOfSquare * board.gridWidth) / 2;
                draw = true;
                pictureBox1.Invalidate();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            bool ok = false;
            int gridWidth = 0;
            int gridHeight = 0;
            try
            {
                gridWidth = int.Parse(textBox2.Text);
                gridHeight = int.Parse(textBox1.Text);
                if (gridWidth > 0 & gridHeight > 0)
                {
                    ok = true;
                }
                else
                {
                    MessageBox.Show("Błąd danych");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
                ok = false;
            }
            if (ok)
            {
                board = new Grid(gridWidth, gridHeight, boundaryConditionType, sizeOfSquare);
                sizeOfSquare = pictureBox1.Height / board.gridHeight;
                if ((pictureBox1.Width / board.gridWidth) < sizeOfSquare)
                {
                    sizeOfSquare = pictureBox1.Width / board.gridWidth;
                }
                heightSpace = (pictureBox1.Height - sizeOfSquare * board.gridHeight) / 2;
                widthSpace = (pictureBox1.Width - sizeOfSquare * board.gridWidth) / 2;
                draw = true;
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point position = me.Location;
            if((position.X>widthSpace) & (position.X < (pictureBox1.Width - widthSpace)) & (position.Y > heightSpace) & (position.Y < (pictureBox1.Height - heightSpace)) )
            {
                int x = (position.X - widthSpace) / sizeOfSquare;
                int y = (position.Y - heightSpace) / sizeOfSquare;
                if(board.grid[x, y].isAvailable)
                {
                    int r, g, b;
                    int counter = 0;
                    Random rand = new Random();
                    bool colorOK = true;
                    while(counter<1000)
                    {
                        r = rand.Next(256);
                        g = rand.Next(256);
                        b = rand.Next(256);
                        for(int i =0; i < board.colorList.Count; i++)
                        {
                            if(board.colorList[i].r == r & board.colorList[i].g == g & board.colorList[i].b == b)
                                colorOK = false;
                        }
                        counter++;
                        if(colorOK)
                        {
                            counter = 2000;
                            board.colorList.Add(new ColorRGB());
                            board.colorList[board.colorList.Count-1].r = r;
                            board.colorList[board.colorList.Count-1].g = g;
                            board.colorList[board.colorList.Count-1].b = b;
                        }
                    }
                    if(counter ==2000)
                    {
                        board.counter++;
                        board.grid[x, y].value = board.counter;
                        board.grid[x, y].isAvailable = false;
                        if (isRadius)
                            board.radiusCheck(x, y, radius, sizeOfSquare);
                    }
                    else
                    {
                        MessageBox.Show("Błąd dodawania zarodka - brak wolnego koloru");
                    }
                }
                else
                {
                    board.grid[x, y].value = 0;
                    board.grid[x, y].isAvailable = true;
                }
                draw = true;
                pictureBox1.Invalidate();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if(speed  > 1)
            {
                speed -= 50;
                tt.Interval = speed;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (speed < 1001)
            {
                speed += 50;
                tt.Interval = speed;
            }
        }
       
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
                board.boundaryConditionType = 0;
                boundaryConditionType = 0;
            }
            else
            {
                checkBox2.Checked = true;
                board.boundaryConditionType = 1;
                boundaryConditionType = 1;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
                board.boundaryConditionType = 1;
            }
            else
            {
                checkBox1.Checked = true;
                board.boundaryConditionType = 0;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                lines = true;
                draw = true;
                pictureBox1.Invalidate();
            }
            else
            {
                lines = false;
                draw = true;
                pictureBox1.Invalidate();
            }   
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                bool ok = false;
                try
                {
                    radius = int.Parse(textBox6.Text);
                    if (radius > 0)
                    {
                        ok = true;
                    }
                    else
                    {
                        MessageBox.Show("Błąd danych");
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                    ok = false;
                    checkBox3.Checked = false;
                    textBox6.Enabled = true;
                }
                if (ok)
                {
                    isRadius = true;
                    textBox6.Enabled = false;
                }
            }
            else
            {
                isRadius = false;
                textBox6.Enabled = true;
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                blockedCells = true;
                draw = true;
                pictureBox1.Invalidate();
            }
            else
            {
                blockedCells = false;
                draw = true;
                pictureBox1.Invalidate();
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                gravityCenter = true;
                draw = true;
                pictureBox1.Invalidate();
            }
            else
            {
                gravityCenter = false;
                draw = true;
                pictureBox1.Invalidate();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            bool ok = false;
            double kt = 0.1;
            try
            {
                kt = double.Parse(textBox7.Text);
                if (kt >= 0.1 & kt <=6)
                {
                    ok = true;
                }
                else
                {
                    MessageBox.Show("Błąd danych");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
                ok = false;
            }
            if (ok)
            {
                board.MonteCarlo(kt);
                draw = true;
                pictureBox1.Invalidate();
                MessageBox.Show("gotowe");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            bool ok = false;
            double dt = 0.001;
            double percentX = 0.2;
            try
            {
                percentX = double.Parse(textBox8.Text);
                dt = double.Parse(textBox9.Text);
                if (dt > 0 & percentX > 0)
                {
                    ok = true;
                }
                else
                {
                    MessageBox.Show("Błąd danych");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
                ok = false;
            }
            if (ok)
            {
                board.DRX(dt,percentX);
                draw = true;
                pictureBox1.Invalidate();
                MessageBox.Show("gotowe");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            draw = true;
            pictureBox1.Invalidate();
        }
    }  
}
