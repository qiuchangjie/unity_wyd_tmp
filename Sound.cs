using UnityEngine;
using System.Collections;

namespace WYD
{
	namespace Skill
	{
		[AddComponentMenu("WYD/Sound")]
		public class Sound : FrameHookBase
		{
			public override void OnAnimationEvent(Object arg)
			{
				Debug.Log ("Sound OnAnimationEvent");
			}

			public void Play()
			{
			}
		}
	}// namespace WYD.Skill end
}// namespace WYD end
