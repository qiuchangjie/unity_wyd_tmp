using UnityEngine;
using System.Collections;

//动作事件管理 
//说明： 事件回调函数参数只能传一个参数进去,分别为int,string ,float, Unity3d.Object
//       此脚本可以绑在任意GameObject上，但注册的回调函数所在的脚本必须要绑在有Animator组件的GameObject上,否则无法执行  
public class AnimationEventMgr : MonoBehaviour 
{
    //回调函数参数类型 
    public enum Param
    {
        NONE,       //无类型 
        
        INT,        //int类型

        FLOAT,      //float类型 
        
        STRING,     //string类型 

        OBJECT,     //Unity3d.Object类型
    }

    //事件信息 
    [System.Serializable]
    public class EventInfo
    {
        //信息对应的动作剪辑名称 
        public string m_ClipName;

        //回调函数名 
        public string m_FunctionName;

        //参数类型枚举
        public Param m_Param;

        //int参数
        public int m_InParam;

        //float参数 
        public float m_FloatParam;

        //string参数  
        public string m_StringParam;

        //Object类型 
        public Object m_Object;

        //触发时间 
        public float m_fTime;
    }

    //动作剪辑集合 
    public AnimationClip[] Clips;

    public EventInfo[] EventInfomation;

    void Awake()
    { 
        for (int i = 0; i < EventInfomation.Length; ++i)
        { 
            //创建事件信息 
            AnimationEvent temp_event = CreateAniamtionEventInfo(EventInfomation[i]);
           
            if (null == temp_event)
            {
                Debug.LogError("temp_event is Null");

                return;
            }

            //获取动作剪辑 
            AnimationClip clip = GetAnimationClip(EventInfomation[i].m_ClipName);

            if (null == clip)
            {
                Debug.LogError("Clip is Null");

                return;
            }

            //向动作剪辑里增加事件 
            clip.AddEvent(temp_event);

            temp_event = null;

            clip = null; 
        } 
    }

    void Start () 
    {
    
    } 
     
    void Update () 
    {
    
    } 

    //获取动作剪辑 
    public AnimationClip GetAnimationClip(string _clipname)
    {
        if (Clips != null)
        {
            if (Clips.Length > 0)
            {
                for(int i=0; i<Clips.Length; ++i)
                {
                    if(Clips[i].name == _clipname)
                    {
                        return Clips[i];
                    }
                }
            }
        } 
       
        return null;
    }

    //创建动作事件信息 
    public AnimationEvent CreateAniamtionEventInfo(EventInfo _info)
    { 
        if (null == _info)
        {
            Debug.LogError("info is Null");

            return null;
        }

        //创建动作事件信息  
        AnimationEvent temp_evet = new AnimationEvent();

        //设置回调函数名  
        temp_evet.functionName = _info.m_FunctionName; 
        
        //设置参数  
        switch (_info.m_Param)
        {
            case Param.INT:
                temp_evet.intParameter = _info.m_InParam;
                break;
            case Param.FLOAT:
                temp_evet.floatParameter = _info.m_FloatParam;
                break;
            case Param.STRING:
                temp_evet.stringParameter = _info.m_StringParam;
                break;
            case Param.OBJECT:
                temp_evet.objectReferenceParameter = _info.m_Object;
                break;
        }

        //设置触发时间 
        temp_evet.time = _info.m_fTime;

        return temp_evet;
    } 
}
