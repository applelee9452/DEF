#if DEF_CLIENT

using UnityEngine;
#if WEIXINMINIGAME
using WeChatWASM;
#elif DOUYINMINIGAME
#else
#endif

public class UpdateState1Idle : SimpleState2
{
    Updater Updater { get; set; }
    bool IsDone { get; set; }

    public UpdateState1Idle(Updater updater)
    {
        Updater = updater;
    }

    public override string GetName()
    {
        return "Idle";
    }

    public override void Enter()
    {
        Debug.Log("UpdateState1Idle.Enter()");

        IsDone = false;
    }

    public override void Exit()
    {
        Debug.Log("UpdateState1Idle.Exit()");
    }

    public override string Update(float tm)
    {
        if (IsDone)
        {
            return "RouteGatewayFromOss";
        }

        return string.Empty;
    }

    public override string OnEvent(string ev_name, string ev_param)
    {
        if (ev_name == "EvFire")
        {
#if WEIXINMINIGAME
            if (Application.platform == RuntimePlatform.MiniGamePlayer)
            {
                WX.InitSDK((a) =>
                {
                    Debug.Log("----------------INIT WX SDK----------------");
                    //WX.SetEnableDebug(new SetEnableDebugOption() {enableDebug = true });
                    //Debug.Log("----------------ENABLED DEBUG DONE----------------");
                    //WX.GetLogManager(new GetLogManagerOption() { level = 1});
                    //Debug.Log("----------------SET LOG LEVEL DONE---------------");
                    WX.ReportGameStart();// 启动分析报告
                    Debug.Log("----------------GAME REPORT DONE---------------");
                    IsDone = true;
                });
            }
            else
            {
                IsDone = true;
            }
#elif DOUYINMINIGAME
#else
            IsDone = true;
#endif
        }

        return string.Empty;
    }

    public override string OnEvent(string ev_name, object ev_param)
    {
        return string.Empty;
    }
}

#endif