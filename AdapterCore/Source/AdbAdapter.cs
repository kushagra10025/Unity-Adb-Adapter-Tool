using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace NP.AdapterCore
{
    public enum AdbAdapter_InitStatus
    {
        Successful, Failed
    }

    public class AdapterCoreConstants
    {
        public const string TOOL_AUTHOR = "Kushagra Prakash";
        public const string TOOL_VERSION = "v1.0.0";

        public const string ADB = "adb";
        public const string ADB_CMD_ANDROID_VERSION = "getprop ro.build.version.release";
        public const string ADB_CMD_SDK_LEVEL = "getprop ro.build.version.sdk";
        public const string ADB_ERROR_CLIENT_NULL = "AdbClient is null! Retry!";
    }

    public class AdbAdapter
    {
        private IPAddress _adbServerIPAddress;
        private int _adbServerPort;
        private string _adbCustomPath;
        private bool _adbForceStartNew;

        private AdbServer? _adbServer;
        private AdbClient? _adbClient;

        public AdbAdapter(string adbCustomPath, bool adbForceStartNew) : this(adbCustomPath, IPAddress.Loopback, AdbClient.AdbServerPort, adbForceStartNew)
        {
        }

        public AdbAdapter(string adbCustomPath, IPAddress abdServerIPAddress, int adbServerPort, bool adbForceStartNew)
        {
            _adbCustomPath = adbCustomPath;
            _adbServerIPAddress = abdServerIPAddress;
            _adbServerPort = adbServerPort;
            _adbForceStartNew = adbForceStartNew;
        }

        public AdbAdapter_InitStatus Initialize()
        {
            _adbClient = new AdbClient(new IPEndPoint(_adbServerIPAddress, _adbServerPort), Factories.AdbSocketFactory);

            if (!AdbServer.Instance.GetStatus().IsRunning)
            {
                _adbServer = new AdbServer(_adbClient, Factories.AdbCommandLineClientFactory);
                string adbPath = GetExecutablePathFromEnvironmentVariable(AdapterCoreConstants.ADB) ?? string.Empty;
                StartServerResult result = _adbServer.StartServer(string.IsNullOrEmpty(adbPath) ? _adbCustomPath : adbPath, _adbForceStartNew);
                if (result != StartServerResult.Started)
                {
                    return AdbAdapter_InitStatus.Failed;
                }
                return AdbAdapter_InitStatus.Successful;
            }
            else
            {
                _adbServer = (AdbServer)AdbServer.Instance;
            }

            return AdbAdapter_InitStatus.Successful;
        }

        #region Core Helper Functions
        public void InstallPackageOnDevice(DeviceData device, string apkFilePath)
        {
            if (_adbClient == null)
                return;

            _adbClient.Install(device, File.OpenRead(apkFilePath));
        }

        public void UninstallPackageOnDevice(DeviceData device, string packageIdentifier)
        {
            if (_adbClient == null)
                return;

            _adbClient.UninstallPackage(device, packageIdentifier);
        }

        public void LaunchAppOnDevice(DeviceData device, string packageIdentifier)
        {
            if (_adbClient == null)
                return;

            _adbClient.StartApp(device, packageIdentifier);
        }

        public string LaunchAppOnDevice(DeviceData device, string packageIdentifier, string specificActivity)
        {
            if (_adbClient == null)
                return AdapterCoreConstants.ADB_ERROR_CLIENT_NULL;

            return ExecuteRemoteCommandOnDevice(device, $"am start -n ${packageIdentifier}/${specificActivity}");
        }

        public void StopAppOnDevice(DeviceData device, string packageIdentifier)
        {
            if (_adbClient == null)
                return;

            _adbClient.StopApp(device, packageIdentifier);
        }

        public string CleanAppDataOnDevice(DeviceData device, string packageIdentifier)
        {
            if (_adbClient == null)
                return AdapterCoreConstants.ADB_ERROR_CLIENT_NULL;

            return ExecuteRemoteCommandOnDevice(device, $"pm clear ${packageIdentifier}");
        }

        public void WakeUpDevice(DeviceData device)
        {
            if (_adbClient == null)
                return;

            SimulateDeviceCustomKeyEvent(device, "KEYCODE_WAKEUP");
        }
        #endregion

        #region KeyEvents Functions
        public void PressDeviceBackButton(DeviceData device)
        {
            if (_adbClient == null)
                return;

            _adbClient.BackBtn(device);
        }

        public void PressDeviceHomeButton(DeviceData device)
        {
            if (_adbClient == null)
                return;

            _adbClient.HomeBtn(device);
        }
        #endregion

        #region Specific Device Functions
        public string GetDeviceAndroidVersion(DeviceData device)
        {
            if (_adbClient == null)
                return AdapterCoreConstants.ADB_ERROR_CLIENT_NULL;

            return ExecuteRemoteCommandOnDevice(device, AdapterCoreConstants.ADB_CMD_ANDROID_VERSION);
        }

        public string GetDeviceAndroidSdkLevel(DeviceData device)
        {
            if (_adbClient == null)
                return AdapterCoreConstants.ADB_ERROR_CLIENT_NULL;

            return ExecuteRemoteCommandOnDevice(device, AdapterCoreConstants.ADB_CMD_SDK_LEVEL);
        }

        public string ExecuteRemoteCommandOnDevice(DeviceData device, string command)
        {
            if (_adbClient == null)
                return AdapterCoreConstants.ADB_ERROR_CLIENT_NULL;

            ConsoleOutputReceiver? outputReceiver = new ConsoleOutputReceiver();
            _adbClient.ExecuteRemoteCommand(command, device, outputReceiver);
            string result = outputReceiver.ToString();
            outputReceiver = null;
            return result;
        }

        public void SimulateDeviceCustomKeyEvent(DeviceData device, string keyCode)
        {
            if (_adbClient == null)
                return;

            _adbClient.SendKeyEvent(device, keyCode);
        }
        #endregion

        #region General Device Functions
        public string PairDevice(IPEndPoint endPoint, string pairCode)
        {
            if (_adbClient == null)
                return AdapterCoreConstants.ADB_ERROR_CLIENT_NULL;

            return _adbClient.Pair(endPoint, pairCode);
        }

        public string ConnectDevice(IPEndPoint endPoint)
        {
            if (_adbClient == null)
                return AdapterCoreConstants.ADB_ERROR_CLIENT_NULL;

            return _adbClient.Connect(endPoint);
        }

        public string DisconnectDevice(DnsEndPoint endPoint)
        {
            if (_adbClient == null)
                return AdapterCoreConstants.ADB_ERROR_CLIENT_NULL;

            return _adbClient.Disconnect(endPoint);
        }

        public DeviceData? GetDeviceAtIndex(int index)
        {
            if (_adbClient == null)
                return null;

            return GetAllConnectedDevices().ElementAt<DeviceData>(index);
        }

        public IEnumerable<DeviceData> GetAllConnectedDevices()
        {
            if (_adbClient == null)
                return Enumerable.Empty<DeviceData>();

            return _adbClient.GetDevices();
        }
        #endregion

        #region Utils
        internal static string? GetExecutablePathFromEnvironmentVariable(string exeName)
        {
            List<string> paths = GetEnvironmentVariablesInMachine();
            foreach (string path in paths)
            {
                string t_path = path;
                if (File.Exists(t_path = Path.Combine(path, exeName)))
                {
                    return t_path;
                }
            }
            return null;
        }

        internal static List<string> GetEnvironmentVariablesInMachine()
        {
            List<string> variablePaths = new List<string>();

            foreach (string variablePath in (Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine) ?? "").Split(';'))
            {
                string t_variablePath = variablePath.Trim();
                if (!string.IsNullOrEmpty(t_variablePath))
                {
                    variablePaths.Add(t_variablePath);
                }
            }

            return variablePaths;
        }
        #endregion
    }
}
