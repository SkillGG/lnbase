using Microsoft.Xna.Framework;

namespace ITW.Exts {
	public struct Line1D {

		public int X;
		public int Y;
		private int L;

		public Point Location => new Point(X, Y);
		public int Length { get => L; set => L = value; }

		public Line1D(int x, int y, int l){
			X = x;
			Y = y;
			L = l;
		}

		public Line1D(Point p, int l){
			X = p.X;
			Y = p.Y;
			L = l;
		}

	}

	public struct Line2D {
		public Point Start;
		public Point End;

		public Line2D(Point s, Point e){
			this.Start = s;
			this.End = e;
		}

		public Line2D(int x1, int y1, int x2, int y2){
			Start = new Point(x1, y1);
			End = new Point(x2, y2);
		}

	}
}
