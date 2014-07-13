using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WYD
{
	namespace Skill
	{
		[AddComponentMenu("WYD/Skill")]
		public class Skill : MonoBehaviour
		{
			private List<FrameHookBase> m_skillElements = new List<FrameHookBase>();
			public List<FrameHookBase> skillElements
			{
				get { return m_skillElements; }
				set { m_skillElements = value; }
			}

			public void AddSkillElement(FrameHookBase element)
			{
				if (m_skillElements == null)
				{
					m_skillElements = new List<FrameHookBase>();
				}
				m_skillElements.Add(element);
			}

			void Start()
			{
				//Debug.Log ("Skill.Start()");
				foreach (FrameHookBase element in m_skillElements)
				{
					if (!element.Equals(null))
					{
						element.Start();
					}
				}
				//AddAnimationEvent();
			}

			void Update()
			{
				foreach (FrameHookBase element in m_skillElements)
				{
					if (!element.Equals(null))
					{
						element.Update();
					}
				}
			}
		}
	}// namespace WYD.Skill end
}// namespace WYD end
