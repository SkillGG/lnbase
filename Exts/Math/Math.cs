using Microsoft.Xna.Framework;

namespace ITW {
	public class Math {
		public static Point Vec2Point(Vector2 v) => new Point((int) v.X, (int) v.Y);
		public static Vector2 Point2Vec(Point p) => new Vector2(p.X, p.Y);
	}
}