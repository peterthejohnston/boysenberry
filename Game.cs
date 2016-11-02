#region File Description
//-----------------------------------------------------------------------------
// EngineGame.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

#endregion

namespace Engine
{
    /// <summary>
    /// Default Project Template
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        #region Fields

        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        public KeyboardState keyState, keyOldState;
        public MouseState mouseState, mouseOldState;
        public GameTime gameTime;
        public int timer;
        public Random rand = new Random();
        public SpriteFont font;

        #endregion

        #region Scenes

        private Scene _currentScene;
        public Scene currentScene
        {
            get { return _currentScene; }
            set
            {
                if (currentScene != null)
                {
                    foreach (SoundEffectInstance s in currentScene.sounds)
                    {
                        s.Stop();
                    }
                }

                _currentScene = value;

                graphics.PreferredBackBufferWidth = value.view.width;
                graphics.PreferredBackBufferHeight = value.view.height;
                // graphics.ApplyChanges();

                currentScene.SceneStart();
            }
        }

        public List<Scene> scenes = new List<Scene>();
        public Scene mainScene;

        public void NextScene()
        {
            if (!currentScene.displayTextbox)
                currentScene = scenes[scenes.IndexOf(currentScene, 0) + 1];
            else
                currentScene.tryNextScene = true;
        }
        public void SetScene(Scene scene)
        {
            currentScene = scenes[scenes.IndexOf(scene)];
        }

        #endregion

        #region Initialization

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Assets";

            graphics.IsFullScreen = false;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be use to draw textures.
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 480;

            /*mainScene = new Scene();
            mainScene.Load(640, 480, this);
            scenes.Add(mainScene);
            currentScene = mainScene;*/
        }

        #endregion

        #region Update and Draw

        protected override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            keyState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (currentScene != null)
                currentScene.Update(gameTime);

            ++timer;

            keyOldState = keyState;
            mouseOldState = mouseState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear the backbuffer
            graphics.GraphicsDevice.Clear(Color.Black);

            if (currentScene != null)
                currentScene.Draw();

            base.Draw(gameTime);
        }

        #endregion
    }
}
