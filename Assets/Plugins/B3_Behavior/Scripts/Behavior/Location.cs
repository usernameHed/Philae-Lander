using UnityEngine;
using System.Collections;

namespace Quantized.Game.Behavior
{
	/// <summary>
	/// Quantizedbit (Komorowski Sebastian)
	/// Location.
	/// </summary>
	public class Location
	{
		#region properties
		public int row
		{
			get;
			private set;
		}

		public int coll
		{
			get;
			private set;
		}
		#endregion

		#region methods
		public Location(int r, int c)
		{
			row = r;
			coll = c;
		}

		public Location(Location l)
		{
			row = l.row;
			coll = l.coll;
		}

		public override string ToString ()
		{
			return string.Format ("L [{0},{1}]", row, coll);
		}
		#endregion
	}
}