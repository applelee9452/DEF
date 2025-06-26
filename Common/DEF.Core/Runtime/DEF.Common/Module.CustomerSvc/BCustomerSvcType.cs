using System.Collections.Generic;

namespace DEF.CustomerSvc
{
    public enum MsgType
    {
        Text = 0,// 文本
        Image,// 图片
        CmdRateRequest,// 评价请求
        CmdRateResponse,// 评价回复
        CmdGetDeviceInfoRequest,// 获取设备信息请求
        CmdGetDeviceInfoResponse,// 获取设备信息回复
    }
}