using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Predator_Prey
{
    public partial class Form1 : Form
    {
        Graphics g;

        public Form1()
        {
            InitializeComponent();

            g = panel1.CreateGraphics();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private class Predator
        {
            public int XPos;
            public int YPos;
            public int fastDur;
            public Predator(int xPos, int yPos)
            {
                XPos = xPos;
                YPos = yPos;
                fastDur = 0;
            }
        }

        private class Prey
        {
            public int XPos;
            public int YPos;
            public Prey(int xPos, int yPos)
            {
                XPos = xPos;
                YPos = yPos;
            }
        }

        List<Predator> predators;
        List<Prey> preys;
        int gridSize = 30;
        int cellSize = 20;
        int[,] entities;
        Random rand = new Random();
        // Запуск
        private void button1_Click(object sender, EventArgs e)
        {
            gridSize = SizeSelecter();
            g.Clear(Color.White);
            for (int i = 0; i <= gridSize; i++)
            {
                g.DrawLine(Pens.Gray, i * cellSize, 0, i * cellSize, gridSize * cellSize);
                g.DrawLine(Pens.Gray, 0, i * cellSize, gridSize * cellSize, i * cellSize);
            }
            g.DrawLine(Pens.Black, 0, 0, 0, 619);
            g.DrawLine(Pens.Black, 0, 0, 619, 0);
            g.DrawLine(Pens.Black, 619, 0, 619, 619);
            g.DrawLine(Pens.Black, 0, 619, 619, 619);
            entities = new int[gridSize, gridSize];
            predators = new List<Predator>();
            preys = new List<Prey>();
            EntitiesConstruct(entities, gridSize, predators, preys);
            timer1.Enabled = true;
        }

        private int SizeSelecter()
        {
            if (radioButton1.Checked == true)
                return 30;
            else if (radioButton2.Checked == true)
                return 25;
            else if (radioButton3.Checked == true)
                return 20;
            else if (radioButton4.Checked == true)
                return 15;
            else
            {
                radioButton1.Checked = true;
                return 30;
            }
        }

        private void EntitiesConstruct(int[,] entities, int gridsize, List<Predator> predators, List<Prey> preys)
        {
            Random rand = new Random();
            List<(int, int)> positions = new List<(int, int)>();
            for (int i = 0; i < gridsize; i++)
                for (int j = 0; j < gridsize; j++)
                    positions.Add((i, j));
            int? countPreds = int.Parse(textBoxPred.Text);
            int? countPreys = int.Parse(textBoxPrey.Text);
            if (countPreds == null) countPreds = 0;
            if (countPreys == null) countPreys = 0;
            for (int i = 0; i < countPreds; i++)
            {
                int index = rand.Next(positions.Count);
                (int row, int col) = positions[index];
                positions.RemoveAt(index);
                predators.Add(new Predator(row, col));
                entities[row, col] = 1;
            }
            for (int i = 0; i < countPreys; i++)
            {
                int index = rand.Next(positions.Count);
                (int row, int col) = positions[index];
                positions.RemoveAt(index);
                preys.Add(new Prey(row, col));
                entities[row, col] = -1;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int[,] newEntities = (int[,])entities.Clone(); // Полотно для следующего хода


            // беспорядочное движение хищников
            for (int i = 0; i < predators.Count; i++)
            {
                int x = predators[i].XPos;
                int y = predators[i].YPos;

                List<(int, int)> directions = new List<(int, int)> { (-1, 0), (1, 0), (0, -1), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1) };

                directions = directions.OrderBy(a => rand.Next()).ToList();
                bool isMoving = false;

                //напрвленно
                foreach ((int dx, int dy) in directions)
                {
                    if (x + dx >= 0 && x + dx < gridSize && y + dy >= 0 && y + dy < gridSize
                        && newEntities[x + dx, y + dy] == -1)
                    {
                        newEntities[x, y] = 0;
                        g.FillRectangle(Brushes.White, 2 + predators[i].XPos * 20, 2 + predators[i].YPos * 20, 17, 17);
                        newEntities[x + dx, y + dy] = 1;
                        predators[i].XPos += dx; x = predators[i].XPos;
                        predators[i].YPos += dy; y = predators[i].YPos;
                        for (int p = 0; p < preys.Count; p++)
                        {
                            if (preys[p].XPos == x && preys[p].YPos == y)
                            {
                                g.FillRectangle(Brushes.White, 2 + preys[p].XPos * 20, 2 + preys[p].YPos * 20, 17, 17);
                                preys.RemoveAt(p);
                                continue;
                            }
                        }
                        isMoving = true;
                        break;
                    }
                }
                //беспорядочно
                foreach ((int dx, int dy) in directions)
                {
                    if (x + dx >= 0 && x + dx < gridSize && y + dy >= 0 && y + dy < gridSize
                        && newEntities[x + dx, y + dy] == 0 && isMoving == false)
                    {
                        newEntities[x, y] = 0;
                        g.FillRectangle(Brushes.White, 2 + predators[i].XPos * 20, 2 + predators[i].YPos * 20, 17, 17);
                        newEntities[x + dx, y + dy] = 1;
                        predators[i].XPos += dx;
                        predators[i].YPos += dy;
                        isMoving = true;
                        break;
                    }
                }
                if (!isMoving)

                {
                    predators.RemoveAt(i);
                    newEntities[x, y] = 0;
                }
            }

            // движение жертв
            for (int i = 0; i < preys.Count; i++)
            {
                int x = preys[i].XPos;
                int y = preys[i].YPos;

                List<(int, int)> directions = new List<(int, int)> { (-1, 0), (1, 0), (0, -1), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1) };

                directions = directions.OrderBy(a => rand.Next()).ToList();
                bool isMoving = false;

                foreach ((int dx, int dy) in directions)
                {
                    if (x + dx >= 0 && x + dx < gridSize && y + dy >= 0 && y + dy < gridSize
                        && newEntities[x + dx, y + dy] == 0)
                    {
                        newEntities[x, y] = 0;
                        g.FillRectangle(Brushes.White, 2 + preys[i].XPos * 20, 2 + preys[i].YPos * 20, 17, 17);
                        newEntities[x + dx, y + dy] = 1;
                        preys[i].XPos = x + dx;
                        preys[i].YPos = y + dy;
                        isMoving = true;
                        break;
                    }
                }
                if (!isMoving)

                {
                    preys.RemoveAt(i);
                    newEntities[x, y] = 0;
                }
            }

            entities = newEntities;
            foreach (Predator predator in predators)
            {
                g.FillRectangle(Brushes.Red, 2 + predator.XPos * 20, 2 + predator.YPos * 20, 17, 17);
                continue;
            }
            foreach (Prey prey in preys)
            {
                g.FillRectangle(Brushes.Green, 2 + prey.XPos * 20, 2 + prey.YPos * 20, 17, 17);
                continue;
            }    
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (Predator predator in predators)
            {
                g.FillRectangle(Brushes.Red, 2 + predator.XPos * 20, 2 + predator.YPos * 20, 17, 17);
                continue;
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }
    }
}
