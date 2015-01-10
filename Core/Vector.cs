using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    public struct Vector
    {
        private Vector2 motion;

        private Vector(float x, float y)
        {
            motion = new Vector2(x, y);
            rememberDir = (float)((180 / Math.PI) * Math.Atan2(-motion.Y, motion.X));
            rememberSpeed = 0;
        }

		public static Vector makeVector2(Vector2 v)
		{
			return new Vector(v.X, v.Y);
		}

        public static Vector makeXY(float x, float y)
        {
            return new Vector(x, y);
        }

        public static Vector makeDirSpeed(float direction, float speed)
        {
            float localX = speed * (float)Math.Cos(direction * Math.PI / 180);
            float localY = speed * -(float)Math.Sin(direction * Math.PI / 180);
            return new Vector(localX, localY);
        }

        public float x
        {
            get { return motion.X; }
            set
            {
                float prevdir = direction;
                motion.X = value;

                if (speed == 0)
                    rememberDir = prevdir;
            }
        }
        public float y
        {
            get { return motion.Y; }
            set
            {
                float prevdir = direction;
                motion.Y = value;

                if (speed == 0)
                    rememberDir = prevdir;
            }
        }

        private float rememberDir;
        public float direction
        {
            get
            {
                /*if (speed != 0)
                    return (float)((180 / Math.PI) * Math.Atan2(-motion.Y, motion.X));*/
				if (speed != 0)
				{
					float val = (float)((180 / Math.PI) * Math.Atan2(motion.Y, -motion.X) + 180);
					return (val < 360) ? val : val - 360;
				}
                return rememberDir;
            }
            set
            {
                //placeholder for the previous speed
                float prevspeed = this.speed;
                x = prevspeed * (float)Math.Cos(value * Math.PI / 180);
                y = prevspeed * -(float)Math.Sin(value * Math.PI / 180);

                if (speed == 0)
                    rememberDir = value;
            }
        }
        private float rememberSpeed;
        public float speed
        {
            get
            {
                if (rememberSpeed < 0)
                    return rememberSpeed;
                return motion.Length();
            }
            set
            {
                if (value < 0)
                    rememberSpeed = value;
                else rememberSpeed = 0;

                float prevdir = this.direction;
                x = value * (float)Math.Cos(prevdir * Math.PI / 180);
                y = value * -(float)Math.Sin(prevdir * Math.PI / 180);

                if (value == 0)
                    rememberDir = prevdir;
            }
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.x + b.x, a.y + b.y);
        }

        public static implicit operator Vector2(Vector v)
        {
            return new Vector2(v.x, v.y);
        }
    }
}
