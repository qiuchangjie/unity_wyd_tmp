using UnityEngine;
using System.Collections;

namespace WYD
{
	namespace Skill
	{
		public interface IEventCallback
		{
			void OnAnimationEvent(Object obj);
		}

		public class AnimationEventReceiver : MonoBehaviour 
		{
			void OnReceiveEvent(Object obj)
			{
				if (obj is IEventCallback)
				{
					((IEventCallback)obj).OnAnimationEvent(obj);
				}
			}
		}
	}// namespace WYD.Skill end
}// namespace WYD end
