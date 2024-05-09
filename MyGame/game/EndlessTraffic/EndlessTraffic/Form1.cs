namespace EndlessTraffic
{
    public partial class Form1 : Form
    {
        //global variables
        int gravity;
        int gravityValue;
        int obstaleSpeed = 10;
        int score;
        int highScore;
        bool gameOver = false;
        Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            RestartGame();
        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            IblScore.Text = "Score: " + score;
            IblhighScore.Text = "High Score: " + highScore;
            player.Top += gravity;

            //when the player land on the platforms
            if (player.Top > 322)
            {
                gravity = 0;
                player.Top = 322;
                player.Image = Properties.Resources.run_down0;
            }
            else if (player.Top < 57)
            {
                gravity = 0;
                player.Top = 57;
                player.Image = Properties.Resources.run_up0;
            }

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "obstacle")
                {
                    x.Left -= obstaleSpeed;

                    if (x.Left < -100)
                    {
                        x.Left = random.Next(1200, 3000);
                        score += 1;
                    }

                    if (x.Bounds.IntersectsWith(player.Bounds))
                    {
                        gameTimer.Stop();
                        IblScore.Text += "  Game Over!! Press Enter to Restart.";
                        gameOver = true;


                        if (score > highScore)
                        {
                            highScore = score;
                        }
                    }
                }
            }

            if (score > 4)
            {
                obstaleSpeed = 20;
                gravityValue = 12;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                if (player.Top == 322)
                {
                    player.Top -= 10;
                    gravity = -gravityValue;
                }
                else if (player.Top == 57)
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
            IblScore.Parent = pictureBox1;
            IblhighScore.Parent = pictureBox2;

            IblhighScore.Top = 0;
            player.Location = new Point(180, 149);
            player.Image = Properties.Resources.run_down0;
            score = 0;
            gravityValue = 8;
            gravity = gravityValue;
            obstaleSpeed = 10;

            foreach (Control x in this.Controls)
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
