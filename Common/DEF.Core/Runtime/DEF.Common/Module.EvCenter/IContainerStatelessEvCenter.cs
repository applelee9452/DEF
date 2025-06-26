#if DEF_CLIENT
using UnityEngine;
#else
using Orleans;
#endif
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProtoBuf;

namespace DEF.EvCenter
{
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public class CrashReportInfo
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string PlayerGuid { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string Message { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string LogType { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string StackTrace { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string Platform { get; set; }

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string DeviceModel { get; set; }

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public string OS { get; set; }

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public string OperatingSystemFamily { get; set; }

        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public string DeviceName { get; set; }

        [ProtoMember(10)]
#if !DEF_CLIENT
        [Id(9)]
#endif
        public string DeviceType { get; set; }

        [ProtoMember(11)]
#if !DEF_CLIENT
        [Id(10)]
#endif
        public string GraphicsDeviceName { get; set; }

        [ProtoMember(12)]
#if !DEF_CLIENT
        [Id(11)]
#endif
        public string GraphicsDeviceType { get; set; }

        [ProtoMember(13)]
#if !DEF_CLIENT
        [Id(12)]
#endif
        public string GraphicsDeviceVersion { get; set; }

        [ProtoMember(14)]
#if !DEF_CLIENT
        [Id(13)]
#endif
        public int GraphicsMemorySize { get; set; }

        [ProtoMember(15)]
#if !DEF_CLIENT
        [Id(14)]
#endif
        public string ProcessorType { get; set; }

        [ProtoMember(16)]
#if !DEF_CLIENT
        [Id(15)]
#endif
        public int SystemMemorySize { get; set; }

        [ProtoMember(17)]
#if !DEF_CLIENT
        [Id(16)]
#endif
        public int ProcessorCount { get; set; }

        [ProtoMember(18)]
#if !DEF_CLIENT
        [Id(17)]
#endif
        public int ProcessorFrequency { get; set; }

        public CrashReportInfo()
        {
        }

#if DEF_CLIENT
        public CrashReportInfo(string message, string stack_trace, LogType type)
        {
            InitializeDatas(message, stack_trace, type);
        }

        public CrashReportInfo(string message, string stack_trace, LogType type, string playerId)
        {
            InitializeDatas(message, stack_trace, type, playerId);
        }

        void InitializeDatas(string message, string stack_trace, LogType type)
        {
            this.Message = message;
            this.StackTrace = stack_trace;
            this.LogType = type.ToString();
            this.PlayerGuid = null;

            InitializePlatform();
            InitializeDeviceModel();
            InitializeOS();
            InitializeOperatingSystemFamily();
            InitializeDeviceName();
            InitializeDeviceType();
            InitializeGraphicsDeviceName();
            InitializeGraphicsDeviceType();
            InitializeGraphicsDeviceVersion();
            InitializeGraphicsMemorySize();
            InitializeProcessorType();
            InitializeSystemMemorySize();
            InitializeProcessorCount();
            InitializeProcessorFrequency();
        }

        void InitializeDatas(string message, string stack_trace, LogType type, string player_guid)
        {
            this.Message = message;
            this.StackTrace = stack_trace;
            this.LogType = type.ToString();
            this.PlayerGuid = player_guid;

            InitializePlatform();
            InitializeDeviceModel();
            InitializeOS();
            InitializeOperatingSystemFamily();
            InitializeDeviceName();
            InitializeDeviceType();
            InitializeGraphicsDeviceName();
            InitializeGraphicsDeviceType();
            InitializeGraphicsDeviceVersion();
            InitializeGraphicsMemorySize();
            InitializeProcessorType();
            InitializeSystemMemorySize();
            InitializeProcessorCount();
            InitializeProcessorFrequency();
        }

        void InitializePlatform()
        {
            try
            {
                this.Platform = Application.platform.ToString();
            }
            catch
            {
                this.Platform = null;
            }
        }

        void InitializeDeviceModel()
        {
            try
            {
                this.DeviceModel = SystemInfo.deviceModel;
            }
            catch
            {
                this.DeviceModel = string.Empty;
            }
        }

        void InitializeOS()
        {
            try
            {
                this.OS = SystemInfo.operatingSystem;
            }
            catch
            {
                this.OS = string.Empty;
            }
        }

        void InitializeOperatingSystemFamily()
        {
            try
            {
                this.OperatingSystemFamily = SystemInfo.operatingSystemFamily.ToString();
            }
            catch
            {
                this.OperatingSystemFamily = null;
            }
        }

        void InitializeDeviceName()
        {
            try
            {
                this.DeviceName = SystemInfo.deviceName;
            }
            catch
            {
                this.DeviceName = string.Empty;
            }
        }

        void InitializeDeviceType()
        {
            try
            {
                this.DeviceType = SystemInfo.deviceType.ToString();
            }
            catch
            {
                this.DeviceType = null;
            }
        }

        void InitializeGraphicsDeviceName()
        {
            try
            {
                this.GraphicsDeviceName = SystemInfo.graphicsDeviceName;
            }
            catch
            {
                this.GraphicsDeviceName = string.Empty;
            }
        }

        void InitializeGraphicsDeviceType()
        {
            try
            {
                this.GraphicsDeviceType = SystemInfo.graphicsDeviceType.ToString();
            }
            catch
            {
                this.GraphicsDeviceType = null;
            }
        }

        void InitializeGraphicsDeviceVersion()
        {
            try
            {
                this.GraphicsDeviceVersion = SystemInfo.graphicsDeviceVersion;
            }
            catch
            {
                this.GraphicsDeviceVersion = string.Empty;
            }
        }

        void InitializeGraphicsMemorySize()
        {
            try
            {
                this.GraphicsMemorySize = SystemInfo.graphicsMemorySize;
            }
            catch
            {
                this.GraphicsMemorySize = -1;
            }
        }

        void InitializeProcessorType()
        {
            try
            {
                this.ProcessorType = SystemInfo.processorType;
            }
            catch
            {
                this.ProcessorType = string.Empty;
            }
        }

        void InitializeSystemMemorySize()
        {
            try
            {
                this.SystemMemorySize = SystemInfo.systemMemorySize;
            }
            catch
            {
                this.SystemMemorySize = -1;
            }
        }

        void InitializeProcessorCount()
        {
            try
            {
                this.ProcessorCount = SystemInfo.processorCount;
            }
            catch
            {
                this.ProcessorCount = -1;
            }
        }

        void InitializeProcessorFrequency()
        {
            try
            {
                this.ProcessorFrequency = SystemInfo.processorFrequency;
            }
            catch
            {
                this.ProcessorFrequency = -1;
            }
        }
#endif
    }

    [ContainerRpc("DEF.EvCenter", "EvCenter", ContainerStateType.Stateless)]
    public interface IContainerStatelessEvCenter : IContainerRpc
    {
        Task ClientCrashReport(CrashReportInfo info, string client_ip);
    }
}