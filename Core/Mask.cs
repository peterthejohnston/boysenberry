using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public class Mask
    {
        public Rectangle shape;
        public Vector2 origin = Vector2.Zero;

        public int width
        {
            get { return shape.Width; }
        }
        public int height
        {
            get { return shape.Height; }
        }

        public Mask(Texture2D texture)
        {
            this.shape = new Rectangle(0, 0, texture.Width, texture.Height);
        }
        public Mask(Texture2D texture, Vector2 origin)
        {
            this.shape = new Rectangle(0, 0, texture.Width, texture.Height);
            this.origin = origin;
        }

        public void CenterOrigin()
        {
            origin = new Vector2(width / 2, height / 2);
        }
    }
}
