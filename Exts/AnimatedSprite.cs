using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;

namespace Multris {
	public class AnimatedSprite {

		public class AnimationFrame {

			private Texture2D texture;
			private Rectangle source;
			private Vector2 position;
			private Point size;
			private Color color;
			private float rotation;

			public AnimationFrame(Texture2D txt, Vector2 pos, Point? siz = null, Rectangle? src = null, float rot = 0.0f, Color? tint = null) {
				texture = txt;
				position = pos;
				source = src ?? txt.Bounds;
				size = siz ?? source.Size;
				color = tint ?? Color.White;
				rotation = rot;
			}

			public void Draw(SpriteBatch sb) {
				sb.Draw(
				texture:texture,
				sourceRectangle:source, 
				destinationRectangle: new Rectangle((int) position.X, (int) position.Y, size.X, size.Y),
				color: color
				);
			}

		}
		private readonly AnimationFrame[] frames;
		private uint currentFrame;
		private readonly bool animatable = false;

		public AnimatedSprite(AnimationFrame[] frames) {
			try {
				if( frames.Length > 0 ) // If not empty
					currentFrame = 0;          // Start counting
				this.frames = frames;
				this.animatable = true;
			} catch( Exception ) {
				this.animatable = false;
			}
		}

		public void DrawNext(SpriteBatch sb) {

		}

	}
}