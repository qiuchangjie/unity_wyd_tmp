using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace WYD
{
	public class SkillEditor : EditorWindow
	{// 技能编辑器
		const int UNKNOWN = 0;
		const int EFFECT = 1;
		const int SOUND = 2;
		const int DAMAGE = 3;

		public delegate void SkillEditorDelegate(SkillEditor self);

		public class EffectMember : Object
		{// 技能特效
			int m_frameIndex;
			public int frameIndex
			{
				get { return m_frameIndex; }
				set { m_frameIndex = value; }
			}
			Object m_effectObject;
			public Object effectObject
			{
				get { return m_effectObject; }
				set { m_effectObject = value; }
			}
			Object m_skeleton;
			public Object skeleton
			{
				get { return m_skeleton; }
				set { m_skeleton = value; }
			}
			bool m_isFollow;
			public bool isFollow
			{
				get { return m_isFollow; }
				set { m_isFollow = value; }
			}
		};

		public class SoundMember : Object
		{// 技能声音
			int m_frameIndex;
			public int frameIndex
			{
				get { return m_frameIndex; }
				set { m_frameIndex = value; }
			}
			string m_resourceFile;
			public string resourceFile
			{
				get { return m_resourceFile; }
				set { m_resourceFile = value; }
			}
		};

		public class DamageMember : Object
		{// 技能伤害
			int m_frameIndex;
			public int frameIndex
			{
				get { return m_frameIndex; }
				set { m_frameIndex = value; }
			}
			float m_damageValue;
			public float damageValue
			{
				get { return m_damageValue; }
				set { m_damageValue = value; }
			}
		};

		public class FoldoutItemBase : Object
		{
			public int type;
			public bool folded;
		}

		public class EffectFoldoutItem : FoldoutItemBase
		{// 折叠标签信息
			public EffectMember member;
		};

		public class SoundFoldoutItem : FoldoutItemBase
		{
			public SoundMember member;
		};

		public class DamageFoldoutItem : FoldoutItemBase
		{
			public DamageMember member;
		};

		private Object m_characterObj;
		public Object charactorObj
		{
			get { return m_characterObj; }
			set { m_characterObj = value; }
		}

		private string m_animationName;
		public string animationName
		{
			get { return m_animationName; }
			set { m_animationName = value; }
		}

		private List<EffectFoldoutItem> m_effectList = new List<EffectFoldoutItem>();
		public List<EffectFoldoutItem> effectList
		{
			get { return m_effectList; }
			set { m_effectList = value; }
		}

		private List<SoundFoldoutItem> m_soundList = new List<SoundFoldoutItem>();
		public List<SoundFoldoutItem> soundList
		{
			get { return m_soundList; }
			set { m_soundList = value; }
		}

		private List<DamageFoldoutItem> m_damageList = new List<DamageFoldoutItem>();
		public List<DamageFoldoutItem> damageList
		{
			get { return m_damageList; }
			set { m_damageList = value; }
		}

		
		private SkillEditorDelegate m_editorDelegate = null;
		public SkillEditorDelegate editorDelegate
        {
            get { return m_editorDelegate; }
            set { m_editorDelegate = value; }
        }

		private List<int> m_removeList = new List<int>();
		private Vector2 m_scrollPos = Vector2.zero;
		private static SkillEditor g_editor;

		[MenuItem("WYD/Skill Editor")]
		static void createWindow()
		{// 打开window
			g_editor = (SkillEditor)EditorWindow.GetWindow(typeof(SkillEditor));
		}

		static SkillEditor GetSkillEditorInstance()
		{
			if (g_editor == null)
			{
				g_editor = (SkillEditor)EditorWindow.GetWindow(typeof(SkillEditor));
			}
			return g_editor;
		}
		
		void OnGUI()
		{
			m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);

			m_characterObj = EditorGUILayout.ObjectField("角色", m_characterObj, typeof(Object), false);
			if (m_characterObj != null && m_characterObj.GetType().Equals(typeof(GameObject)))
			{
				string[] names = GetAnimationClipNames(((GameObject)m_characterObj).animation);
				int index = 0;
				for (index = names.Length - 1; index > 0; --index)
				{
					if (m_animationName != null && m_animationName.Equals(names[index]))
					{
						break;
					}
				}
				index = EditorGUILayout.Popup(index, names);
				m_animationName = names[index];
			}
			else
			{
				m_animationName = EditorGUILayout.TextField("动作", m_animationName);
			}
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("添加特效"))
			{// if click the button
				OnAddEffect();
			}
			if (GUILayout.Button("添加声音"))
			{// if click the button
				OnAddSound();
			}
			if (GUILayout.Button("添加伤害"))
			{// if click the button
				OnAddDamage();
			}
			if (GUILayout.Button("保存技能"))
			{
				OnSaveSkill();
			}
			GUILayout.EndHorizontal();
			OnDrawFoldouts();

			EditorGUILayout.EndScrollView();

            if (m_editorDelegate != null)
            {
                m_editorDelegate(this);
            }
		}

		void Update()
		{
		}

		void OnDestroy()
		{
			Debug.Log ("SkillEditor OnDestroy");
		}

		void OnDrawFoldouts()
		{// 绘制折叠标签
			//Debug.LogWarning(string.Format("m_itemList.Count = {0}", m_itemList.Count));
//			for (int i = 0; i < m_itemList.Count; i++)
//			{
//				switch (m_itemList[i].type)
//				{// 绘制各种折叠标签
//				case EFFECT:
//					m_itemList[i] = DrawEffectFoldout(m_itemList[i], i);
//					break;
//				case SOUND:
//					m_itemList[i] = DrawSoundFoldout(m_itemList[i], i);
//					break;
//				case DAMAGE:
//					m_itemList[i] = DrawDamageFoldout(m_itemList[i], i);
//					break;
//				default:
//					Debug.LogWarning(string.Format("不合法的类型 {0}", m_itemList[i].type));
//					break;
//				}
//			}
			Debug.LogWarning(string.Format("m_effectList.Count={0}, m_soundList.Count={1}, m_damageList.Count={2} ", m_effectList.Count, m_soundList.Count, m_damageList.Count));
			for (int i = 0; i < m_effectList.Count; ++i)
			{
				m_effectList[i] = DrawEffectFoldout(m_effectList[i], i);
			}
			for (int i = 0; i < m_soundList.Count; ++i)
			{
				m_soundList[i] = DrawSoundFoldout(m_soundList[i], i);
			}
			for (int i = 0; i < m_damageList.Count; ++i)
			{
				m_damageList[i] = DrawDamageFoldout(m_damageList[i], i);
			}
			ClearRemoveList();// 删除折叠标签
		}

		void OnAddEffect()
		{
			EffectFoldoutItem item = new EffectFoldoutItem();
			item.folded = true;
			item.type = EFFECT;
			item.member = new EffectMember();
			Object.DontDestroyOnLoad(item.member);
			Object.DontDestroyOnLoad(item);
			m_effectList.Add(item);
		}

		EffectFoldoutItem DrawEffectFoldout(EffectFoldoutItem item, int index)
		{
//			Debug.Log ("DrawEffectFoldout == ");
			if (item != null && item.member != null)
			{Debug.Log ("DrawEffectFoldout22 == ");
				GUILayout.BeginHorizontal();
				item.folded = EditorGUILayout.Foldout(item.folded, "特效");
				if (GUILayout.Button("删除"))
				{
					m_removeList.Add(index);// 添加到函数列表
				}
				GUILayout.EndHorizontal();
				EditorGUI.indentLevel = 1;
				if (item.folded)
				{
					item.member.frameIndex = EditorGUILayout.IntField("帧", item.member.frameIndex);
					item.member.effectObject = EditorGUILayout.ObjectField("特效对象", item.member.effectObject, typeof(Object), true);
					item.member.skeleton = EditorGUILayout.ObjectField("骨骼", item.member.skeleton, typeof(Object), true);
					item.member.isFollow = EditorGUILayout.Toggle("是否跟随", item.member.isFollow);
				}
				EditorGUI.indentLevel = 0;
			}
//			else
//			{
//				Debug.LogWarning("item == null or item.member == null");
//			}
			return item;
		}

		void OnAddSound()
		{
			SoundFoldoutItem item = new SoundFoldoutItem();
			item.folded = true;
			item.type = SOUND;
			item.member = new SoundMember();
			Object.DontDestroyOnLoad(item.member);
			Object.DontDestroyOnLoad(item);
			m_soundList.Add(item);
		}

		SoundFoldoutItem DrawSoundFoldout(SoundFoldoutItem item, int index)
		{
			GUILayout.BeginHorizontal();
			item.folded = EditorGUILayout.Foldout(item.folded, "声音");
			if (GUILayout.Button("删除"))
			{
				m_removeList.Add(index);
			}
			GUILayout.EndHorizontal();
			EditorGUI.indentLevel = 1;
			if (item.folded)
			{
				item.member.frameIndex = EditorGUILayout.IntField("帧", item.member.frameIndex);
				item.member.resourceFile = EditorGUILayout.TextField("声音文件", item.member.resourceFile);
			}
			EditorGUI.indentLevel = 0;
			return item;
		}

		void OnAddDamage()
		{
			DamageFoldoutItem item = new DamageFoldoutItem();
			item.folded = true;
			item.type = DAMAGE;
			item.member = new DamageMember();
			Object.DontDestroyOnLoad(item.member);
			Object.DontDestroyOnLoad(item);
			m_damageList.Add(item);
		}

		DamageFoldoutItem DrawDamageFoldout(DamageFoldoutItem item, int index)
		{
			GUILayout.BeginHorizontal();
			item.folded = EditorGUILayout.Foldout(item.folded, "伤害");
			if (GUILayout.Button("删除"))
			{
				m_removeList.Add(index);
			}
			GUILayout.EndHorizontal();
			EditorGUI.indentLevel = 1;
			if (item.folded)
			{
				item.member.frameIndex = EditorGUILayout.IntField("帧", item.member.frameIndex);
				item.member.damageValue = EditorGUILayout.FloatField("伤害值", item.member.damageValue);
			}
			EditorGUI.indentLevel = 0;
			return item;
		}

		void ClearRemoveList()
		{
			foreach (int i in m_removeList)
			{
//				if (m_itemList.Count > i && i >= 0)
//				{
//					m_itemList.RemoveAt(i);
//				}
			}
			m_removeList.Clear();
		}

		string[] GetAnimationClipNames(Animation ani)
		{// 获取动作列表
			string[] names = new string[ani.GetClipCount()];
			int i = 0;
			foreach (AnimationState state in ani)
			{
				names[i] = state.name;
				++i;
			}
			return names;
		}

		void CreateCharacter()
		{
			if (m_characterObj != null)
			{

				GameObject characterClone = (GameObject)Object.Instantiate(m_characterObj);

				foreach (EffectFoldoutItem item in m_effectList)
				{
					GameObject effectClone = (GameObject)Object.Instantiate(((EffectMember)item.member).effectObject);
					WYD.Skill.Effect effect = effectClone.AddComponent<WYD.Skill.Effect>();
					effect.animationName = animationName;
					effect.character = characterClone;
					effect.frameIndex = ((EffectMember)item.member).frameIndex;
					effect.AddAnimationEvent();
					characterClone.animation.Play (m_animationName);
					characterClone.animation.wrapMode = WrapMode.Loop;
				}
			}
		}

		// 序列化
		public static string Serialize<T>(T obj) 
		{
			try
			{
				IFormatter formatter = new BinaryFormatter();
				MemoryStream stream = new MemoryStream();
				formatter.Serialize(stream, obj);
				stream.Position = 0;
				byte[] buffer = new byte[stream.Length];
				stream.Read(buffer, 0, buffer.Length);
				stream.Flush();
				stream.Close();
				return System.Convert.ToBase64String(buffer); 
			}
			catch (System.Exception ex)
			{
				throw new System.Exception("序列化失败,原因:" + ex.Message);
			}
		}
		
		// 反序列化
		public static T Desrialize<T>(string str) 
		{
			T obj = default(T);
			try
			{
				IFormatter formatter = new BinaryFormatter();
				byte[] buffer = System.Convert.FromBase64String(str);
				MemoryStream stream = new MemoryStream(buffer);
				obj = (T)formatter.Deserialize(stream);
				stream.Flush();
				stream.Close();
			}
			catch (System.Exception ex)
			{
				throw new System.Exception("反序列化失败,原因:" + ex.Message);
			}
			return obj;
		}

		void OnSaveSkill()
		{
			CreateCharacter();
		}
	}

}// end namespace WYD
