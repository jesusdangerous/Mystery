﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Components;


namespace MyGame.Map
{
    public class Entities
    {
        private List<BaseObject> _entities;

        public Entities()
        {
            _entities = new List<BaseObject>();
        }

        public void AddEntity(BaseObject newObject)
        {
            _entities.Add(newObject);
        }

        public void Update(double gameTime)
        {
            var i = 0;
            while (i < _entities.Count)
            {
                _entities[i].Update(gameTime);
                if (_entities[i].Kill)
                    _entities.RemoveAt(i);
                else
                    i++;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var baseObject in _entities)
            {
                baseObject.Draw(spriteBatch);
            }
        }

        public bool CheckCollision(Rectangle rectangle, out Animation outAnimation, out BaseObject outBaseObject, string id, bool checkOnlyHostile = false)
        {
            outAnimation = null;
            outBaseObject = null;


            foreach (var baseObject in _entities)
            {
                if (baseObject == null)
                    continue;

                if (baseObject.Id != null && baseObject.Id == id)
                    continue;

                var sprite = baseObject.GetComponent<Sprite>(ComponentType.Sprite);
                if (sprite == null)
                    continue;
                if (sprite.Rectangle.Intersects(rectangle))
                {
                    if (checkOnlyHostile)
                        if (!baseObject.Hostile)
                            continue;
                    outAnimation = baseObject.GetComponent<Animation>(ComponentType.Animation);
                    outBaseObject = baseObject;
                    return true;
                }
            }
            return false;
        }

        public bool CheckCollision(Rectangle rectangle, string ownerId)
        {
            if ((string.IsNullOrWhiteSpace(ownerId)) || !ownerId.Equals("player"))
            {
                var player = _entities.FirstOrDefault(e => e.Id.Equals("player"));
                if (player == null)
                    return false;
                return rectangle.Intersects(player.GetComponent<Sprite>(ComponentType.Sprite).Rectangle);
            }

            foreach (var entity in _entities)
            {
                if (!string.IsNullOrWhiteSpace(ownerId) && entity.Id != null && entity.Id.Equals("player"))
                    continue;

                if (entity.GetComponent<Sprite>(ComponentType.Sprite).Rectangle.Intersects(rectangle))
                    return true;
            }

            return false;
        }

        public void Initialize()
        {
            if (_entities == null)
                return;

            _entities.ForEach(e => e.Initialize());
        }

        public void Uninitialize()
        {
            if (_entities == null)
                return;

            _entities.ForEach(e => e.Uninitialize());
        }
    }
}