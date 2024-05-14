using MysteryWorld.Models;
using System;
using System.Windows;
using System.Windows.Threading;

namespace MysteryWorld
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            timer.Tick += GameLoop;
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Start();
        }

        private void GameLoop(object sender, EventArgs args)
        {
            if (level.GoLeft)
            {
                level.MoveLeft();
            }

            if (level.GoRight)
            {
                level.MoveRight();
            }

            if (level.GoUp)
            {
                level.MoveUp();
            }

            if (level.GoDown)
            {
                level.MoveDown();
            }
        }
    }
}
