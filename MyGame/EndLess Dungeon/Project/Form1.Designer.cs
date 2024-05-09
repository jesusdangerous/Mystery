namespace Project
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            Floor = new PictureBox();
            Sealing = new PictureBox();
            Score = new Label();
            HighScore = new Label();
            Coins = new Label();
            player = new PictureBox();
            ObstacleColumn = new PictureBox();
            ObstaclePlatform = new PictureBox();
            Ghost = new PictureBox();
            gameTimer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)Floor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Sealing).BeginInit();
            ((System.ComponentModel.ISupportInitialize)player).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ObstacleColumn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ObstaclePlatform).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Ghost).BeginInit();
            SuspendLayout();
            // 
            // Floor
            // 
            Floor.BackgroundImage = Properties.Resources.floorNew;
            Floor.Dock = DockStyle.Bottom;
            Floor.Location = new Point(0, 613);
            Floor.MaximumSize = new Size(1920, 90);
            Floor.MinimumSize = new Size(1920, 60);
            Floor.Name = "Floor";
            Floor.Size = new Size(1920, 60);
            Floor.SizeMode = PictureBoxSizeMode.StretchImage;
            Floor.TabIndex = 0;
            Floor.TabStop = false;
            // 
            // Sealing
            // 
            Sealing.BackgroundImage = Properties.Resources.sealingNew;
            Sealing.Dock = DockStyle.Top;
            Sealing.Location = new Point(0, 0);
            Sealing.MaximumSize = new Size(1920, 90);
            Sealing.MinimumSize = new Size(1920, 60);
            Sealing.Name = "Sealing";
            Sealing.Size = new Size(1920, 60);
            Sealing.TabIndex = 1;
            Sealing.TabStop = false;
            // 
            // Score
            // 
            Score.AutoSize = true;
            Score.BackColor = Color.Transparent;
            Score.Font = new Font("Castellar", 12.2F, FontStyle.Bold);
            Score.ForeColor = Color.IndianRed;
            Score.Location = new Point(12, 9);
            Score.Name = "Score";
            Score.Size = new Size(92, 25);
            Score.TabIndex = 2;
            Score.Tag = "LabelScore";
            Score.Text = "0000м";
            // 
            // HighScore
            // 
            HighScore.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            HighScore.AutoSize = true;
            HighScore.BackColor = Color.Transparent;
            HighScore.Font = new Font("Castellar", 7.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            HighScore.ForeColor = Color.FromArgb(128, 64, 64);
            HighScore.Location = new Point(12, 648);
            HighScore.Name = "HighScore";
            HighScore.Size = new Size(129, 16);
            HighScore.TabIndex = 3;
            HighScore.Tag = "LabelHighScore";
            HighScore.Text = "РЕКОРД: 0000м";
            // 
            // Coins
            // 
            Coins.AutoSize = true;
            Coins.BackColor = Color.Transparent;
            Coins.Font = new Font("Castellar", 6.2F, FontStyle.Bold);
            Coins.ForeColor = Color.FromArgb(128, 64, 64);
            Coins.Location = new Point(27, 34);
            Coins.Name = "Coins";
            Coins.Size = new Size(51, 13);
            Coins.TabIndex = 4;
            Coins.Tag = "LabelCoins";
            Coins.Text = "000💎 ";
            // 
            // player
            // 
            player.BackColor = Color.Transparent;
            player.Image = Properties.Resources.runUp;
            player.Location = new Point(374, 47);
            player.Name = "player";
            player.Size = new Size(77, 102);
            player.SizeMode = PictureBoxSizeMode.StretchImage;
            player.TabIndex = 5;
            player.TabStop = false;
            // 
            // ObstacleColumn
            // 
            ObstacleColumn.Anchor = AnchorStyles.Bottom;
            ObstacleColumn.BackColor = Color.Transparent;
            ObstacleColumn.Image = Properties.Resources.obstacleColumn;
            ObstacleColumn.Location = new Point(638, 471);
            ObstacleColumn.Name = "ObstacleColumn";
            ObstacleColumn.Size = new Size(67, 142);
            ObstacleColumn.SizeMode = PictureBoxSizeMode.StretchImage;
            ObstacleColumn.TabIndex = 6;
            ObstacleColumn.TabStop = false;
            ObstacleColumn.Tag = "obstacle";
            // 
            // ObstaclePlatform
            // 
            ObstaclePlatform.BackColor = Color.Transparent;
            ObstaclePlatform.Image = Properties.Resources.obstaclePlatform;
            ObstaclePlatform.Location = new Point(800, 320);
            ObstaclePlatform.Name = "ObstaclePlatform";
            ObstaclePlatform.Size = new Size(213, 56);
            ObstaclePlatform.SizeMode = PictureBoxSizeMode.StretchImage;
            ObstaclePlatform.TabIndex = 7;
            ObstaclePlatform.TabStop = false;
            ObstaclePlatform.Tag = "obstacle";
            // 
            // Ghost
            // 
            Ghost.BackColor = Color.Transparent;
            Ghost.Image = Properties.Resources.ghost;
            Ghost.Location = new Point(829, 59);
            Ghost.Name = "Ghost";
            Ghost.Size = new Size(102, 122);
            Ghost.SizeMode = PictureBoxSizeMode.StretchImage;
            Ghost.TabIndex = 8;
            Ghost.TabStop = false;
            Ghost.Tag = "obstacle";
            // 
            // gameTimer
            // 
            gameTimer.Enabled = true;
            gameTimer.Interval = 20;
            gameTimer.Tick += GameTimerEvent;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1262, 673);
            Controls.Add(Coins);
            Controls.Add(HighScore);
            Controls.Add(Score);
            Controls.Add(Sealing);
            Controls.Add(Floor);
            Controls.Add(ObstacleColumn);
            Controls.Add(ObstaclePlatform);
            Controls.Add(Ghost);
            Controls.Add(player);
            DoubleBuffered = true;
            MaximumSize = new Size(1920, 1080);
            MinimumSize = new Size(1280, 720);
            Name = "Form1";
            Text = "Endless Dungeon";
            TopMost = true;
            KeyUp += KeyIsUp;
            ((System.ComponentModel.ISupportInitialize)Floor).EndInit();
            ((System.ComponentModel.ISupportInitialize)Sealing).EndInit();
            ((System.ComponentModel.ISupportInitialize)player).EndInit();
            ((System.ComponentModel.ISupportInitialize)ObstacleColumn).EndInit();
            ((System.ComponentModel.ISupportInitialize)ObstaclePlatform).EndInit();
            ((System.ComponentModel.ISupportInitialize)Ghost).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox Floor;
        private PictureBox Sealing;
        private Label Score;
        private Label HighScore;
        private Label Coins;
        private PictureBox player;
        private PictureBox ObstacleColumn;
        private PictureBox ObstaclePlatform;
        private PictureBox Ghost;
        private System.Windows.Forms.Timer gameTimer;
    }
}
