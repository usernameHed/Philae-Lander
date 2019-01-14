using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Quantized.Game.Behavior.Components;

namespace Quantized.Game.Behavior.Window
{
	/// <summary>
	/// Quantizedbit (Komorowski Sebastian)
	/// I edit window.
	/// </summary>
	public interface IEditWindow
	{
		#region properties
		Behavior.ComponentsManager components
		{
			get;
		}
		#endregion

		#region methods
		void MakeArrangGraph();
        void DrawBezier(Rect currentRect, Rect r, float visibleScale, float usedColorScale, float offsetUp);
		void SaveAsset(XmlDocument xml, TextAsset txt);
        void FadeOnUnits ();
        void FadeOffUnits (float time = 0f);
        string GetAssetTextPath(TextAsset txt);
        void AnimButtonsAdds(float size, System.Action<float> refresh);
        void AddSomeEvent(System.Func<bool> waitUtilTrue, System.Action action);
        void WaitEventFadeOff(System.Action action);
        void AnimateRemoveUnit(UnitWindow removeUnit, System.Action endAnimEvent);
        void OffsetMoveBg();
		#endregion

		#region methods - menu actions
		void MenuActionMore(System.Action<object> evt);
        void MenuActionDelete(System.Action<object> evt, string actionName);
		void MenuMoreComponent(System.Action<object> evt);
        void MenuMoreComponentRandom(System.Action<object> evt, BComponent unit);
        void MenuComponentDelete(System.Action<object> evt, string name);
		void MenuComponentAdd(System.Action<object> evt);
		#endregion
	}
}