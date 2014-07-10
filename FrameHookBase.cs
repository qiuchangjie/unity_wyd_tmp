using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WYD
{
	namespace Skill
	{
		public abstract class FrameHookBase : MonoBehaviour
		{

			public GameObject m_targetObject = null;
			public string m_animationName = string.Empty;
			public int m_frameIndex = 0;
			public string m_resourceFile = string.Empty;
			public List<FrameHookBase> m_children = new List<FrameHookBase>();

			protected virtual void Start()
			{
				AddAnimationEvent();
			}

			protected virtual void AddAnimationEvent()
			{
				if (!m_animationName.Equals(string.Empty) && m_targetObject != null)
				{
					AnimationEventReceiver receiver = m_targetObject.GetComponent<AnimationEventReceiver>();
					if (receiver == null)
					{
						m_targetObject.AddComponent("AnimationEventReceiver");
						receiver = m_targetObject.GetComponent<AnimationEventReceiver>();
					}
					receiver.eventDelegate = OnAnimationEvent;
					AnimationClip clip = m_targetObject.animation[m_animationName].clip;
					float secPerFrame = 1.0f / clip.frameRate;
					AnimationEvent aniEvent = new AnimationEvent();
					aniEvent.time = ((float)m_frameIndex) * secPerFrame;
					aniEvent.functionName = "OnReceiveEvent";
					aniEvent.objectReferenceParameter = this;
					clip.AddEvent(aniEvent);
				}
			}

			protected virtual void OnAnimationEvent(Object arg)
			{
				Debug.Log ("FrameHookBase OnAnimationEvent");
			}
		}
	}// namespace WYD.Skill end
}// namespace WYD end