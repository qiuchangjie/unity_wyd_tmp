using UnityEngine;
using System.Collections;

namespace WYD
{
	namespace Skill
	{
		[AddComponentMenu("WYD/Damage")]
		public class Damage : FrameHookBase
		{
			public override void OnAnimationEvent(Object arg)
			{
				Debug.Log ("Damage OnAnimationEvent");
			}

			public void Play()
			{
			}

		}
	}// namespace WYD.Skill end
}// namespace WYD end
