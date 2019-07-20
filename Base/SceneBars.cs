﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Text;
using System;

namespace lnbase.Base {
	public class SceneBars {

		public GameSprite Texture { get; private set; }
		public Rectangle Name { get; private set; }
		public Rectangle Text { get; private set; }

		public SceneBars(GameSprite txt, Rectangle? name = null, Rectangle? text = null) {
			Texture = txt;
			Name = name ?? Texture.Texture.Bounds;
			Text = text ?? Texture.Texture.Bounds;
		}

		/// <summary>
		/// Given font, text and width of container returns string with \n where should be line break 
		/// </summary>
		public string WrapText(SpriteFont spriteFont, string text, float maxLineWidth) {
			string[] words = text.Split(' ');
			StringBuilder sb = new StringBuilder( );
			float lineWidth = 0f;
			float spaceWidth = spriteFont.MeasureString(" ").X;

			foreach( string word in words ) {
				Vector2 size = spriteFont.MeasureString(word);
				if( lineWidth + size.X < maxLineWidth ) {
					sb.Append(word + " ");
					lineWidth += size.X + spaceWidth;
				} else {
					sb.Append("\n" + word + " ");
					lineWidth = size.X + spaceWidth;
				}
			}
			return sb.ToString( );
		}

		/// <summary>
		/// Returns number of lines that will fit into given height
		/// </summary>
		/// <param name="f"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public int WrapLines(SpriteFont f, float height) =>
			(int) Math.Floor((double) ( height / f.MeasureString("U").Y ));
		
		/// <summary>
		/// Draws string with WordWrap
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="font"></param>
		/// <param name="text"></param>
		/// <param name="rect"></param>
		/// <param name="c"></param>
		public void DrawWrappedString(SpriteBatch sb, SpriteFont font, string text, Rectangle rect, Color c) {
			string wrapped = WrapText(font, text, rect.Width);
			string[] lines = wrapped.Split('\n');
			int maxLines = WrapLines(font, rect.Height);
			if( lines.Length > maxLines ) {
				for( int i = 0; i < maxLines; i++ )
					sb.DrawString(font, lines[i],
						new Vector2(rect.X, rect.Y + ( i * font.MeasureString(lines[i]).Y ) + ( i * 5 )),
						c
					);
			} else {
				int i = 0;
				foreach( string s in lines ) {
					sb.DrawString(font, s,
						new Vector2(rect.X, rect.Y + ( i * font.MeasureString(s).Y ) + ( i * 5 )),
						c
					);
					i++;
				}
			}
		}

		/// <summary>
		/// Draws Bars and its Text and Name
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="font"></param>
		/// <param name="dest"></param>
		/// <param name="n"></param>
		/// <param name="t"></param>
		/// <param name="nc"></param>
		/// <param name="tc"></param>
		public void Draw(SpriteBatch sb, SpriteFont font,
		Rectangle dest,
		string n, string t,
		Color? nc = null, Color? tc = null) {
			sb.Draw(Texture.Texture, dest, Texture.Source, Color.White);

			// TODO: Implement name centering!
			sb.DrawString(font, n, new Vector2(Name.X + dest.X, Name.Y + dest.Y), nc ?? Color.White);

			DrawWrappedString(sb, font, t,
			new Rectangle(Text.X + dest.X, Text.Y + dest.Y, Text.Width, Text.Height),
			tc ?? Color.White);
		}

	}
}
