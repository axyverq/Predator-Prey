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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
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
            this.DoubleBuffered = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            textBox2.Text = 0.ToString();
        }

        private class Predator
        {
            public int XPos;
            public int YPos;
            public int fastDur;
            public int satDur;
            public Predator(int xPos, int yPos)
            {
                XPos = xPos;
                YPos = yPos;
                fastDur = 0;
                satDur = 0;
            }
        }

        private class Prey
        {
            public int XPos;
            public int YPos;
            public int satDur;
            public Prey(int xPos, int yPos)
            {
                XPos = xPos;
                YPos = yPos;
                satDur = 0;
            }
        }

        List<Predator> predators;
        List<Prey> preys;
        int gridSize = 30;
        int cellSize = 20;
        int[,] entities;
        Random rand = new Random();
        int maxEntities;
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
            timer2.Enabled = true;
        }

        private int SizeSelecter()
        {
            if (radioButton1.Checked == true)
            {
                maxEntities = 270;
                return 30;
            }
            else if (radioButton2.Checked == true)
            {
                maxEntities = 187;
                return 25;
            }
            else if (radioButton3.Checked == true)
            {
                maxEntities = 120;
                return 20;
            }
            else if (radioButton4.Checked == true)
            {
                maxEntities = 67;
                return 15;
            }
            else
            {
                maxEntities = 720;
                radioButton1.Checked = true;
                return 30;
            }
        }

        private void EntitiesConstruct(int[,] entities, int gridsize, List<Predator> predators, List<Prey> preys)
        {
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
            List<Predator> predatorsToRemove = new List<Predator>();
            List<Predator> predatorsToCreate = new List<Predator>();
            List<Prey> preysToRemove = new List<Prey>();
            List<Prey> preysToCreate = new List<Prey>();

            // поведение хищников
            foreach (Predator predator in predators.ToList())
            {
                predator.fastDur++;
                if (predator.fastDur > 5)
                    predator.satDur = 0;
                else predator.satDur++;

                int x = predator.XPos;
                int y = predator.YPos;

                List<(int, int)> directions = new List<(int, int)> { (-1, 0), (1, 0), (0, -1), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1) };
                List<(int, int)> huntDirections = new List<(int, int)> {   (-2, 0), (-1, 0), (1, 0), (2, 0),
                                                                           (-2, -1), (-1, -1), (0, -1), (1, -1), (2, -1),
                                                                           (-2, 1), (-1, 1), (0, 1), (1, 1), (2, 1),
                                                                           (-2, -2), (-1, -2), (0, -2), (1, -2), (2, -2),
                                                                           (-2, 2), (-1, 2), (0, 2), (1, 2), (2, 2) };

                directions = directions.OrderBy(a => rand.Next()).ToList();
                huntDirections = huntDirections.OrderBy(a => rand.Next()).ToList();
                bool isMoving = false;

                //движение
                //напрвленно
                if (predator.fastDur > 2)
                    foreach ((int dx, int dy) in huntDirections)
                    {
                        int newX = x + dx;
                        int newY = y + dy;
                        if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize
                            && newEntities[newX, newY] == -1)
                        {
                            newEntities[x, y] = 0;
                            g.FillRectangle(Brushes.White, 2 + predator.XPos * 20, 2 + predator.YPos * 20, 17, 17);
                            newEntities[newX, newY] = 1;
                            predator.XPos = newX - dx / 2;
                            predator.YPos = newY - dy / 2;
                            if (predator.XPos == newX && predator.YPos == newY)
                                foreach (Prey prey in preys.ToList())
                                {
                                    if (prey.XPos == newX && prey.YPos == newY)
                                    {
                                        g.FillRectangle(Brushes.White, 2 + prey.XPos * 20, 2 + prey.YPos * 20, 17, 17);
                                        preysToRemove.Add(prey);
                                        predator.fastDur = 0;
                                    }
                                }
                            isMoving = true;
                            break;
                        }
                    }

                //беспорядочно
                if (!isMoving)
                    foreach ((int dx, int dy) in directions)
                    {
                        int newX = x + dx;
                        int newY = y + dy;
                        if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize
                            && newEntities[newX, newY] == 0)
                        {
                            newEntities[x, y] = 0;
                            g.FillRectangle(Brushes.White, 2 + predator.XPos * 20, 2 + predator.YPos * 20, 17, 17);
                            newEntities[newX, newY] = 1;
                            predator.XPos = newX;
                            predator.YPos = newY;
                            isMoving = true;
                            break;
                        }
                    }

                // размножение
                if (predator.satDur >= 10 && predators.Count < maxEntities * 0.7 && (predators.Count + preys.Count) < maxEntities)
                    foreach ((int dx, int dy) in directions)
                    {
                        int newX = x + dx;
                        int newY = y + dy;
                        if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize
                            && newEntities[newX, newY] == 0)
                        {
                            predatorsToCreate.Add(new Predator(newX, newY));
                            newEntities[newX, newY] = 1;
                            predator.satDur = 0;
                            break;
                        }
                    }

                if (!isMoving || predator.fastDur >= 10)
                {
                    predatorsToRemove.Add(predator);
                    g.FillRectangle(Brushes.White, 2 + predator.XPos * 20, 2 + predator.YPos * 20, 17, 17);
                    newEntities[predator.XPos, predator.YPos] = 0;
                }
            }

            foreach (Prey prey in preysToRemove)
                preys.Remove(prey);

            preysToRemove = new List<Prey>();

            // поведение жертв
            foreach (Prey prey in preys.ToList())
            {
                prey.satDur++;

                int x = prey.XPos;
                int y = prey.YPos;

                List<(int, int)> directions = new List<(int, int)> { (-1, 0), (1, 0), (0, -1), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1) };

                directions = directions.OrderBy(a => rand.Next()).ToList();
                bool isMoving = false;
                foreach ((int dx, int dy) in directions)
                {
                    int newX = x + dx;
                    int newY = y + dy;
                    if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize
                        && newEntities[newX, newY] == 0)
                    {
                        newEntities[x, y] = 0;
                        g.FillRectangle(Brushes.White, 2 + prey.XPos * 20, 2 + prey.YPos * 20, 17, 17);
                        newEntities[newX, newY] = -1;
                        prey.XPos = newX;
                        prey.YPos = newY;
                        isMoving = true;
                        break;
                    }
                }

                // размножение
                if (prey.satDur >= 10 && preys.Count < maxEntities * 0.7 && (predators.Count + preys.Count) < maxEntities)
                    foreach ((int dx, int dy) in directions)
                    {
                        int newX = x + dx;
                        int newY = y + dy;
                        if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize
                            && newEntities[newX, newY] == 0)
                        {
                            preysToCreate.Add(new Prey(newX, newY));
                            newEntities[newX, newY] = -1;
                            prey.satDur = 0;
                            break;
                        }
                    }

                if (!isMoving)
                {
                    preysToRemove.Add(prey);
                    g.FillRectangle(Brushes.White, 2 + prey.XPos * 20, 2 + prey.YPos * 20, 17, 17);
                    newEntities[prey.XPos, prey.YPos] = 0;
                }
            }

            foreach (Prey prey in preysToRemove)
                preys.Remove(prey);

            foreach (Predator predator in predatorsToRemove)
                predators.Remove(predator);

            foreach (Predator predator in predatorsToCreate)
                predators.Add(predator);

            foreach (Prey prey in preysToCreate)
                preys.Add(prey);


            entities = newEntities;


            foreach (Predator predator in predators)
            {
                g.FillRectangle(Brushes.Coral, 2 + predator.XPos * 20, 2 + predator.YPos * 20, 17, 17);
                continue;
            }
            foreach (Prey prey in preys)
            {
                g.FillRectangle(Brushes.ForestGreen, 2 + prey.XPos * 20, 2 + prey.YPos * 20, 17, 17);
                continue;
            }

            if (predators.Count == 0 || (checkBox1.Checked && int.Parse(textBox1.Text) >= prevTime))
            {
                button3.Enabled = false;
                timer1.Enabled = false;
                timer2.Enabled = false;
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

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        int prevTime = 99;
        private void timer2_Tick(object sender, EventArgs e)
        {
            int newTime = DateTime.Now.Second;
            if (prevTime != newTime)
            {
                textBox2.Text = (int.Parse(textBox2.Text) + 1).ToString();
                prevTime = newTime;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
           if (timer1.Enabled)
            {
                timer1.Enabled = false;
                timer2.Enabled = false;
            }
           else
            {
                timer1.Enabled = true;
                timer2.Enabled = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                textBox1.Enabled = true;
            else textBox1.Enabled = false;
        }

        private void textBoxPred_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool isBackspace = (int)e.KeyChar == 8;
            bool isNumber = (e.KeyChar >= '0' && e.KeyChar <= '9');
            if (!isBackspace && !isNumber)
                e.KeyChar = (char)0;
        }

        private void textBoxPrey_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool isBackspace = (int)e.KeyChar == 8;
            bool isNumber = (e.KeyChar >= '0' && e.KeyChar <= '9');
            if (!isBackspace && !isNumber)
                e.KeyChar = (char)0;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool isBackspace = (int)e.KeyChar == 8;
            bool isNumber = (e.KeyChar >= '0' && e.KeyChar <= '9');
            if (!isBackspace && !isNumber)
                e.KeyChar = (char)0;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
