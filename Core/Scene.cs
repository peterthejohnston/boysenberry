using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Engine
{
    public class Scene
    {
		#region Fields

        private Game game;
		private GameTime gameTime;

		public bool displayTextbox;
		public Keys continueKey = Keys.Space;
		public Texture2D textboxBackground;
		public SoundEffect typeSound;
		public char delayChar = '|';
		public int delayTime = 10;
		public Sprite continueSprite;
		private Sprite textboxSprite;
		private bool isTextboxSprite;
		public int textboxSpriteBufferH = 20;
		public int textboxSpriteBufferV = 20;
		public int continueSpriteBufferH = 15;
		public int continueSpriteBufferV = 15;
		public int textboxBufferV = 30;
		public int textBufferH = 20;
		public int textBufferV = 20;
		public int textDelay = 80;
		private String currentText = "";
		private String targetText;
		private List<String> textQueue;
		private List<Sprite> spriteQueue;
		private bool isDoneDrawing;
		private int lastTypedTextLength;
		private double typedTextLength;
		private List<int> indexesOfDelays;

        public List<Actor> actors { get; set; }
        public List<Actor> solidActors { get; set; }
        public List<Actor> enemies { get; set; }
        private List<Actor> draw;
		public Texture2D background { get; set; }
		private View m_view;
		public int width, height;

		public List<SoundEffectInstance> sounds;

		#endregion

		#region View

		public View view
		{
			get { return m_view; }
			set
			{
				m_view = value;
				/*if (game.currentScene == this)
				{
					game.graphics.PreferredBackBufferWidth = value.width;
					game.graphics.PreferredBackBufferHeight = value.height;
					game.graphics.ApplyChanges();
				}*/
			}
		}

		#endregion

        /*private Viewport _viewport;
        public Viewport viewport
        {
            get { return _viewport; }
            set { _viewport = value; }
        }
        public int width
        {
            get { return viewport.Width; }
            set
            {
                if (value > 0)
                {
                    _viewport.Width = value;
                    if (game.currentScene == this)
                    {
                        game.graphics.PreferredBackBufferWidth = value;
                        game.graphics.GraphicsDevice.Viewport = viewport;
                        game.graphics.ApplyChanges();
                    }
                }
            }
        }
        public int height
        {
            get { return viewport.Height; }
            set
            {
                if (value > 0)
                {
                    _viewport.Height = value;
                    if (game.currentScene == this)
                    {
                        game.graphics.PreferredBackBufferHeight = value;
                        game.graphics.GraphicsDevice.Viewport = viewport;
                        game.graphics.ApplyChanges();
                    }
                }
            }
        }*/

		#region Loading

		public Scene() {}

		private SceneLoad sceneLoad;

		public void Load(List<Actor> actors, Texture2D background, int width, int height, View view, Game game)
        {
			displayTextbox = false;
			isTextboxSprite = false;
			textQueue = new List<String>();
			spriteQueue = new List<Sprite>();
			isDoneDrawing = true;
			typedTextLength = 0;
			lastTypedTextLength = 0;
			tryRestart = false;
			tryNextScene = false;

			List<Actor> loadActors = new List<Actor>();
			foreach (Actor a in actors)
			{
				loadActors.Add(a.Clone());
			}
			sceneLoad = new SceneLoad(loadActors, background, width, height, view, game);

			this.actors = new List<Actor>();
			solidActors = new List<Actor>();
			draw = new List<Actor>();
			enemies = new List<Actor>();

			foreach (Actor a in actors)
			{
				AddActor(a);
			}

            if (this.actors.Count != 0)
            {
                draw = draw.OrderByDescending(actor => actor.depth).ToList();
            }

            for (int i = 0; i < actors.Count; ++i)
            {
                if (actors[i].solid)
                    solidActors.Add(actors[i]);
                /*if (actors[i] is Enemy)
                    enemies.Add(actors[i]);*/
            }

            this.background = background;
            //goes first so that we can change game.graphics when we assign width and height next
            this.game = game;
            this.width = width;
			this.height = height;

			this.view = view;

			sounds = new List<SoundEffectInstance>();
        }

		public void Load(Texture2D background, int width, int height, Game game)
        {
			displayTextbox = false;
			isTextboxSprite = false;
			textQueue = new List<String>();
			spriteQueue = new List<Sprite>();
			isDoneDrawing = true;
			typedTextLength = 0;
			lastTypedTextLength = 0;
			tryRestart = false;
			tryNextScene = false;

			sceneLoad = new SceneLoad(new List<Actor>(), background, width, height, new View(width, height, game), game);

			//so that if we don't have actors, draw and actors won't be null
            actors = new List<Actor>();
            solidActors = new List<Actor>();
            draw = new List<Actor>();
            enemies = new List<Actor>();

            this.background = background;
            //goes first so that we can change game.graphics when we assign width and height next
            this.game = game;
            this.width = width;
			this.height = height;

			view = new View(width, height, game);

			sounds = new List<SoundEffectInstance>();
        }

		public void Load(int width, int height, Game game)
        {
			displayTextbox = false;
			isTextboxSprite = false;
			textQueue = new List<String>();
			spriteQueue = new List<Sprite>();
			isDoneDrawing = true;
			typedTextLength = 0;
			lastTypedTextLength = 0;
			tryRestart = false;
			tryNextScene = false;

			sceneLoad = new SceneLoad(new List<Actor>(), null, width, height, new View(width, height, game), game);

			//so that if we don't have actors, draw and actors won't be null
            actors = new List<Actor>();
            solidActors = new List<Actor>();
            draw = new List<Actor>();
            enemies = new List<Actor>();

            //goes first so that we can change game.graphics when we assign width and height next
            this.game = game;
            this.width = width;
			this.height = height;

			view = new View(width, height, game);

			sounds = new List<SoundEffectInstance>();
        }

		#endregion

		#region Scene Functions

		public void SceneStart()
		{
			for (int i = 0; i < actors.Count; i++)
			{
				actors[i].SceneStart();
			}
		}

		private bool tryRestart;
		public bool tryNextScene;
		public void Restart()
		{
			if (!displayTextbox)
			{
				for (int i = 0; i < actors.Count; i++)
				{
					Actor a = actors[i];
					DeleteActor(a);
				}

				Load(sceneLoad.actors, sceneLoad.background, sceneLoad.width, sceneLoad.height, sceneLoad.view, sceneLoad.game);

				game.currentScene = this;
			}
			else
				tryRestart = true;
		}

		#endregion

		#region Update

        public void Update(GameTime gameTime)
        {
			this.gameTime = gameTime;

			if (paused)
			{
				pause.Update();
				return;
			}

			if (displayTextbox)
			{
				UpdateTextbox();
				continueSprite.Update();
				return;
			}
			else
			{
				if (tryRestart)
					Restart();
				if (tryNextScene)
					game.NextScene();
			}
			for (int i = 0; i < actors.Count; i++)
        	{
				actors[i].ActorUpdate();
        	}

			if (game.keyState.IsKeyDown(Keys.P) && game.keyOldState.IsKeyUp(Keys.P))
				Pause();

			Collision();

			for (int i = 0; i < actors.Count; i++)
			{
				actors[i].EndUpdate();
			}

			view.Update();
        }

		#endregion

		private bool paused = false;
		private Pause pause;
		public bool pausable = false;
		public void Pause()
		{
			if (pausable)
			{
				pause = new Pause(game.currentScene.view.x, game.currentScene.view.y, game);
				game.currentScene.AddActor(pause);
				paused = true;
			}
		}

		public void Unpause()
		{
			paused = false;
			pause.Fade();
		}

		public void PlaySound(SoundEffect s, bool looped, float volume)
		{
			SoundEffectInstance i = new SoundEffectInstance(s);
			sounds.Add(i);
			i.Play();
			i.IsLooped = looped;
			i.Volume = volume;
		}

		#region Textbox

		private void UpdateTextbox()
		{
			DelayTimer();

			if (isTextboxSprite)
				textboxSprite.Update();

			if (game.keyState.IsKeyDown(continueKey) && game.keyOldState.IsKeyUp(continueKey))
			{
				if (isDoneDrawing)
				{
					if (textQueue.Count == 0)
					{
						displayTextbox = false;
						currentText = "";
						targetText = "";
					}
					else
					{
						targetText = textQueue[0];
						if (spriteQueue[0] == null)
							isTextboxSprite = false;
						else
							isTextboxSprite = true;
						textboxSprite = spriteQueue[0];
						textQueue.Remove(textQueue[0]);
						spriteQueue.Remove(spriteQueue[0]);
						isDoneDrawing = false;
						typedTextLength = 0;
						lastTypedTextLength = 0;
					}
				}
				else
				{
					currentText = targetText;
					for (int i = 0; i < currentText.Length; i++)
					{
						if (currentText[i] == delayChar)
						{
							currentText = currentText.Remove(i, 1);
							i--;
						}
					}
					typedTextLength = targetText.Length;
					lastTypedTextLength = (int)typedTextLength;
					isDoneDrawing = true;
				}
			}

			if (delayTimer == 0)
			{
				if (!isDoneDrawing)
				{
					if (textDelay == 0)
					{
						currentText = targetText;
						for (int i = 0; i < currentText.Length; i++)
						{
							if (currentText[i] == delayChar)
							{
								currentText = currentText.Remove(i, 1);
								i--;
							}
						}
						isDoneDrawing = true;
					}
					else if (typedTextLength < targetText.Length)
					{
						if (gameTime != null)
							typedTextLength += gameTime.ElapsedGameTime.TotalMilliseconds / textDelay;

						if ((int)typedTextLength > lastTypedTextLength)
							typeSound.Play();

						if (indexesOfDelays.Contains((int)typedTextLength))
						{
							delayTimer = delayTime;
						}

						if (typedTextLength >= targetText.Length)
						{
							typedTextLength = targetText.Length;
							isDoneDrawing = true;
						}

						currentText = targetText.Substring(0, (int)typedTextLength);

						for (int i = 0; i < currentText.Length; i++)
						{
							if (currentText[i] == delayChar)
							{
								currentText = currentText.Remove(i, 1);
								i--;
							}
						}

						lastTypedTextLength = (int)typedTextLength;
					}
				}
			}
		}

		private int delayTimer = 0;
		private void DelayTimer()
		{
			if (delayTimer > 0)
			{
				delayTimer -= 1;
				if (delayTimer == 0)
				{
					typedTextLength += 1;
				}
			}
		}

		public void Textbox(Sprite sprite, String text)
		{

			if (!displayTextbox)
			{
				isTextboxSprite = true;
				textboxSprite = sprite;
				displayTextbox = true;

				indexesOfDelays = new List<int>();
				for (int index = text.IndexOf('|');
					index >= 0;
					index = text.IndexOf('|', index + 1))
				{
					indexesOfDelays.Add(index);
				}
				targetText = wrapText(parseText(text, sprite), sprite);
				isDoneDrawing = false;
				typedTextLength = 0;
				lastTypedTextLength = 0;
				UpdateTextbox();
			}
			else
			{
				int prevCount = textQueue.Count;
				String addText = wrapText(parseText(text, sprite), sprite);
				if (textQueue.Count > prevCount)
					textQueue.Insert(Math.Max(0, textQueue.Count - 1), addText);
				else textQueue.Add(addText);
				spriteQueue.Add(sprite);
			}
		}

		public void Textbox(String text)
		{
			if (!displayTextbox)
			{
				isTextboxSprite = false;
				textboxSprite = null;
				displayTextbox = true;

				indexesOfDelays = new List<int>();
				for (int index = text.IndexOf('|');
					index >= 0;
					index = text.IndexOf('|', index + 1))
				{
					indexesOfDelays.Add(index);
				}
				targetText = wrapText(parseText(text, null), null);
				isDoneDrawing = false;
				typedTextLength = 0;
				lastTypedTextLength = 0;
				UpdateTextbox();
			}
			else
			{
				int prevCount = textQueue.Count;
				String addText = wrapText(parseText(text, null), null);
				if (textQueue.Count > prevCount)
					textQueue.Insert(Math.Max(0, textQueue.Count - 1), addText);
				else textQueue.Add(addText);
				spriteQueue.Add(null);
			}
		}

		private String parseText(String text, Sprite sprite)
		{
			if (text == null)
				return String.Empty;

			String line = String.Empty;
			String returnString = String.Empty;
			String[] wordArray = text.Split(' ');

			foreach (String word in wordArray)
			{
				if (sprite != null)
				{
					if (game.font.MeasureString(line + word).Length() > (textboxBackground.Width - 2 * textBufferH - sprite.width - textboxSpriteBufferH))
					{
						returnString = returnString + line + '\n';
						line = String.Empty;
					}
				}
				else
				{
					if (game.font.MeasureString(line + word).Length() > (textboxBackground.Width - 2 * textBufferH))
					{
						returnString = returnString + line + '\n';
						line = String.Empty;
					}
				}

				line = line + word + ' ';
			}

			return returnString + line;
		}

		private String wrapText(String text, Sprite sprite)
		{
			String[] lineArray = text.Split('\n');
			String returnString = String.Empty;
			String nextString = String.Empty;

			if (text == String.Empty)
				return String.Empty;
			foreach (String line in lineArray)
			{
				if (game.font.MeasureString(returnString + line).Y <= (textboxBackground.Height - textBufferV - continueSprite.height - continueSpriteBufferV))
				{
					returnString += line + '\n';
				}
				else
				{
					if (line != "")
						nextString += line + '\n';
				}
			}

			if (nextString != "")
			{
				List<String> next = new List<String>();
				wrapTextR(nextString, sprite, next);
				foreach (String s in next)
				{
					textQueue.Add(s);
				}
				spriteQueue.Add(sprite);
			}

			return returnString;
		}

		private String wrapTextR(String text, Sprite sprite, List<String> next)
		{
			String[] lineArray = text.Split('\n');
			String returnString = String.Empty;
			String nextString = String.Empty;

			if (text == String.Empty)
				return String.Empty;
			foreach (String line in lineArray)
			{
				if (game.font.MeasureString(returnString + line).Y <= (textboxBackground.Height - textBufferV - continueSprite.height - continueSpriteBufferV))
				{
					returnString += line + '\n';
				}
				else
				{
					if (line != "")
						nextString += line + '\n';
				}
			}

			if (nextString != "")
			{
				next.Insert(0, wrapTextR(nextString, sprite, next));
				spriteQueue.Add(sprite);
			}
			else
				next.Insert(0, returnString);
			
			return returnString;
		}

		#endregion

		#region Draw

        public void Draw()
        {
			game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, view.Transform);
            if (background != null)
				game.spriteBatch.Draw(background, new Vector2(game.currentScene.view.x, 0), null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

			for (int i = 0; i < draw.Count; i++)
			{
				draw[i].Draw();
			}

			if (displayTextbox)
			{
				game.spriteBatch.Draw(
					textboxBackground,
					new Vector2(view.x + view.width / 2 - textboxBackground.Width / 2,
						view.y + view.height - textboxBackground.Height - textboxBufferV),
					Color.White);
				continueSprite.Draw(
					game.spriteBatch,
					new Vector2(view.x + view.width / 2 + textboxBackground.Width / 2 - continueSprite.width - continueSpriteBufferH,
						view.y + view.height - continueSprite.height - textboxBufferV - continueSpriteBufferV));

				if (isTextboxSprite)
				{
					textboxSprite.Draw(
						game.spriteBatch,
						new Vector2(view.x + view.width / 2 - textboxBackground.Width / 2 + textboxSpriteBufferH,
							view.y + view.height - textboxBackground.Height - textboxBufferV + textboxSpriteBufferV));

					game.spriteBatch.DrawString(game.font, currentText,
						new Vector2(view.x + view.width / 2 - textboxBackground.Width / 2 + textBufferH + textboxSprite.width + textboxSpriteBufferH,
							view.y + view.height - textboxBackground.Height - textboxBufferV + textBufferV),
						Color.White);
				}
				else
				{
					game.spriteBatch.DrawString(game.font, currentText,
						new Vector2(view.x + view.width / 2 - textboxBackground.Width / 2 + textBufferH,
							view.y + view.height - textboxBackground.Height - textboxBufferV + textBufferV),
						Color.White);
				}
			}

            game.spriteBatch.End();
        }

		#endregion

		#region Actors Modifiers

        public void AddActor(Actor a)
        {
            actors.Add(a);

            if (a.solid)
                solidActors.Add(a);
            /*if (a is Enemy)
                enemies.Add(a);*/

			draw.Add(a);
			draw = draw.OrderByDescending(actor => actor.depth).ToList();

			a.Initialize();

			// Remove once we fix the level editor and redo level 1
			a.position += a.origin;
        }
        public void DeleteActor(Actor a)
		{
			a.Destroy();

            actors.Remove(a);

            if (a.solid)
                solidActors.Remove(a);
            /*if (a is Enemy)
                enemies.Remove(a);*/

            draw.Remove(a);
        }

		#endregion

		#region Collisions

        public void Collision()
        {

			for (int i = 0; i < actors.Count; i++)
			{
				Actor a = actors[i];
				// check for collision with every other actor
				for (int ii = i + 1; ii < actors.Count; ii++)
				{
					Actor b = actors[ii];
					if (b != a && a.bbox.Intersects(b.bbox))
					{
						a.Collision(b);
						b.Collision(a);
					}
				}
			}


			// for every actor,
			/*for (int i = 0; i < actors.Count; i++)
            {
				Actor a = actors[i];
                // check for collision with every other actor
				if (a.shape.Points.Count > 0)
				{
					for (int ii = i + 1; ii < actors.Count; ii++)
					{
						Actor b = actors[ii];
						if (b.shape.Points.Count > 0)
						{
							if (b != a && new Rectangle((int)b.shape.minX, (int)b.shape.minY, (int)b.shape.maxX - (int)b.shape.minX, (int)b.shape.maxY - (int)b.shape.minY).Intersects(
								new Rectangle((int)a.shape.minX, (int)a.shape.minY, (int)a.shape.maxX - (int)a.shape.minX, (int)a.shape.maxY - (int)a.shape.minY)))
							{
								a.Collision(b);
								b.Collision(a);
							}
						}
					}
				}
            }*/

			/*
			=====================
			VectorE playerTranslation = new VectorE(freeman.xspeed, freeman.yspeed);
			=======================

			foreach (Polygon polygon in polygons) {
				if (polygon == player) continue;

				PolygonCollisionResult r = PolygonCollision(player, polygon, velocity);

				if (r.WillIntersect) {
					playerTranslation = velocity + r.MinimumTranslationVector;
					break;
				}
			}

			/*Polygon freemanPolygon = new Polygon();
			freemanPolygon.Points.Add(new VectorE(freeman.shape.Left, freeman.shape.Top));
			freemanPolygon.Points.Add(new VectorE(freeman.shape.Left, freeman.shape.Bottom));
			freemanPolygon.Points.Add(new VectorE(freeman.shape.Right, freeman.shape.Bottom));
			freemanPolygon.Points.Add(new VectorE(freeman.shape.Right, freeman.shape.Top));
			//freemanPolygon.Offset(new VectorE(freeman.x, freeman.y));
			freemanPolygon.BuildEdges();*/

			/*=========================
			PolygonCollisionResult r = PolygonCollision(freemanPolygon, polygon, new VectorE(freeman.xspeed, freeman.yspeed));

			if (r.WillIntersect) {
				playerTranslation = new VectorE(freeman.xspeed, freeman.yspeed) + r.MinimumTranslationVector;
			}

			freeman.position += new Vector2(playerTranslation.X, playerTranslation.Y);
			freeman.shape.Offset(playerTranslation);
			==========================*/
			// freeman.position = new Vector2(freemanPolygon.Center.X, freemanPolygon.Center.Y);*/
        }


		// Check if polygon A is going to collide with polygon B.
		// The last parameter is the *relative* velocity 
		// of the polygons (i.e. velocityA - velocityB)
		/*public PolygonCollisionResult PolygonCollision(Polygon polygonA, 
			Polygon polygonB, VectorE velocity) {
			PolygonCollisionResult result = new PolygonCollisionResult();
			result.Intersect = true;
			result.WillIntersect = true;

			int edgeCountA = polygonA.Edges.Count;
			int edgeCountB = polygonB.Edges.Count;
			float minIntervalDistance = float.PositiveInfinity;
			VectorE translationAxis = new VectorE();
			VectorE edge;

			// Loop through all the edges of both polygons
			for (int edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++) {
				if (edgeIndex < edgeCountA) {
					edge = polygonA.Edges[edgeIndex];
				} else {
					edge = polygonB.Edges[edgeIndex - edgeCountA];
				}

				// ===== 1. Find if the polygons are currently intersecting =====

				// Find the axis perpendicular to the current edge
				VectorE axis = new VectorE(-edge.Y, edge.X);
				axis.Normalize();

				// Find the projection of the polygon on the current axis
				float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
				Polygon.ProjectPolygon(axis, polygonA, ref minA, ref maxA);
				Polygon.ProjectPolygon(axis, polygonB, ref minB, ref maxB);

				// Check if the polygon projections are currentlty intersecting
				if (Polygon.IntervalDistance(minA, maxA, minB, maxB) > 0)
					result.Intersect = false;

				// ===== 2. Now find if the polygons *will* intersect =====

				// Project the velocity on the current axis
				float velocityProjection = axis.DotProduct(velocity);

				// Get the projection of polygon A during the movement
				if (velocityProjection < 0) {
					minA += velocityProjection;
				} else {
					maxA += velocityProjection;
				}

				// Do the same test as above for the new projection
				float intervalDistance = Polygon.IntervalDistance(minA, maxA, minB, maxB);
				if (intervalDistance > 0) result.WillIntersect = false;

				// If the polygons are not intersecting and won't intersect, exit the loop
				if (!result.Intersect && !result.WillIntersect) break;

				// Check if the current interval distance is the minimum one. If so store
				// the interval distance and the current distance.
				// This will be used to calculate the minimum translation vector
				intervalDistance = Math.Abs(intervalDistance);
				if (intervalDistance < minIntervalDistance) {
					minIntervalDistance = intervalDistance;
					translationAxis = axis;

					VectorE d = polygonA.Center - polygonB.Center;
					if (d.DotProduct(translationAxis) < 0)
						translationAxis = -translationAxis;
				}
			}

			// The minimum translation vector
			// can be used to push the polygons appart.
			if (result.WillIntersect)
				result.MinimumTranslationVector = 
					translationAxis * minIntervalDistance;

			return result;
		}*/

		#endregion
    }
}