using Microsoft.Xna.Framework;


namespace ITW.Exts {

	/// <summary>
	/// StringRectangle is a representation of a <see cref="string"/> enclosed into a <see cref="Rectangle"/>
	/// </summary>
	public class StringRectangle {

		private string String;
		private Rectangle Rect;

		public StringRectangle(string s, Rectangle r){
			String = s;
			Rect = r;
		}

	}

}