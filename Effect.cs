using UnityEngine;
using System.Collections;

namespace WYD
{
	namespace Skill
	{
		[AddComponentMenu("WYD/Effect")]
		public class Effect : FrameHookBase, IFrameHook
		{
			protected void OnAnimationEvent()
			{
				Debug.Log ("Effect OnAnimationEvent");
			}

			public void Play()
			{
			}

			void Awake()
			{
			}
			
			void Start()
			{
			}
			
			void Update()
			{
			}

		}
	}// namespace WYD.Skill end
}// namespace WYD end
