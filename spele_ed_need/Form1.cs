using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace spele_ed_need
{
    public partial class Form1 : Form
    {


        private int L = 30;
        private int Lxp = 400, Lyp = 400;
        private int score = 0;
        private Random R = new Random();
        private int objectCount = 10;
        private int currentLevel = 1;
        private int objectSpeed = 5;
        private bool levelUpTriggered = false;

        private int correctLeftClickCount = 0;
        private int correctRightClickCount = 0;
        private int wrongLeftClickCount = 0;
        private int wrongRightClickCount = 0;
        private int fallenObjectCount = 0;
        private int gameTime = 0;

        public Form1()
        {
            InitializeComponent();
            panel1.Width = Lxp;
            panel1.Height = Lyp;
            panel2.Left = Lxp + 30;

            timer1.Enabled = false;
            timer2.Enabled = false;

            MessageBox.Show(
                "Instructions:\n" +
                "1. Click on the edible items with the left mouse button.\n" +
                "2. Right-click on the inedible items.\n" +
                "3. Points:\n" + 
                "   +10 for a correct click.\n" + 
                "   -10 for a mistake.\n" +
                "4. Level up:\n" + "   Reach 50 points to advance to the next level.\n" +
                "5. Game ends:\n" + "   After 30 seconds or if your score falls below -50.\n" +
                "Final Score = Base Score + (Correct Clicks x Level x 2) - (Mistakes x 5).\n" +
                "Press 'Start' to begin the game!",
                "Game Instructions",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            score = 0;

            correctLeftClickCount = 0;
            correctRightClickCount = 0;
            wrongLeftClickCount = 0;
            wrongRightClickCount = 0;
            fallenObjectCount = 0;
            gameTime = 0;

            UpdateStatistics();

            int level = (int)numericUpDown1.Value;

            switch (level)
            {
                case 1:
                    objectCount = 10;
                    break;
                case 2:
                    objectCount = 15;
                    break;
                case 3:
                    objectCount = 20;
                    break;
                default:
                    objectCount = 10;
                    break;
            }

            for (int i = 0; i < objectCount; i++)
            {
                CreateFallingObject();
            }

            timer1.Enabled = true;
            timer2.Enabled = true;
        }


        private void CreateFallingObject()
        {
            PictureBox obj = new PictureBox
            {
                Width = L * 13 / 10,
                Height = L * 13 / 10,
                Left = R.Next(0, Lxp - L),
                Top = R.Next(-Lyp, 0),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            int r = R.Next(0, 100);

            if (r < 50)
            {
                obj.ImageLocation = @"Resources\f" + R.Next(1, 6) + ".png";
                obj.Tag = "Y";
            }
            else
            {
                obj.ImageLocation = @"Resources\a" + R.Next(1, 6) + ".png";
                obj.Tag = "N";
            }

            obj.MouseDown += new MouseEventHandler(p_MouseDown);
            panel1.Controls.Add(obj);
        }


        private void p_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is PictureBox s)
            {
                if (e.Button == MouseButtons.Left && s.Tag.ToString() == "Y")
                {
                    score += 10;
                    correctLeftClickCount++;
                    PlaySound("correct_click.wav");
                    s.BackColor = Color.Green;
                }
                else if (e.Button == MouseButtons.Right && s.Tag.ToString() == "N")
                {
                    score += 10;
                    correctRightClickCount++;
                    PlaySound("correct_click.wav");
                    s.BackColor = Color.Blue;
                }
                else
                {
                    score -= 10;
                    if (e.Button == MouseButtons.Left) wrongLeftClickCount++;
                    else if (e.Button == MouseButtons.Right) wrongRightClickCount++;
                    PlaySound("wrong_click.wav");
                    s.BackColor = Color.Red;
                }

                Timer disappearTimer = new Timer
                {
                    Interval = 200
                };
                disappearTimer.Tick += (o, args) =>
                {
                    disappearTimer.Stop();
                    panel1.Controls.Remove(s);
                };
                disappearTimer.Start();

                UpdateStatistics();
            }
        }


        private void PlaySound(string fileName)
        {
            try
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(fileName);
                player.Play();
            }
            catch
            {
                MessageBox.Show("Sound file not found: " + fileName);
            }
        }


        private void UpdateStatistics()
        {
            label2.Text = $"Score: {score}"; 
            label5.Text = $"Correct Left Clicks: {correctLeftClickCount}";
            label6.Text = $"Correct Right Clicks: {correctRightClickCount}";
            label7.Text = $"Wrong Left Clicks: {wrongLeftClickCount}";
            label8.Text = $"Wrong Right Clicks: {wrongRightClickCount}";
            label9.Text = $"Fallen Objects: {fallenObjectCount}";
            label10.Text = $"Time: {gameTime}s";
        }


        private void CheckLevelProgress()
        {
            if (score >= 50 * currentLevel && currentLevel < 3 && !levelUpTriggered)
            {
                levelUpTriggered = true;

                currentLevel++;
                numericUpDown1.Value = currentLevel;

                MessageBox.Show($"Level up! New level: {currentLevel}");

                objectCount += 5;
                objectSpeed += 2;

                panel1.Controls.Clear();
                for (int i = 0; i < objectCount; i++)
                {
                    CreateFallingObject();
                }
            }

            if (score < 50 * currentLevel)
            {
                levelUpTriggered = false;
            }
        }


        private void CalculateFinalScore()
        {
            double finalScore = score
            + (correctLeftClickCount + correctRightClickCount) * (int)numericUpDown1.Value * 2
            - (wrongLeftClickCount + wrongRightClickCount + fallenObjectCount) * 5;

            MessageBox.Show($"Your final score: {finalScore:F2}");
        }


        private void EndGame()
        {
            timer1.Enabled = false;
            timer2.Enabled = false;

            CalculateFinalScore();

            MessageBox.Show("Game Over! Thanks for playing!");
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                MessageBox.Show("Author: Samanta Rukmane");
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            gameTime++;
            UpdateStatistics();

            CheckLevelProgress();

            if (gameTime >= 30 || score < -50)
            {
                EndGame();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (Control c in panel1.Controls)
            {
                if (c is PictureBox obj)
                {
                    obj.Top += 5;

                    if (obj.Top > Lyp)
                    {
                        fallenObjectCount++;
                        score -= 10;

                        UpdateStatistics();

                        obj.Top = R.Next(-Lyp, 0);
                        obj.Left = R.Next(0, Lxp - L);
                    }
                }
            }
        }



    }
}
