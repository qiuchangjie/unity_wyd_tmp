using UnityEngine;
using System.Collections;

namespace WYD
{
	namespace Skill
	{
		[AddComponentMenu("WYD/Sound")]
		public class Sound : FrameHookBase, IFrameHook
		{
			protected void OnAnimationEvent()
			{
				Debug.Log ("Sound OnAnimationEvent");
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
