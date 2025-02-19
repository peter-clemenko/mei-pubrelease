using UnityEngine;
using System.Collections;
using Nodify.Runtime;

namespace Nodify.Runtime.Nodes
{
	[CreateMenu("Nodify/Variable/Get", "Variable.Get")]
	public class NodifyVariableGet : Node 
	{
        [Expose]
        public string keyName;

        [Expose]
        public object result;

        /// <summary>
        /// Should the node continue if the
        /// result is null?
        /// </summary>
        public bool continueIfNull = false;

		protected override void OnExecute()
		{
            result = GlobalVariableBase.GetValue(keyName, false);

            if (result != null)
            {
                base.OnExecute();
            }
            else
            {
                if(continueIfNull)
                {
                    base.OnExecute();
                }
            }
		}
	}
}
