using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Engine
{
    public class Pause : Actor
    {
        private int fadeTime;
        private int maxfadeTime;
        private bool fading;
        private Sprite resumeSprite;

        public Pause(float x, float y, Game game)
            : base(x, y, -3, game)
        {
        }

        public override void Initialize()
        {
            fadeTime = 40;
            maxfadeTime = 40;

            sprite = new Sprite(game.Content.Load<Texture2D>("pause"));
            resumeSprite = new Sprite(game.Content.Load<Texture2D>("resume"));

            base.Initialize();
        }

        public override void Update()
        {
            if (game.keyState.IsKeyDown(Keys.P) && game.keyOldState.IsKeyUp(Keys.P) && !fading)
                game.currentScene.Unpause();
            if (game.keyState.IsKeyDown(Keys.R) && game.keyOldState.IsKeyUp(Keys.R))
            {
                game.currentScene.Unpause();
                game.currentScene.Restart();
            }

            if (fading)
            {
                fadeTime -= 1;
                if (fadeTime == 0)
                    game.currentScene.DeleteActor(this);

                sprite.alpha = (float)fadeTime / (float)maxfadeTime;

                x = game.currentScene.view.x;
                y = game.currentScene.view.y;
            }

            base.Update();
        }

        public void Fade()
        {
            fading = true;
            sprite = resumeSprite;
        }
    }
}
