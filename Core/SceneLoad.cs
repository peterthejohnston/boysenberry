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
	class SceneLoad
	{
		public List<Actor> actors;
		public Texture2D background;
		public int width, height;
		public View view;
		public Game game;

		public SceneLoad(List<Actor> actors, Texture2D background, int width, int height, View view, Game game)
		{
			this.actors = actors;
			this.background = background;
			this.width = width;
			this.height = height;
			this.view = view;
			this.game = game;
		}
	}
}

