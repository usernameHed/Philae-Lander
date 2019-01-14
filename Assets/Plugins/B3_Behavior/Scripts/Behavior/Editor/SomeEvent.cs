using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Quantized.Game.Behavior.Edit
{
    public class SomeEvent 
    {
        #region def
        class CurrentEvent
        {
            #region values
            public System.Action action;
            public System.Func<bool> stayUntilTrue;
            #endregion

            #region methds
            public CurrentEvent(System.Func<bool> stayUntilTrue, System.Action action)
            {
                this.action = action;
                this.stayUntilTrue = stayUntilTrue;
            }
            #endregion
        }
        #endregion

        #region values
        private List<CurrentEvent> events = new List<CurrentEvent>();
        private List<CurrentEvent> delEvent = new List<CurrentEvent>();
        #endregion

        #region methods
        public void AddEvent(System.Func<bool> stayUntilTrue, System.Action action)
        {
            if (stayUntilTrue == null || action == null)
            {
                return;
            }

            events.Add(new CurrentEvent(stayUntilTrue, action));
        }

        public void Update()
        {
            delEvent.Clear();

            foreach (CurrentEvent e in events)
            {
                if (e.stayUntilTrue())
                {
                    delEvent.Add(e);
                    if (e.action != null)
                    {
                        e.action();
                    }
                }
            }

            foreach (CurrentEvent e in delEvent)
            {
                events.Remove(e);
            }
        }
        #endregion
    }
}