namespace Project
{
    public partial class Form1 : Form
    {
        int gravity;
        int gravityValue = 8;
        int obstaleSpeed = 10;
        int score = 0;
        int highScore = 0;
        bool gameOver = false;
        Random random = new Random();


        public Form1()
        {
            InitializeComponent();
            RestartGame();
        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            Score.Text = score + "ì";
            HighScore.Text = "ÐÅÊÎÐÄ:" + highScore + "ì";
            player.Top += gravity;

            if (player.Top > 526)
            {
                gravity = 0;
                player.Top = 526;
                player.Image = Properties.Resources.runDown;
            }
            else if (player.Top < 47)
            {
                gravity = 0;
                player.Top = 47;
                player.Image= Properties.Resources.runUp;
            }

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "obstacle")
                {
                    x.Left -= obstaleSpeed;

                    if (x.Left < 100)
                    {
                        x.Left = random.Next(1200, 3000);
                        score += 1;
                    }

                    if (x.Bounds.IntersectsWith(player.Bounds))
                    {
                        gameTimer.Stop();
                        Score.Text += "  Game Over!! Press Enter to Restart.";
                        gameOver = true;

                        if (score > highScore)
                        {
                            highScore = score;
                        }
                    }
                }
            }

            if (score > 10)
            {
                obstaleSpeed = 20;
                gravityValue = 12;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                if (player.Top == 526)
                {
                    player.Top -= 10;
                    gravity = -gravityValue;
                }
                else if (player.Top == 47)
                {
                    player.Top += 10;
                    gravity = gravityValue;
                }
            }

            if (e.KeyCode == Keys.Enter && gameOver == true)
            {
                RestartGame();
            }
        }

        private void RestartGame()
        {
            Score.Parent = Sealing;
            HighScore.Parent = Floor;
            HighScore.Top = 0;
            player.Location = new(180, 149);
            player.Image = Properties.Resources.runDown;
            score = 0;
            gravityValue = 8;
            gravity = gravityValue;
            obstaleSpeed = 10;

            foreach (Control x in Controls)
            {
                if (x is PictureBox && (string)x.Tag == "obstacle")
                {
                    x.Left = random.Next(1200, 3000);
                }
            }

            gameTimer.Start();
        }
    }
}
