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
		static int lastSaveTempFileTime = 0;
		static int lastReadTempFileTime = 0;
		public delegate void SkillEditorDelegate(SkillEditor self);

		public class FoldoutItemBase
		{
			public int type;	// 类型
			public bool folded;	// 折叠标签是否展开
		}

		public class EffectFoldoutItem : FoldoutItemBase
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
			/*Object m_skeleton;
			public Object skeleton
			{
				get { return m_skeleton; }
				set { m_skeleton = value; }
			}*/
			string m_skeletonName;
			public string skeletonName
			{
				get { return m_skeletonName; }
				set { m_skeletonName = value; }
			}
			bool m_isFollow;
			public bool isFollow
			{
				get { return m_isFollow; }
				set { m_isFollow = value; }
			}
		};

		public class SoundFoldoutItem : FoldoutItemBase
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

		public class DamageFoldoutItem : FoldoutItemBase
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

		private List<FoldoutItemBase> m_itemList = new List<FoldoutItemBase>();
		public List<FoldoutItemBase> itemList
		{
			get { return m_itemList; }
			set { m_itemList = value; }
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

			m_characterObj = EditorGUILayout.ObjectField("角色", m_characterObj, typeof(Object), true);
            
			if (m_characterObj != null && m_characterObj.GetType().Equals(typeof(GameObject)))
			{
				string[] names = GetAnimationClipNames(((GameObject)m_characterObj).animation);
				int index = 0;
				for (index = names.Length - 1; index >= 0; --index)
				{
					if (m_animationName != null && m_animationName.Equals(names[index]))
					{
						break;
					}
				}
				index = (index >= 0 ? index : 0);
				index = EditorGUILayout.Popup("动作", index, names);
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

		void OnFocus()
		{
			int currentSecond = System.Environment.TickCount / 1000;
			if (lastReadTempFileTime == 0 || currentSecond - lastReadTempFileTime > 1)
			{
				lastReadTempFileTime = currentSecond;
				LoadListInfoFromTemp();
			}

		}

		void OnLostFocus()
		{
			int currentSecond = System.Environment.TickCount / 1000;
			if (lastSaveTempFileTime == 0 || currentSecond - lastSaveTempFileTime > 1)
			{
				lastSaveTempFileTime = currentSecond;
				SaveListInfoToTemp();
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
			for (int i = 0; i < m_itemList.Count; i++)
			{
				switch (m_itemList[i].type)
				{// 绘制各种折叠标签
				case EFFECT:
					m_itemList[i] = DrawEffectFoldout((EffectFoldoutItem)m_itemList[i], i);
					break;
				case SOUND:
					m_itemList[i] = DrawSoundFoldout((SoundFoldoutItem)m_itemList[i], i);
					break;
				case DAMAGE:
					m_itemList[i] = DrawDamageFoldout((DamageFoldoutItem)m_itemList[i], i);
					break;
				default:
					Debug.LogWarning(string.Format("不合法的类型 {0}", m_itemList[i].type));
					break;
				}
			}
//			Debug.LogWarning(string.Format("m_effectList.Count={0}, m_soundList.Count={1}, m_damageList.Count={2} ", m_effectList.Count, m_soundList.Count, m_damageList.Count));
			ClearRemoveList();// 删除折叠标签
		}

		void OnAddEffect()
		{
			EffectFoldoutItem item = new EffectFoldoutItem();
			if (item == null)
			{
				Debug.LogWarning("item is null");
			}
			item.folded = true;
			item.type = EFFECT;
			m_itemList.Add(item);
		}

		EffectFoldoutItem DrawEffectFoldout(EffectFoldoutItem item, int index)
		{
			if (item != null)
			{
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
					item.frameIndex = EditorGUILayout.IntField("帧", item.frameIndex);
					item.effectObject = EditorGUILayout.ObjectField("特效对象", item.effectObject, typeof(Object), true);
					//item.skeleton = EditorGUILayout.ObjectField("骨骼", item.skeleton, typeof(Object), true);
					if (m_characterObj != null && ((GameObject)m_characterObj).GetComponentsInChildren<SkinnedMeshRenderer>().Length > 0)
					{
						int nameIndex = 0;
						string[] names = GetBoneNames();
						for (nameIndex = 0; nameIndex < names.Length; ++nameIndex)
						{
							if (names[nameIndex].Equals(item.skeletonName))
							{
								break;
							}
						}

						nameIndex = (nameIndex >= names.Length ? 0 : nameIndex);
						nameIndex = EditorGUILayout.Popup("骨骼", nameIndex, names);
						item.skeletonName = names[nameIndex];
					}
					else
					{
						item.skeletonName = EditorGUILayout.TextField("骨骼", item.skeletonName);
					}
					item.isFollow = EditorGUILayout.Toggle("是否跟随", item.isFollow);
				}
				EditorGUI.indentLevel = 0;
			}
			else
			{
				Debug.LogWarning("item == null ");
			}
			return item;
		}

		void OnAddSound()
		{
			SoundFoldoutItem item = new SoundFoldoutItem();
			item.folded = true;
			item.type = SOUND;
			m_itemList.Add(item);
		}

		SoundFoldoutItem DrawSoundFoldout(SoundFoldoutItem item, int index)
		{
			if (item != null)
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
                    item.frameIndex = EditorGUILayout.IntField("帧", item.frameIndex);
                    item.resourceFile = EditorGUILayout.TextField("声音文件", item.resourceFile);
                }
                EditorGUI.indentLevel = 0;
            }
            return item;
        }

		void OnAddDamage()
		{
			DamageFoldoutItem item = new DamageFoldoutItem();
			item.folded = true;
			item.type = DAMAGE;
			m_itemList.Add(item);
		}

		DamageFoldoutItem DrawDamageFoldout(DamageFoldoutItem item, int index)
		{
			if (item != null)
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
                    item.frameIndex = EditorGUILayout.IntField("帧", item.frameIndex);
                    item.damageValue = EditorGUILayout.FloatField("伤害值", item.damageValue);
                }
            }
            EditorGUI.indentLevel = 0;
            return item;
        }

		void ClearRemoveList()
		{
			foreach (int i in m_removeList)
			{
				if (m_itemList.Count > i && i >= 0)
				{
					m_itemList.RemoveAt(i);
				}
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
	
		string GetListDataString()
		{// 根据List生成字符串
			string result = string.Empty;
			for (int i = 0; i < m_itemList.Count; ++i)
			{
				switch (m_itemList[i].type)
				{
				case EFFECT:
					{
						EffectFoldoutItem item = (EffectFoldoutItem)m_itemList[i];
						result = result + string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\n", i, item.type, item.folded, item.frameIndex, 
						                                AssetDatabase.GetAssetPath(item.effectObject), /*AssetDatabase.GetAssetPath(item.skeleton)*/item.skeletonName, item.isFollow);
					}
					break;
				case SOUND:
					{
						SoundFoldoutItem item = (SoundFoldoutItem)m_itemList[i];
						result = result + string.Format("{0}\t{1}\t{2}\t{3}\t{4}\n", i, item.type, item.folded, item.frameIndex, item.resourceFile);
					}
					break;
				case DAMAGE:
					{
						DamageFoldoutItem item = (DamageFoldoutItem)m_itemList[i];
						result = result + string.Format("{0}\t{1}\t{2}\t{3}\t{4}\n", i, item.type, item.folded, item.frameIndex, item.damageValue);
					}
					break;
				default:
					Debug.LogWarning("SaveListInfo: unknown item type");
					break;
				}
			}
			return result;
		}

		void SaveListInfoToTemp()
		{// 保存字符串
			Debug.Log("SaveListInfoToTemp: " + Application.temporaryCachePath + "/SkillEditor.data");
			string str = GetListDataString();
			FileUtil.DeleteFileOrDirectory(Application.temporaryCachePath + "/SkillEditor.data");
			FileStream fs = new FileStream(Application.temporaryCachePath + "/SkillEditor.data", FileMode.CreateNew);
			StreamWriter sw = new StreamWriter(fs);
			sw.Write(str);
			sw.Flush();
			sw.Close();
			fs.Close();
		}

		void LoadListInfoFromTemp()
		{// 加载界面状态
			FileStream fs = new FileStream(Application.temporaryCachePath + "/SkillEditor.data", FileMode.Open);
			StreamReader sr = new StreamReader(fs);
			m_itemList.Clear();
			for (string line = sr.ReadLine(); (line != null && !line.Equals(string.Empty)) && (line.Length > 0); line = sr.ReadLine())
			{
				string[] subs = line.Split('\t');
				if (subs.Length > 3)
				{
					int i = int.Parse(subs[0]);
					int type = int.Parse(subs[1]);
					FoldoutItemBase item = GetListItemFromStrings(subs, type);
					if (item != null)
					{
						m_itemList.Insert(i, item);
					}
				}
			}
			sr.Close();
			fs.Close();
		}

		FoldoutItemBase GetListItemFromStrings(string[] strs, int type)
		{// 解析字段生成相应类型的Item
			if (type == EFFECT)
			{
				EffectFoldoutItem item = new EffectFoldoutItem();
				item.type = type;
				item.folded = bool.Parse(strs[2]);
				item.frameIndex = int.Parse(strs[3]);
				if (strs[4].Length > 0)
				{
					item.effectObject = AssetDatabase.LoadAssetAtPath(strs[4], typeof(GameObject));
				}
				else
				{
					item.effectObject = null;
				}
				/*
				if (strs[5].Length > 0)
				{
					item.skeleton = AssetDatabase.LoadAssetAtPath(strs[5], typeof(GameObject));
				}
				else
				{
					item.skeleton = null;
				}*/
				item.skeletonName = strs[5];
				item.isFollow = bool.Parse(strs[6]);
				return item;
			}
			if (type == SOUND)
			{
				SoundFoldoutItem item = new SoundFoldoutItem();
				item.type = type;
				item.folded = bool.Parse(strs[2]);
				item.frameIndex = int.Parse(strs[3]);
				item.resourceFile = strs[4];
				return item;
			}
			if (type == DAMAGE)
			{
				DamageFoldoutItem item = new DamageFoldoutItem();
				item.type = type;
				item.folded = bool.Parse(strs[2]);
				item.frameIndex = int.Parse(strs[3]);
				item.damageValue = float.Parse(strs[4]);
				return item;
			}
			return null;
		}

		string [] GetBoneNames()
		{
			SkinnedMeshRenderer[] components = ((GameObject)m_characterObj).GetComponentsInChildren<SkinnedMeshRenderer>();
			int bonesCount = 0;
			foreach (SkinnedMeshRenderer smr in components)
			{
				bonesCount += smr.bones.Length;
			}
			string[] names = new string[bonesCount];
			int i = 0;
			foreach (SkinnedMeshRenderer smr in components)
			{
				foreach (Transform t in smr.bones)
				{
					names[i] = t.name;
					++i;
				}
			}
			return names;
		}

		void CreateCharacter()
		{
			if (m_characterObj != null)
			{
				GameObject characterClone = (GameObject)Object.Instantiate(m_characterObj);

				foreach (FoldoutItemBase item in m_itemList)
				{
					if (item.type == EFFECT)
					{
						EffectFoldoutItem effect = (EffectFoldoutItem)item;
						GameObject effectClone = (GameObject)Object.Instantiate(effect.effectObject);
						WYD.Skill.Effect element = ScriptableObject.CreateInstance<WYD.Skill.Effect>();//new WYD.Skill.Effect();
						element.animationName = animationName;
						element.character = characterClone;
						element.frameIndex = effect.frameIndex;

						WYD.Skill.Skill skill = characterClone.AddComponent<WYD.Skill.Skill>();
						//element.parentScript = skill;
						element.AddAnimationEvent();
						skill.AddSkillElement(element);

						//skill.animationName = animationName;
						//skill.character = characterClone;
						//skill.frameIndex = effect.frameIndex;
						//skill.AddAnimationEvent();
						characterClone.animation.Play (m_animationName);
						//characterClone.animation.wrapMode = WrapMode.Loop;
					}
				}
			}
		}

		void OnSaveSkill()
		{
			/*
			SkinnedMeshRenderer[] components = ((GameObject)m_characterObj).GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer smr in components)
			{
				foreach (Transform t in smr.bones)
					Debug.Log(t.name);
			}*/

			CreateCharacter();
//			m_characterObj = AssetDatabase.LoadAssetAtPath("Assets/Character/JinF/MM.FBX", typeof(GameObject));
//			string str = AssetDatabase.GetAssetPath(m_characterObj);
//			Debug.Log (str);
		}
	}

}// end namespace WYD
