using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class AdbAdapterMainEditor : EditorWindow
{

    private Label AuthorVersionLbl;

    private Foldout AdbConfigFoldout;
    private TextField AdbCustomPathTxt;
    private TextField AdbServerIpTxt;
    private TextField AdbServerPortTxt;
    private Toggle AdbServerForceTgl;

    private Button AdbServerInitBtn;

    private Foldout DeviceConnectionFoldout;
    private Foldout DevicePairingFoldout;
    private TextField PairingIpTxt;
    private TextField PairingPortTxt;
    private TextField PairingCodeTxt;
    private Button PairBtn;
    private TextField DeviceConnectionIpTxt;
    private TextField DeviceConnectionPortTxt;
    private Button DeviceConnectBtn;
    private Button DeviceDisconnectBtn;

    private TextField PkgIdentifierTxt;
    private TextField ApkBuildPathTxt;
    private Button ApkBuildPathBrowseBtn;
    private DropdownField ConnectedDevicesDropDown;

    private Button CoreBtnInstall;
    private Button CoreBtnUninstall;
    private Button CoreBtnLaunch;
    private Button CoreBtnStop;
    private Button CoreBtnClearData;
    private Button CoreBtnWakeup;

    [Shortcut("Tools/ADB Adapter/Open ADB Adapter Window", KeyCode.I, ShortcutModifiers.Control | ShortcutModifiers.Shift)]
    [MenuItem("Tools/ADB Adapter/Open ADB Adapter Window")]
    public static void OpenAdbAdapterMainWindow()
    {
        AdbAdapterMainEditor window = GetWindow<AdbAdapterMainEditor>();
        window.titleContent = new GUIContent("ADB Adapter");
        window.maxSize = new Vector2(300, 400);
        window.minSize = window.maxSize;
        window.ShowModalUtility();
    }

    private void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        VisualTreeAsset mainEditorUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/UnityAdbAdapter/UI/AdbAdapterMain_UXML.uxml");

        if (mainEditorUXML == null)
        {
            Debug.Log("Cannot Load AdbAdapterMain_UXML.uxml. Please re-import or restore original Path!");
            return;
        }

        root.Add(mainEditorUXML.Instantiate());

        // Assign Elements
        AuthorVersionLbl = root.Q<Label>("tool-author-version-lbl");

        AdbConfigFoldout = root.Q<Foldout>("adb-config-foldout");
        AdbCustomPathTxt = root.Q<TextField>("adb-path-txt");
        AdbServerIpTxt = root.Q<TextField>("adb-server-ip-txt");
        AdbServerPortTxt = root.Q<TextField>("adb-server-port-txt");
        AdbServerForceTgl = root.Q<Toggle>("adb-server-force-toggle");

        AdbServerInitBtn = root.Q<Button>("btn-adb-server-init");

        DeviceConnectionFoldout = root.Q<Foldout>("device-connection-foldout");
        DevicePairingFoldout = root.Q<Foldout>("device-pairing-foldout");
        PairingIpTxt = root.Q<TextField>("pair-ip-txt");
        PairingPortTxt = root.Q<TextField>("pair-port-txt");
        PairingCodeTxt = root.Q<TextField>("pair-code-txt");
        PairBtn = root.Q<Button>("btn-pair");
        DeviceConnectionIpTxt = root.Q<TextField>("device-ip-txt");
        DeviceConnectionPortTxt = root.Q<TextField>("device-port-txt");
        DeviceConnectBtn = root.Q<Button>("btn-device-connect");
        DeviceDisconnectBtn = root.Q<Button>("btn-device-disconnect");

        PkgIdentifierTxt = root.Q<TextField>("pkg-identifier-txt");
        ApkBuildPathTxt = root.Q<TextField>("apk-build-path-txt");
        ApkBuildPathBrowseBtn = root.Q<Button>("btn-browse-apk-path");
        ConnectedDevicesDropDown = root.Q<DropdownField>("connected-devices-dropdown");

        CoreBtnInstall = root.Q<Button>("btn-install");
        CoreBtnUninstall = root.Q<Button>("btn-uninstall");
        CoreBtnLaunch = root.Q<Button>("btn-launch");
        CoreBtnStop = root.Q<Button>("btn-force-stop");
        CoreBtnClearData = root.Q<Button>("btn-clear");
        CoreBtnWakeup = root.Q<Button>("btn-wake-up");

        #region Buttons Clicked
        AdbServerInitBtn.clicked += AdbServerInitBtn_clicked;
        PairBtn.clicked += PairBtn_clicked;
        DeviceConnectBtn.clicked += DeviceConnectBtn_clicked;
        DeviceDisconnectBtn.clicked += DeviceDisconnectBtn_clicked;
        ApkBuildPathBrowseBtn.clicked += ApkBuildPathBrowseBtn_clicked;
        
        // Core Function Buttons
        CoreBtnInstall.clicked += CoreBtnInstall_clicked;
        CoreBtnUninstall.clicked += CoreBtnUninstall_clicked;
        CoreBtnLaunch.clicked += CoreBtnLaunch_clicked;
        CoreBtnStop.clicked += CoreBtnStop_clicked;
        CoreBtnClearData.clicked += CoreBtnClearData_clicked;
        CoreBtnWakeup.clicked += CoreBtnWakeup_clicked;
        #endregion
    }

    private void CoreBtnWakeup_clicked()
    {
    }

    private void CoreBtnClearData_clicked()
    {
    }

    private void CoreBtnStop_clicked()
    {
    }

    private void CoreBtnLaunch_clicked()
    {
    }

    private void CoreBtnUninstall_clicked()
    {
    }

    private void CoreBtnInstall_clicked()
    {
    }

    private void ApkBuildPathBrowseBtn_clicked()
    {
    }

    private void DeviceDisconnectBtn_clicked()
    {
    }

    private void DeviceConnectBtn_clicked()
    {
    }

    private void PairBtn_clicked()
    {
    }

    private void AdbServerInitBtn_clicked()
    {
    }
}
