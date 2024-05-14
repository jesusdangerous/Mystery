using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Components
{
    class Animation : Component
    {
        public override ComponentType ComponentType
        {
            get { return ComponentType.Animation;  }
        }

        private int _width;
        private int _height;
        public Rectangle TextureRectangle { get; private set; }
        public Direction _currentDirection { get; set; }
        private State _currentState;
        private double _counter;
        private int _animationIndex;

        public Animation(int width, int  height)
        {
            _width = width;
            _height = height;
            _counter = 0;
            _animationIndex = 0;
            _currentState = State.Standing;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            
        }

        public override void Update(double gameTime)
        {
            switch (_currentState)
            {
                case State.Walking:
                    _counter += gameTime;
                    if (_counter > 200)
                    {
                        ChangeState();
                        _counter = 0;
                    }
                break;
            }
        }

        public void ResetComputer(State state, Direction direction)
        {
            if (state != _currentState) //_currentDirection != direction - анимация, если есть несколько положений
            {
                _counter = 1000;
                _animationIndex = 0;
            }

            _currentState = state;
            _currentDirection = direction;
        }

        private void ChangeState()
        {
            switch (_currentDirection)
            {
                case Direction.Down:
                    TextureRectangle = new Rectangle(_width * _animationIndex, 0, _width, _height);
                    break;
                case Direction.Up:
                    TextureRectangle = new Rectangle(_width * _animationIndex + 17, 0, _width, _height);
                    break;
                case Direction.Left:
                    TextureRectangle = new Rectangle(_width * _animationIndex + 34, 0, _width, _height);
                    break;
                case Direction.Right:
                    TextureRectangle = new Rectangle(_width * _animationIndex + 51, 0, _width, _height);
                    break;
            }

            _animationIndex = _animationIndex == 0 ? 1 : 0;
            _currentState = State.Standing; //если убрать - анимация, если есть несколько положений
        }
    }
}
