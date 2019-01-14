using UnityEngine;
using System.Collections;
using Quantized.Game.Behavior.Components;

namespace Quantized.Game.Behavior
{
	public class ReturnResult
	{
		#region values
		public BComponent componentNext;
		public bool? result;
		public bool isAction;

        public bool isUntilTrue;
        public bool isUntilFalse;
		#endregion

		#region methods
		public ReturnResult(BComponent cNect, bool? result)
		{
			this.componentNext = cNect;
			this.result = result;
			this.isAction = false;
		}

		public ReturnResult(BComponent cNect, bool? result, bool isAction)
		{
			this.componentNext = cNect;
			this.result = result;
			this.isAction = isAction;
		}
		#endregion	
	}
}