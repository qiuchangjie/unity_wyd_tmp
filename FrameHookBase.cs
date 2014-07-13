using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WYD
{
	namespace Skill
	{
		public abstract class FrameHookBase : ScriptableObject, IEventCallback
		{
			protected MonoBehaviour m_parentScript;
			public MonoBehaviour parentScript
			{
				get { return m_parentScript; }
				set { m_parentScript = value; }
			}

			protected GameObject m_character = null;
			public GameObject character 
			{
				get { return m_character; }
				set { m_character = value; }
			}

			protected string m_animationName = string.Empty;
			public string animationName
			{
				get { return m_animationName; }
				set { m_animationName = value; }
			}

			protected int m_frameIndex = 0;
			public int frameIndex
			{
				get { return m_frameIndex; }
				set { m_frameIndex = value; }
			}

			public void AddAnimationEvent()
			{
				if (!m_animationName.Equals(string.Empty) && m_character != null)
				{
					Debug.Log ("AddAnimationEvent");
					//Object.DontDestroyOnLoad(this);

					AnimationEventReceiver receiver = m_character.GetComponent<AnimationEventReceiver>();
					if (receiver == null)
					{
						m_character.AddComponent("AnimationEventReceiver");
						receiver = m_character.GetComponent<AnimationEventReceiver>();
					}
					AnimationClip clip = m_character.animation[m_animationName].clip;
					float secPerFrame = 1.0f / clip.frameRate;
					AnimationEvent aniEvent = new AnimationEvent();
					aniEvent.time = ((float)m_frameIndex) * secPerFrame;
					aniEvent.functionName = "OnReceiveEvent";
					aniEvent.objectReferenceParameter = this;
					clip.AddEvent(aniEvent);
				}
			}

			public virtual void Awake()
			{
			}

			public virtual void Start()
			{
				Debug.Log ("FrameHookBase.Start()");
			}

			public virtual void Update()
			{
			}

			public virtual void OnAnimationEvent(Object obj)
			{
			}
		}
	}// namespace WYD.Skill end
}// namespace WYD end