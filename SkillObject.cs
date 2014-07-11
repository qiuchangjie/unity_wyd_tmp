using UnityEngine;
using UnityEditor;
using System.Collections;
using WYD.Skill;

public class SkillObject : MonoBehaviour 
{
	[MenuItem("GameObject/Create Other/Skill Object")]
	//[MenuItem("Skill/Create Skill Object")]
	static void DoCreateSkillObject ()
	{
		GameObject go = new GameObject ("SkillObject");
		go.transform.localScale = Vector3.one;
		go.transform.rotation = Quaternion.identity;
		go.AddComponent<Sound>();
		go.AddComponent<Effect>();
		go.AddComponent<Damage>();
		Selection.activeGameObject = go;
	}
}
