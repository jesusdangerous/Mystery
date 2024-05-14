using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MysteryWorld.Models;

namespace MysteryWorld.Views
{
    public partial class LevelView : UserControl
    {
        const int SIDE_WIDTH = 50;

        private Level _level = new Level();
        private Rectangle _player = new Rectangle();
        private List<Rectangle> _boxes = new List<Rectangle>();
        private int _levelNumber = 1;

        public bool GoLeft { get; set; } = false;
        public bool GoRight { get; set; } = false;
        public bool GoUp { get; set; } = false;
        public bool GoDown { get; set; } = false;

        public LevelView()
        {
            InitializeComponent();

            LoadLevel(_levelNumber);
        }

        private void LoadNextLevel()
        {
            _levelNumber++;
            LoadLevel(_levelNumber);
        }

        private void LoadLevel(int levelNumber)
        {
            drawCanvas.Children.Clear();

            _level.Load(levelNumber);

            SetWalls();
            SetPlaces();
            SetBoxes();
            SetPlayer();
        }

        private void SetWalls()
        {
            for (int row = 0; row < _level.Height; row++)
            {
                for (int col = 0; col < _level.Width; col++)
                {
                    int index = (row * _level.Width) + col;

                    // внутри: 166 64 30
                    // стены:  122 51 0

                    if (_level.Cells[index] == MapObjectType.Barrier)
                    {
                        var rect = new Rectangle();
                        rect.Fill = new SolidColorBrush(Color.FromRgb(122, 51, 0));
                        rect.Width = SIDE_WIDTH;
                        rect.Height = SIDE_WIDTH;

                        drawCanvas.Children.Add(rect);

                        Canvas.SetLeft(rect, SIDE_WIDTH + col * SIDE_WIDTH);
                        Canvas.SetTop(rect, SIDE_WIDTH + row * SIDE_WIDTH);
                    }
                }
            }
        }

        private void SetPlaces()
        {
            for (int row = 0; row < _level.Height; row++)
            {
                for (int col = 0; col < _level.Width; col++)
                {
                    int index = (row * _level.Width) + col;

                    if (_level.Cells[index] == MapObjectType.Marker)
                    {
                        var rect = new Rectangle();
                        var source = new BitmapImage(new Uri("pack://application:,,,/Assets/place.png"));

                        rect.Fill = new ImageBrush(source);
                        rect.Width = SIDE_WIDTH;
                        rect.Height = SIDE_WIDTH;

                        drawCanvas.Children.Add(rect);

                        Canvas.SetLeft(rect, SIDE_WIDTH + col * SIDE_WIDTH);
                        Canvas.SetTop(rect, SIDE_WIDTH + row * SIDE_WIDTH);
                    }
                }
            }
        }

        private void SetBoxes()
        {
            _boxes.Clear();
            for (int row = 0; row < _level.Height; row++)
            {
                for (int col = 0; col < _level.Width; col++)
                {
                    int index = (row * _level.Width) + col;

                    if (_level.Cells[index] == MapObjectType.Crate)
                    {
                        var rect = new Rectangle();

                        var source = new BitmapImage(new Uri("pack://application:,,,/Assets/box.png"));

                        rect.Fill = new ImageBrush(source);
                        rect.Width = SIDE_WIDTH;
                        rect.Height = SIDE_WIDTH;

                        drawCanvas.Children.Add(rect);

                        Canvas.SetLeft(rect, SIDE_WIDTH + col * SIDE_WIDTH);
                        Canvas.SetTop(rect, SIDE_WIDTH + row * SIDE_WIDTH);

                        _boxes.Add(rect);
                    }
                }
            }
        }

        private void SetPlayer()
        {
            for (int row = 0; row < _level.Height; row++)
            {
                for (int col = 0; col < _level.Width; col++)
                {
                    int index = (row * _level.Width) + col;

                    if (_level.Cells[index] == MapObjectType.Avatar)
                    {
                        var rect = new Rectangle();

                        var source = new BitmapImage(new Uri("pack://application:,,,/Assets/player.png"));

                        rect.Fill = new ImageBrush(source);
                        rect.Width = SIDE_WIDTH;
                        rect.Height = SIDE_WIDTH;

                        drawCanvas.Children.Add(rect);

                        Canvas.SetLeft(rect, SIDE_WIDTH + col * SIDE_WIDTH);
                        Canvas.SetTop(rect, SIDE_WIDTH + row * SIDE_WIDTH);

                        _player = rect;
                    }
                }
            }
        }

        public void MoveLeft()
        {
            if (_level.SetPlayer(-1, 0))
            {
                Canvas.SetLeft(_player, Math.Max(Canvas.GetLeft(_player) - SIDE_WIDTH, 0));
                UpdateBoxes();
                if (_level.CheckLevelComplete())
                {
                    LoadNextLevel();
                }
            }
            GoLeft = false;
        }

        public void MoveRight()
        {
            if (_level.SetPlayer(1, 0))
            {
                Canvas.SetLeft(_player, Canvas.GetLeft(_player) + SIDE_WIDTH);
                UpdateBoxes();
                if (_level.CheckLevelComplete())
                {
                    LoadNextLevel();
                }
            }
            GoRight = false;
        }

        public void MoveUp()
        {
            if (_level.SetPlayer(0, -1))
            {
                Canvas.SetTop(_player, Math.Max(Canvas.GetTop(_player) - SIDE_WIDTH, 0));
                UpdateBoxes();
                if (_level.CheckLevelComplete())
                {
                    LoadNextLevel();
                }
            }
            GoUp = false;
        }

        public void MoveDown()
        {
            if (_level.SetPlayer(0, 1))
            {
                Canvas.SetTop(_player, Canvas.GetTop(_player) + SIDE_WIDTH);
                UpdateBoxes();
                if (_level.CheckLevelComplete())
                {
                    LoadNextLevel();
                }
            }
            GoDown = false;
        }

        private void UpdateBoxes()
        {
            _boxes.ForEach(b => {
                Box found = _level.Boxes.Find(box => {
                    return
                    box.X == Canvas.GetLeft(b) / SIDE_WIDTH - 1 &&
                    box.Y == Canvas.GetTop(b) / SIDE_WIDTH - 1;
                });
                if (found.X != 0 && found.Moved)
                {
                    found.Move();
                    Canvas.SetLeft(b, SIDE_WIDTH + found.X * SIDE_WIDTH);
                    Canvas.SetTop(b, SIDE_WIDTH + found.Y * SIDE_WIDTH);
                }
            });
        }

        private void drawCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.A)
            {
                GoLeft = true;
            }

            if (e.Key == Key.Right || e.Key == Key.D)
            {
                GoRight = true;
            }

            if (e.Key == Key.Up || e.Key == Key.W)
            {
                GoUp = true;
            }

            if (e.Key == Key.Down || e.Key == Key.S)
            {
                GoDown = true;
            }
        }

        private void drawCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.A)
            {
                GoLeft = false;
            }

            if (e.Key == Key.Right || e.Key == Key.D)
            {
                GoRight = false;
            }

            if (e.Key == Key.Up || e.Key == Key.W)
            {
                GoUp = false;
            }

            if (e.Key == Key.Down || e.Key == Key.S)
            {
                GoDown = false;
            }
        }
    }
}
