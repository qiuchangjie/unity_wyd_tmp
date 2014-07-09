using UnityEngine;
using System.Collections;

using WYD.Skill;

namespace WYD
{
	namespace Skill
	{
		[AddComponentMenu("WYD/Skill")]
		public class Skill : FrameHookBase, IFrameHook
		{
			protected void OnAnimationEvent()
			{
				Debug.Log ("Skill OnAnimationEvent");
			}

			public void Play()
			{
			}

			void Awake()
			{
			}

			void Start()
			{
				Debug.Log ("Skill Start");
				base.Start();
			}

			void Update()
			{
			}

		}
	}// namespace WYD.Skill end
}// namespace WYD end
