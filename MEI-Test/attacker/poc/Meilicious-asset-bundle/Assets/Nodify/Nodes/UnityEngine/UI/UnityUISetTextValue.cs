using UnityEngine;
using System.Collections;
using Nodify.Runtime;

namespace Nodify.Runtime.Nodes
{
	[CreateMenu("Unity/UI/Set Text Value", "UI.SetTextValue" )]
	public class UnityUISetTextValue : Node 
	{
		[Expose]
		public UnityEngine.UI.Text target;

		[Expose]
		public string value;

		protected override void OnExecute()
		{
			target.text = value;
			base.OnExecute();
		}
	}
}
