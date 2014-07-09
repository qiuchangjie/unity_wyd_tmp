using UnityEngine;
using System.Collections;

namespace WYD
{
	namespace Skill
	{
		[AddComponentMenu("WYD/Damage")]
		public class Damage : FrameHookBase, IFrameHook 
		{
			protected void OnAnimationEvent()
			{
				Debug.Log ("Damage OnAnimationEvent");
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
