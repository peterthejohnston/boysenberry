using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Engine
{
    public class Actor
    {
        private Vector2 _position;
        private Vector2 _posPrevious;
        protected Vector _motion;
        private Mask _mask;
        private Sprite _sprite;
        private Rectangle _bbox;
        public bool colliding;

        public int depth;
        public bool solid;
        protected float gravity;

        protected Game game;

        protected static Sprite pixel;

        public float x
        {
            get { return position.X; }
            set
            {
                _position.X = value;
                if (mask != null)
                    _bbox.X = (int)(x - mask.origin.X);
                else _bbox.X = (int)x;
            }
        }
        public float y
        {
            get { return position.Y; }
            set
            {
                _position.Y = value;
                if (mask != null)
                    _bbox.Y = (int)(y - mask.origin.Y);
                else _bbox.Y = (int)y;
            }
        }
        public Vector2 position
        {
            get { return _position; }
            set
            {
                _position = value;
                if (mask != null)
                {
                    _bbox.X = (int)(_position.X - mask.origin.X);
                    _bbox.Y = (int)(_position.Y - mask.origin.Y);
                }
                else
                {
                    _bbox.X = (int)_position.X;
                    _bbox.Y = (int)_position.Y;
                }
            }
        }
        public float xPrevious
        {
            get { return posPrevious.X; }
            set { _posPrevious.X = value; }
        }
        public float yPrevious
        {
            get { return posPrevious.Y; }
            set { _posPrevious.Y = value; }
        }
        public Vector2 posPrevious
        {
            get { return _posPrevious; }
            set { _posPrevious = value; }
        }
        public Vector motion
        {
            get { return _motion; }
            set { _motion = value; }
        }
        public float xspeed
        {
            get { return motion.x; }
            set { _motion.x = value; }
        }
        public float yspeed
        {
            get { return motion.y; }
            set { _motion.y = value; }
        }
        public float direction
        {
            get { return motion.direction; }
            set { _motion.direction = value; }
        }
        public float speed
        {
            get { return motion.speed; }
            set { _motion.speed = value; }
        }
        public Sprite sprite
        {
            get { return _sprite; }
            set
            {
                _sprite = value;
                //_sprite.currentFrame = 0;
                if (mask == null)
                {
                    mask = new Mask(value.sprite, sprite.origin);
                }
            }
        }
        public Mask mask
        {
            get { return _mask; }
            set {
                _mask = value;
                _bbox = new Rectangle((int)x, (int)y, value.width, value.height);
            }
        }
        public Vector2 origin
        {
            get
            {
                if (sprite != null)
                    return sprite.origin;
                else return Vector2.Zero;
            }
            set
            {
                if (sprite != null)
                    sprite.origin = value;
                mask.origin = value;
            }
        }
        public Rectangle bbox
        {
            get { return _bbox; }
            set
            {
                _bbox = value;
            }
        }
        /*public Polygon shape
        {
            get { return _bbox; }
            set
            {
                _bbox = value;
                _bbox.BuildEdges();
            }
        }*/
        public bool IntersectBoundary {
            get { return !(new Rectangle(0, 0, game.currentScene.width, game.currentScene.height)).Intersects(bbox); }
        }

        // Only place extra logic in the constructor if the actor is not going to be in the scene
        public Actor(float x, float y, int depth, Game game)
        {
            bbox = new Rectangle();

            this.x = x;
            this.y = y;
            this.depth = depth;
            this.game = game;
        }

        // Only when the scene is starting with the actor in it
        public virtual void SceneStart() {}

        // For when put in the scene; load content here as well
        public virtual void Initialize() {}

        // For when deleted from the scene
        public virtual void Destroy() {}

        public Actor Clone()
        {
            return (Actor)MemberwiseClone();
        }

        public void ActorUpdate()
        {
            posPrevious = position;

            Update();
            if (sprite != null)
                sprite.Update();

            /*if (!colliding)
                position += motion;
            else position = body.Position;*/
            yspeed += gravity;
            position += motion;
        }

        //override for each different child of actor
        public virtual void Update() {}

        public virtual void EndUpdate() {}

        public virtual void Draw()
        {
            if (sprite != null)
                sprite.Draw(game.spriteBatch, position);
        }

        //overload for each different child of actor
        public virtual void Collision(Actor a) {}

        public void MoveContact()
        {
            for (int movement = 0; !CollisionSolid(x + Math.Sign(xspeed), y) && movement < Math.Abs(xspeed) && Math.Abs(xspeed) > 0.05; ++movement)
                x += Math.Sign(xspeed);
            for (int movement = 0; !CollisionSolid(x, y + Math.Sign(yspeed)) && movement < Math.Abs(yspeed) && Math.Abs(yspeed) > 0.05; ++movement)
                y += Math.Sign(yspeed);
        }

        public bool Collision<T>(float x, float y)
        {
            Vector2 begin = position;
            this.x = x; this.y = y;

            foreach (Actor actor in game.currentScene.actors)
            {
                if (this != actor && actor is T && bbox.Intersects(actor.bbox))
                {
                    position = begin;
                    return true;
                }
            }

            position = begin;
            return false;
        }

        public bool Collision(Actor a, float x, float y)
        {
            Vector2 begin = position;
            this.x = x; this.y = y;

            if (bbox.Intersects(game.currentScene.actors[game.currentScene.actors.IndexOf(a)].bbox))
            {
                position = begin;
                return true;
            }

            position = begin;
            return false;
        }

        public bool CollisionSolid(float x, float y)
        {
            Vector2 begin = position;
            this.x = x; this.y = y;
            foreach (Actor actor in game.currentScene.solidActors)
            {
                if (this != actor && actor.bbox.Intersects(bbox))
                {
                    position = begin;
                    return true;
                }
            }
            position = begin;
            return false;
        }

        public float DistanceTo(Actor a)
        {
            Vector2 v = new Vector2(x - a.x, y - a.y);
            return v.Length();
        }

        public void MotionSet(float direction, float speed)
        {
            motion = Vector.makeDirSpeed(direction, speed);
        }

        public void MotionAdd(float direction, float speed)
        {
            motion += Vector.makeDirSpeed(direction, speed);
        }

        protected void DrawBBox()
        {
            Vector2 p1 = new Vector2(bbox.Left, bbox.Top);
            Vector2 p2 = new Vector2(bbox.Right, bbox.Top);
            Vector2 p3 = new Vector2(bbox.Right, bbox.Bottom);
            Vector2 p4 = new Vector2(bbox.Left, bbox.Bottom);

            game.spriteBatch.DrawLine(p1, p2, Color.Green);
            game.spriteBatch.DrawLine(p2, p3, Color.Green);
            game.spriteBatch.DrawLine(p3, p4, Color.Green);
            game.spriteBatch.DrawLine(p4, p1, Color.Green);


            /*for (int i = 0; i < shape.Points.Count; i++) {
                VectorE p1 = shape.Points[i];
                VectorE p2;
                if (i + 1 >= shape.Points.Count) {
                    p2 = shape.Points[0];
                } else {
                    p2 = shape.Points[i + 1];
                }
                game.spriteBatch.DrawLine(new Vector2(p1.X, p1.Y), new Vector2(p2.X, p2.Y), Color.White);
            }*/
        }
    }
}
