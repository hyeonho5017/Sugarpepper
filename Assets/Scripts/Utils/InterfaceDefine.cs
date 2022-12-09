using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sugarpepper
{
    public interface IManagerBase
    {
        void Init();
        void Update(float dt);
    }
    public interface ITimeObject
    {
        void Init(); //해당 오브젝트를 TimeManager에 등록하는 과정.
        void Clear(); //해당 오브젝트를 TimeManager에서 제거하는 과정.
    }
    public interface IWaveBase
    {
        void Start();
        void End();
    }
    public interface ICharacter
    {
        void Update(float dt);
    }
    public interface IPopup
    {
        int GetOrder();
        void Init(object data);
        void ForceUpdate(object data);
        void Destroy();
    }
    public interface IPopupExtension
    {
        void Init();
        void ForceUpdate();
    }
    //public interface IUIBase
    //{
    //    void InitUI();//초기화 용도
    //    void InitUI(eUIType targetType);//현재 씬의 타입에 따라 UI변동.
    //    void RefreshUI();//정보 변경에 따른 갱신 
    //    void ShowEvent();//Visible 관련
    //    void HideEvent();//Visible 관련
    //    void ReuseAnim();//사용 연출
    //    void UnuseAnim();//미사용 연출
    //}
}
