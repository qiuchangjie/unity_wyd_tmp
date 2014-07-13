using UnityEngine;
using System.Collections;

namespace WYD
{
	namespace Skill
	{
		[AddComponentMenu("WYD/Effect")]
		public class Effect : FrameHookBase
		{
			public override void OnAnimationEvent(Object arg)
			{
				Debug.Log ("Effect OnAnimationEvent");
			}

			public override void Awake()
			{
			}
			
			public override void Start()
			{
				Debug.Log ("Effect.Start()");
				//AddAnimationEvent();
			}
			
			public override void Update()
			{
			}

			public void Play()
			{
			}

		}
	}// namespace WYD.Skill end
}// namespace WYD end
