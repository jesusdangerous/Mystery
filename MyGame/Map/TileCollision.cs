using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Map
{
    public class TileCollision
    {
        public int XPos { get; set; }
        public int YPos { get; set; }

        public Rectangle Rectangle { get { return new Rectangle(XPos * 16, YPos * 16, 16, 16); } }

        public bool Intersect(Rectangle rectangle)
        {
            return Rectangle.Intersects(rectangle);
        }

        public TileCollision()
        {

        }
    }
}
