using System;
using System.Drawing;
using System.Collections;

namespace Dots {

	/// <summary>
	/// ��������������� ����� ��� ������ � �������
	/// </summary>
	[Serializable]
	public class Dot {

		public const int NUM_PLAYERS = 2;

		public const string SAVED_GAMES_EXTENSION = "sav";

		public const string SAVED_GAMES_FOLDER = "SavedGames";

		/// <summary>
		/// �������� ����� �������. ������ �������� = ����� ������
		/// </summary>
		public static Color[] PlayerColors = new Color[NUM_PLAYERS];

		public static SolidBrush[] DotsBrushes = new SolidBrush[NUM_PLAYERS];

		/// <summary>����� ������, ������� �������� ��� �����</summary>
		public int Owner;

		/// <summary>
		/// ���� true, ����� ��������� � �������������� ����, �.�. ���������
		/// </summary>
		public bool IsCatched;

		/// <summary>
		/// ���� ������, �� ����� �������� ������� ����
		/// </summary>
		public bool IsBounding;

		public Point Coord;

		private Point _Index;

		public Point Index {
			get {
				return _Index;
			}
		}

		// ===================================================================

		static Dot() {
			PlayerColors[0] = Color.Red;
			PlayerColors[1] = Color.Blue;
			
			DotsBrushes[0] = new SolidBrush(PlayerColors[0]);
			DotsBrushes[1] = new SolidBrush(PlayerColors[1]);
		}

		// --------------------------------------------------------------------

		public Dot(int XIndex, int YIndex) {
			_Index = new Point(XIndex, YIndex);
		}
	}

	// ===================================================================
	// ===================================================================
	// ===================================================================

	/// <summary>
	/// �������������� ����
	/// </summary>
	[Serializable]
	public class CapturedField {
		public Dot[] Dots;
		public Point[] Pts;

		/// <summary>������� ����</summary>
		public float Size;

		// --------------------------------------------------------------------
		/// <summary>
		/// ������� ������� ������� � �������.
		/// </summary>
		public static float CalcContourSquare(Stack Contour) {
			float res = 0;
			int i;
			Point[] Coord = new Point[Contour.Count];
			IEnumerator ConEnumer = Contour.GetEnumerator();

			for(i = 0, ConEnumer.MoveNext(); i < Contour.Count; i++, ConEnumer.MoveNext()) {
				Coord[i] = ((Dot)ConEnumer.Current).Coord;
			}
			Coord[0] = Coord[Coord.Length - 1];
			i = 0;
			do {
				res += (Coord[i].X + Coord[i+1].X) * (Coord[i].Y - Coord[i+1].Y);
				i++;
			} while(i < Coord.Length - 1);
			return 0.5f * Math.Abs(res);
		}

		// --------------------------------------------------------------------

		/// <summary>
		/// ��������� �� �������� ����� ������ ��������������
		/// </summary>
		public static bool PointInPolygon(Point pt, Point[] Polygon) {
			bool a, b, res = false;
			int i = 0;
			
			if(Polygon[Polygon.Length - 1] != Polygon[0])
				Polygon[0] = Polygon[Polygon.Length - 1];
			do {
				a = pt.Y > Polygon[i].Y;
				b = pt.Y <= Polygon[i+1].Y;
				if( !((a == true && b == false) || (a == false && b == true)) ) {
					if(pt.X - Polygon[i].X < (pt.Y - Polygon[i].Y) * 
						(Polygon[i+1].X - Polygon[i].X) / (Polygon[i+1].Y - Polygon[i].Y))
						res = !res;
				}
				i++;
			} while(i < Polygon.Length - 1);
			return res;
		}

		public static bool PointInPolygon(Point pt, Stack Contour) {
			Point[] Coord = new Point[Contour.Count];
			IEnumerator ConEnumer = Contour.GetEnumerator();
			int i;

			for(i = 0, ConEnumer.MoveNext(); i < Contour.Count;
				i++, ConEnumer.MoveNext())
					Coord[i] = ((Dot)ConEnumer.Current).Coord;
			Coord[0] = Coord[Coord.Length - 1];
			return PointInPolygon(pt, Coord);
		}
	}

	// ===================================================================
	// ===================================================================
	// ===================================================================

	/// <summary>
	/// ���������, ���������� ���������� �� ������
	/// </summary>
	[Serializable]
	public struct Statistics {
		/// <summary>������ ����������� ������� ���������� � ��������</summary>
		public float TerritorySize;

		/// <summary>���������� �������, ������� ����� �����</summary>
		public int numFileds;

		/// <summary>���-�� ��������, ������ ������� ��� ��������� �����</summary>
		public int numEmptyFields;

		/// <summary>���-�� �����, ������� �������� �����</summary>
		public int numDots;
	}
}
