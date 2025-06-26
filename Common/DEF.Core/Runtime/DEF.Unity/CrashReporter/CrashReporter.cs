#if DEF_CLIENT

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using DEF.EvCenter;
using System.IO;

namespace DEF
{
    public class CrashReporter : MonoBehaviour
    {
        public string PlayerGuid;
        public string UrlPrefix = "http://127.0.0.1:5000";
        float Tm = 0;

        void Awake()
        {
            Application.logMessageReceived += HandleLog;
            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.Full);
            Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.Full);

            //Debug.LogError("aaaaaaa");
        }

        void OnDestory()
        {
            Application.logMessageReceived -= HandleLog;
        }

        void Update()
        {
            if (Tm < 0) return;
            Tm -= Time.deltaTime;
        }

        void HandleLog(string log_string, string stack_trace, LogType type)
        {
            // todo，判定是否重复，重复日志有上报间隔

            if (Tm > 0) return;
            Tm = 60f;

            try
            {
                if (type == LogType.Error || type == LogType.Exception)
                {
                    GenerateAndSendCrashReport(log_string, stack_trace, type);
                }
            }
            catch (Exception)
            {
            }
        }

        void GenerateAndSendCrashReport(string message, string stack_trace, LogType type)
        {
            CrashReportInfo crashreport_info;
            if (!string.IsNullOrEmpty(PlayerGuid))
            {
                crashreport_info = new(message, stack_trace, type, PlayerGuid);
            }
            else
            {
                crashreport_info = new(message, stack_trace, type);
            }

            StartCoroutine(SendCrashReportToServer(crashreport_info));
        }

        IEnumerator SendCrashReportToServer(CrashReportInfo crashreport_info)
        {
            using var ms = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, crashreport_info);
            byte[] data = ms.ToArray();

            using UnityWebRequest req = UnityWebRequest.PostWwwForm(UrlPrefix + $"/api/evcenter/clientexception", string.Empty);
            req.uploadHandler = new UploadHandlerRaw(data);

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("ReportSetRequest => " + req.error);
            }
        }
    }
}

#endif