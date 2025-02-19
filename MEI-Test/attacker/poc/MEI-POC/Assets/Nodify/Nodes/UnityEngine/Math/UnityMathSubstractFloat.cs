using UnityEngine;
using System.Collections;
using Nodify.Runtime;

namespace Nodify.Runtime.Nodes
{
	[CreateMenu("Unity/Math/Substract/Float", "Math.SubstractFloat" )]
	public class UnityMathSubstractFloat : Node 
	{
		[Expose]
		public float value1;
		
		[Expose]
		public float value2;
		
		[Expose]
		public float result;

		protected override void OnExecute()
		{
			result = value1 - value2;
			base.OnExecute();
		}
	}
}
