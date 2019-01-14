using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using UnityEngine;
using Quantized.Game.Behavior;
using Quantized.Game.Behavior.Components;
using Quantized.Game.Behavior.Components.Actions;
using Quantized.Game.Behavior.Components.Composites;
using Quantized.Game.Behavior.Components.Decorators;

namespace Quantized.Game.Behavior.Window
{
    /// <summary>
    /// Quantizedbit (Komorowski Sebastian)
    /// WinCompositOneChild
    /// </summary>
    public class WinCompositOneChild : WinComposit 
    {
        #region methods
        public WinCompositOneChild(BComponent unit) : base(unit)
        {
        }

        protected override void DrawContentWindow (Rect r)
        {
            UnityEngine.GUI.DrawTexture(new Rect(r.position, IMG_SIZE), img);

            if (!isAddedUsed)
            {
                Rect position = ImgOptionsUnitRect(r);
                Rect posOptions = new Rect(position.position - new Vector2(IMG_OPIONS_SIZE * 1.1f, 1f), position.size);
                if (UnityEngine.GUI.Button(posOptions, imgOptions))
                {
                    UnitArrang.arrang.window.MenuMoreComponent(EventOptionsComponent);
                }

                if (unit.childrenComponents == null || unit.childrenComponents.Length < 1)
                {
                    Rect posAdd = new Rect(position.position + new Vector2(0f, r.yMax - IMG_OPIONS_SIZE * 0.9f), position.size);
                    if (UnityEngine.GUI.Button(posAdd, imgAdd))
                    {
                        UnitArrang.arrang.window.MenuComponentAdd(EventAddComponent);
                    }
                }

                Rect posRemove =  new Rect(position.position + new Vector2(IMG_OPIONS_SIZE * 1.1f, -1f), position.size);
                if (UnityEngine.GUI.Button(posRemove, imgRemove))
                {
                    UnitArrang.arrang.window.MenuComponentDelete(EventDeleteComponent, unit.name);
                }
            }
        }
        #endregion
    }
}