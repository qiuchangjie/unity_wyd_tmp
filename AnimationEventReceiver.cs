using UnityEngine;
using System.Collections;

namespace WYD
{
	namespace Skill
	{
		public delegate void AnimationEventDelegate(Object arg);

		public class AnimationEventReceiver : MonoBehaviour 
		{
			AnimationEventDelegate m_eventDelegate = null;
			public AnimationEventDelegate eventDelegate
			{
				get { return m_eventDelegate; }
				set { m_eventDelegate = value; }
			}

			void OnReceiveEvent(Object arg)
			{
				if (m_eventDelegate != null)
				{
					m_eventDelegate(arg);
				}
			}

			// Use this for initialization
			void Start () 
			{
			
			}
			
			// Update is called once per frame
			void Update () 
			{
			
			}
		}
	}// namespace WYD.Skill end
}// namespace WYD end
