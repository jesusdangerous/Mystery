using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using MysteryWorld.Views;
using MysteryWorld.Controllers;

namespace MysteryWorld.Models
{
    public sealed class SafeGameObjectTreeModel
    {
        private const float Divider = 2f;
        private const int Quad = 4;

        private Rect Area;
        private int Level;
        private const int MaxLevel = 12;
        private List<GameObjectView> Objects;
        private List<Rect> ChildAreas;
        private List<SafeGameObjectTreeModel> Children;

        private SafeGameObjectTreeModel()
        {
        }

        public static SafeGameObjectTreeModel CreateQuadTree(Rect rect, int level = 1)
        {
            var quadTree = new SafeGameObjectTreeModel();
            quadTree.Area = rect;
            quadTree.Level = level;
            quadTree.Objects = new List<GameObjectView>();
            quadTree.ChildAreas = new List<Rect>
            {
                new(quadTree.Area.Position, Vector2.Divide(quadTree.Area.Size, Divider)),
                new(new Vector2(quadTree.Area.Position.X + quadTree.Area.Size.X / Divider, quadTree.Area.Position.Y), Vector2.Divide(quadTree.Area.Size, Divider)),
                new(new Vector2(quadTree.Area.Position.X, quadTree.Area.Position.Y + quadTree.Area.Size.Y / Divider), Vector2.Divide(quadTree.Area.Size, Divider)),
                new(new Vector2(quadTree.Area.Position.X + quadTree.Area.Size.X / Divider, quadTree.Area.Position.Y + quadTree.Area.Size.Y / Divider), Vector2.Divide(quadTree.Area.Size, Divider))
            };
            quadTree.Children = new List<SafeGameObjectTreeModel>(Quad) { null, null, null, null };
            return quadTree;
        }

        internal void Clear()
        {
            Objects.Clear();
            for (var i = 0; i < Quad; i++)
                if (Children[i] != null)
                {
                    Children[i].Clear();
                    Children[i] = null;
                }
        }

        internal void Insert(GameObjectView go)
        {
            var objArea = new Rect(go.Hitbox);
            InsertHelper(go, objArea);
        }

        private void InsertHelper(GameObjectView go, Rect area)
        {
            if (Level >= MaxLevel)
            {
                Objects.Add(go);
                return;
            }
            for (var i = 0; i < Quad; i++)
            {
                if (!ChildAreas[i].Contains(area))
                    continue;

                Children[i] ??= CreateQuadTree(ChildAreas[i], Level + 1);
                Children[i].InsertHelper(go, area);
                return;
            }
            Objects.Add(go);
        }

        internal List<CharacterController> SearchCharacters(Rect area) =>
            Search(area).OfType<CharacterController>().ToList();

        internal List<CharacterController> PointSearchCharacters(Vector2 position) =>
            Search(new Rect(position, new Vector2(1, 1))).OfType<CharacterController>().ToList();

        internal List<GameObjectView> Search(Rectangle hitbox) =>
            Search(new Rect(hitbox));

        public List<CharacterController> SearchCharacters(Rectangle hitbox) =>
            Search(new Rect(hitbox)).OfType<CharacterController>().ToList();

        internal List<ItemModel> SearchItems(Rectangle hitbox) =>
            Search(new Rect(hitbox)).OfType<ItemModel>().ToList();

        private List<GameObjectView> Search(Rect area)
        {
            var outList = new List<GameObjectView>();
            Search(area, outList);
            return outList;
        }

        private void Search(Rect area, List<GameObjectView> outList)
        {
            foreach (var obj in Objects)
                if (area.Overlaps(new Rect(new Vector2(obj.Hitbox.X, obj.Hitbox.Y), new Vector2(obj.Hitbox.Width, obj.Hitbox.Height))))
                    outList.Add(obj);

            for (var i = 0; i < Quad; i++)
            {
                if (Children[i] == null) continue;
                if (area.Contains(ChildAreas[i]))
                    Children[i].FeedItems(outList);
                else if (area.Overlaps(ChildAreas[i]))
                    Children[i].Search(area, outList);
            }
        }

        private void FeedItems(List<GameObjectView> outList)
        {
            outList.AddRange(Objects);
            for (var i = 0; i < Quad; i++)
            {
                if (Children[i] == null) continue;
                Children[i].FeedItems(outList);
            }
        }

        internal void Draw(SpriteBatch spriteBatch, Rectangle visibleArea)
        {
            foreach (var obj in Search(new Rect(new Vector2(visibleArea.Left, visibleArea.Top), new Vector2(visibleArea.Width, visibleArea.Height))))
                obj.Draw(spriteBatch);
        }
    }
}