﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Components;

namespace MyGame.Components.DeathAnimations
{
    class DeathAnimationExplosion : Component
    {
        private enum DeathAnimationState
        {
            Initialize,
            Check
        };

        private DeathAnimationState _currentState;

        public DeathAnimationExplosion()
        {
            _currentState = DeathAnimationState.Initialize;
        }

        public override ComponentType ComponentType
        {
            get { return ComponentType.DeathAnimation; }
        }

        public override void Update(double gameTime)
        {
            var animation = GetComponent<Animation>(ComponentType.Animation);
            if (animation == null)
            {
                Done();
                return;
            }

            if (_currentState == DeathAnimationState.Initialize)
            {
                animation.PlayAnimation(State.Walking, Direction.Down);
                _currentState = DeathAnimationState.Check;
            }
            else
            {
                if (animation.CurrentState == State.Standing)
                {
                    Done();
                }
            }
        }

        public void Done()
        {
            KillBaseObject();
        }

        public override void Draw(SpriteBatch spritebatch)
        {

        }
    }
}