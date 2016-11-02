using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public class Sprite
    {
        public Texture2D sprite;
        private Vector2 _origin = Vector2.Zero;
        public Vector2 origin
        {
            get { return _origin; }
            set
            {
                _origin = value;
                // mask.origin = value;
            }
        }
        /*private Mask _mask;
        public Mask mask
        {
            get { return _mask; }
            set
            {
                _mask = value;
                _mask.origin = origin;
            }
        }*/
        public int frames = 1;
        public int framewidth;
        public float speed;
        public Color color = Color.White;
        public float rotation;
        public float xscale = 1f;
        public float yscale = 1f;
        public float scale
        {
            set
            {
                xscale = value;
                yscale = value;
            }
        }
        public float alpha = 1f;

        public float currentFrame = 0;

        public int width { get { return framewidth; } }
        public int height { get { return sprite.Height; } }

        #region Constructors

        public Sprite(Texture2D sprite)
        {
            this.sprite = sprite;
            framewidth = this.sprite.Width;
        }
        public Sprite(Texture2D sprite, float speed, int frames)
        {
            this.sprite = sprite;
            this.speed = speed;
            this.frames = frames;
            framewidth = this.sprite.Width / frames;
        }
        public Sprite(Texture2D sprite, float speed, int framewidth, int frames)
        {
            this.sprite = sprite;
            this.speed = speed;
            this.framewidth = framewidth;
            if (frames < 0)
                this.frames = this.sprite.Width / framewidth + frames;
            else this.frames = frames;
        }

        #endregion

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            // vv ???
            position.X = (float)Math.Round(position.X);
            position.Y = (float)Math.Round(position.Y);
            spriteBatch.Draw(sprite, new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y)),
                new Rectangle(framewidth * (int)Math.Floor((double)currentFrame), 0, framewidth, height), color * alpha,
                (float)(((double)rotation) * Math.PI / 180), origin, new Vector2(xscale, yscale), SpriteEffects.None, 0);
        }

        public void Update()
        {
            currentFrame += speed;
            if (currentFrame >= frames)
                currentFrame -= frames;
        }

        public void CenterOrigin()
        {
            origin = new Vector2(framewidth / 2, height / 2);
        }
    }
}
