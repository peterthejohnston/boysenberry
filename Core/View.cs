using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class View
    {
		public View(int width, int height, Game game)
        {
            this.width = width;
            this.height = height;
			this.game = game;
        }

        public int width, height;
		private Game game;

        private Vector2 m_position = Vector2.Zero;
		public Matrix Transform {
			get { return Matrix.CreateRotationZ(0) * Matrix.CreateScale(1f) * Matrix.CreateTranslation(-x, -y, 0); }
		}

        public Vector2 position
        {
            get { return m_position; }
            set { m_position = value; }
        }
        public float x
        {
            get { return m_position.X; }
            set { m_position.X = value; }
        }
        public float y
        {
            get { return m_position.Y; }
            set { m_position.Y = value; }
        }

        /*private Vector m_velocity;

        public Vector velocity
        {
            get { return m_velocity; }
            set { m_velocity = value; }
        }
        public float xspeed
        {
            get { return velocity.x; }
            set { m_velocity.x = value; }
        }
        public float yspeed
        {
            get { return velocity.y; }
            set { m_velocity.y = value; }
        }
        public float direction
        {
            get { return velocity.direction; }
            set { m_velocity.direction = value; }
        }
        public float speed
        {
            get { return velocity.speed; }
            set { m_velocity.speed = value; }
        }*/

        public void Update()
        {
            // position += velocity;

            if (follow != null)
                position = new Vector2(follow.x - width / 2, follow.y - height / 2);

			// x = Math.Max(0, x);
			y = MathHelper.Clamp(y, 0, game.currentScene.height - height);
        }

        private Actor follow;
        public void Follow(Actor a)
        {
            follow = a;
        }
    }
}
