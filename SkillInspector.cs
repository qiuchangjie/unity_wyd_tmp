using UnityEngine;
using UnityEditor;
using System.Collections;

namespace WYD
{
	namespace Skill	
	{
		[CustomEditor(typeof(Skill), true)]
		[ExecuteInEditMode]
		public class SkillInspector : Editor
		{
			Object m_obj;
			public override void OnInspectorGUI()
			{
				DrawDefaultInspector();
				EditorGUILayout.BeginHorizontal();
				m_obj = EditorGUILayout.ObjectField("角色", m_obj, typeof(Object), true);
				EditorGUILayout.EndHorizontal();
				GUILayout.Button("test Button");
			}
		}

	}
}
