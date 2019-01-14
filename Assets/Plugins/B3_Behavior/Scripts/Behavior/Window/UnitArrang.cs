using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using UnityEngine;
using Quantized.Game.Behavior.Components;
using Quantized.Game.Behavior.Components.Actions;
using Quantized.Game.Behavior.Components.Composites;
using Quantized.Game.Behavior.Components.Decorators;

namespace Quantized.Game.Behavior.Window
{
	/// <summary>
	/// Quantizedbit (Komorowski Sebastian)
	/// Rozmieszczenie elementów w głównym okinie
	/// </summary>
	public class UnitArrang 
	{
		#region values
		// FIXME SEBA - to powinno być zrobione poprzez properties
		public static UnitArrang arrang = null;

		private List<List<BComponent>> rows = new List<List<BComponent>>();
		private int currentRowId = -1;
		#endregion

		#region properties
		public IEditWindow window
		{
			get;
			private set;
		}

		public Vector2 areaSize
		{
			get;
			private set;
		}

		public Vector2 offsetLeft
		{
			get;
			private set;
		}

		private int findLongestRow
		{
			get
			{
				int result = 0;
				int resultCount = 0;
				int currentCount = 0;
				for (int i = 0, j = rows.Count; i < j; ++i)
				{
					currentCount = rows[i].Count;
					if (currentCount > resultCount)
					{
						result = i;
						resultCount = currentCount;
					}
				}
				return result;
			}
		}
		#endregion

		#region methods - msg
		/// <summary>
		/// rozstawienie elementów GUI - okienek komponentów
		/// </summary>
		public void OnGUICalculate()
		{
			if (rows == null || rows.Count < 1)
			{
				return;
			}

			// down
			Vector2 position = new Vector2(UnitWindow.WINDOW_FRAMES.x, currentRowId * (UnitWindow.WINDOW_BEHAVIOR_UNIT_SIZE.y + UnitWindow.WINDOW_FRAMES.y));
			List<BComponent> current = rows[currentRowId];

			foreach (BComponent ut in current)
			{
				ut.window.OnGUICalculate(position);
				position.x = position.x + UnitWindow.offsetHorizontal.x;
			}

			// up
			LinkedList<BComponent> separatelyUnits = new LinkedList<BComponent>();
			List<BComponent> pastedUnits = new List<BComponent>();
			position = new Vector2(UnitWindow.WINDOW_FRAMES.x, currentRowId * UnitWindow.offsetVertical.y);

			for (int i = currentRowId - 1; i >= 0; --i)
			{
				separatelyUnits.Clear();
				pastedUnits.Clear();

				position.y -= UnitWindow.offsetVertical.y;
				current = rows[i];

				for (int ii = 0, jj = current.Count; ii < jj; ++ii)
				{
					if (current[ii].window.CenterBetweenTwoChildren(position))
					{
						pastedUnits.Add(current[ii]);
					} 
					else
					{
						separatelyUnits.AddLast(current[ii]);
					}
				}

				foreach (BComponent u in separatelyUnits)
				{
					int idLeft = u.locationArrang.coll - 1;
					if (idLeft < 0)
					{
						int find = pastedUnits.FindIndex(x => x.locationArrang.coll > u.locationArrang.coll);
						if (find > -1)
						{
							u.window.PutNextPositionLeft(pastedUnits[find].window.rect.position);
							pastedUnits.Insert(find, u);
						}
					}
					else 
					{
						int find = pastedUnits.FindIndex(x => x.locationArrang.coll > u.locationArrang.coll);
						if (find > -1)
						{
							Vector2 currentPosition = pastedUnits[find - 1].window.rect.position;
							Vector2 rightPosition = UnitWindow.GetNextPositionRight(currentPosition);
							for (int ii = find - 1; ii < pastedUnits.Count; ++ii)
							{
								if (pastedUnits[ii].window.IsOverlapsWindows(rightPosition))
								{
									for (; ii < pastedUnits.Count; ++ii)
									{
										pastedUnits[ii].window.TransformCurrentAndChildren(UnitWindow.offsetHorizontal);
									}
									break;
								}
							}

							u.window.PutNextPositionRight(currentPosition);
							pastedUnits.Insert(find, u);
						}
						else
						{
							u.window.PutNextPositionRight(pastedUnits[pastedUnits.Count - 1].window.rect.position);
							pastedUnits.Add(u);
						}
					}

				}
			}

			// find area size
			Vector2 right = new Vector2(position.x, 0f);
			Vector2 left = new Vector2(float.PositiveInfinity, 0f);
			current = null;

			for (int i = 0, j = rows.Count; i < j; ++i)
			{
				current = rows[i];
				if (current != null && current.Count > 0)
				{
					Rect r = current[0].window.rect;
					if (r.xMin < left.x)
					{
						left.x = r.xMin - UnitWindow.WINDOW_FRAMES.x;
					}
					r = current[current.Count - 1].window.rect;
					if (r.xMax > right.x)
					{
						right.x = r.xMax + UnitWindow.WINDOW_FRAMES.x;
					}
				}
			}
			offsetLeft = left;
            areaSize = new Vector2( right.x - left.x, (rows.Count - 1) * UnitWindow.offsetVertical.y + UnitWindow.WINDOW_FRAMES.y);
		}

		public void OnGUI(System.Action repaintEvent, float unitVisible)
		{
			List<BComponent> current = null;
			for (int i = 0, j = rows.Count; i < j; ++i)
			{
				current = rows[i];
				foreach (BComponent ut in current)
				{
					ut.window.OnGUI (-offsetLeft, repaintEvent, unitVisible);
				}
			}
		}
		#endregion

		#region methods - static
		public static void CleanArrang()
		{
			// TODO SEBA ...
		}
		#endregion

		#region methods
		public UnitArrang(BComponent units, IEditWindow window)
		{
			this.window = window;
			rows.Add(new List<BComponent>());

			BuildArrangMap(units, 0);
			currentRowId = findLongestRow;
		}

		/// <summary>
		/// Wyszukujemy konkretny komponent ze wskazanej lokacji
		/// </summary>
		/// <param name="location">Location.</param>
		public BComponent Find(Location location)
		{
			BComponent result = null;
			if (location.row < rows.Count)
			{
				List<BComponent> row = rows[location.row];
				if (location.coll < row.Count)
				{
					result = row[location.coll];
				}
			}
			return result;
		}

		protected void BuildArrangMap(BComponent unit, int currentId)
		{
			if (unit == null)
			{
				return;
			}

			rows[currentId].Add(unit);
			unit.locationArrang = new Location(currentId, rows[currentId].Count - 1);

			if ((rows.Count - 1) < (currentId + 1))
			{
				rows.Add(new List<BComponent>());
			}
			if (unit.childrenComponents != null)
			{
				foreach (BComponent u in unit.childrenComponents)
				{
					BuildArrangMap(u, currentId + 1);
				}
			}
		}
		#endregion
	}
}