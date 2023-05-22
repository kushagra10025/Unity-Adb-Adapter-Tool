using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class AdbAdapterEditorSaveData
{
    public string AdbCustomPath;
    public string AdbServerIP;
    public string AdbServerPort;
    public bool AdbForceStartNew;

    public string PairingIP;
    public string PairingPort;
    public string PairingCode;

    public string DeviceConnectionIP;
    public string DeviceConnectionPort;

    public string AppPackageIdentifier;
    public string AppApkBuildPath;
}

public class AdbAdapterMainEditor : EditorWindow
{
    public const string ADB_ADAPTER_MAIN_EDITOR_PREFS_KEY = "ADBAdapter_MainEditor_Prefs";

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

    [HideInInspector][SerializeField] private AdbAdapterEditorSaveData adbAdapterEditorSaveData;

    [MenuItem("Tools/ADB Adapter/Open ADB Adapter Window %#i")]
    public static void OpenAdbAdapterMainWindow()
    {
        AdbAdapterMainEditor window = GetWindow<AdbAdapterMainEditor>(true, "ADB Adapter");
        window.maxSize = new Vector2(300, 400);
        window.minSize = window.maxSize;
        window.ShowUtility();
    }

    private void OnEnable()
    {
        // TODO Improve Save Conditions -> Currently Save/Load when Window Enabled and Disabled
        string data = EditorPrefs.GetString(ADB_ADAPTER_MAIN_EDITOR_PREFS_KEY, EditorJsonUtility.ToJson(this, false));
        JsonUtility.FromJsonOverwrite(data, this);
    }

    private void OnDisable()
    {
        string data = JsonUtility.ToJson(this, false);
        EditorPrefs.SetString(ADB_ADAPTER_MAIN_EDITOR_PREFS_KEY, data);
    }

    private void OnGUI()
    {
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

        #region Assigned Elements from UXML
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
        #endregion

        #region RegisterCallbacks
        AdbCustomPathTxt.RegisterValueChangedCallback<string>(AdbCustomPath_valueChanged);
        AdbServerIpTxt.RegisterValueChangedCallback<string>(AdbServerIp_valueChanged);
        AdbServerPortTxt.RegisterValueChangedCallback<string>(AdbServerPort_valueChanged);
        AdbServerForceTgl.RegisterValueChangedCallback<bool>(AdbServerForce_valueChanged);

        PairingIpTxt.RegisterValueChangedCallback<string>(PairingIP_valueChanged);
        PairingPortTxt.RegisterValueChangedCallback<string>(PairingPort_valueChanged);
        PairingCodeTxt.RegisterValueChangedCallback<string>(PairingCode_valueChanged);

        DeviceConnectionIpTxt.RegisterValueChangedCallback<string>(DeviceConnectionIP_valueChanged);
        DeviceConnectionPortTxt.RegisterValueChangedCallback<string>(DeviceConnectionPort_valueChanged);

        PkgIdentifierTxt.RegisterValueChangedCallback<string>(PkgIdentifier_valueChanged);
        ApkBuildPathTxt.RegisterValueChangedCallback<string>(ApkBuildPath_valueChanged);
        #endregion

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

        LoadSavedValues();
    }

    #region Value Changed Callbacks
    private void ApkBuildPath_valueChanged(ChangeEvent<string> evt)
    {
        if (!string.IsNullOrEmpty(evt.newValue))
        {
            adbAdapterEditorSaveData.AppApkBuildPath = evt.newValue;
            ApkBuildPathTxt.value = evt.newValue;
        }
    }

    private void PkgIdentifier_valueChanged(ChangeEvent<string> evt)
    {
        if (!string.IsNullOrEmpty(evt.newValue))
        {
            adbAdapterEditorSaveData.AppPackageIdentifier = evt.newValue;
            PkgIdentifierTxt.value = evt.newValue;
        }
    }

    private void DeviceConnectionPort_valueChanged(ChangeEvent<string> evt)
    {
        if (!string.IsNullOrEmpty(evt.newValue))
        {
            adbAdapterEditorSaveData.DeviceConnectionPort = evt.newValue;
            DeviceConnectionPortTxt.value = evt.newValue;
        }
    }

    private void DeviceConnectionIP_valueChanged(ChangeEvent<string> evt)
    {
        if (!string.IsNullOrEmpty(evt.newValue))
        {
            adbAdapterEditorSaveData.DeviceConnectionIP = evt.newValue;
            DeviceConnectionIpTxt.value = evt.newValue;
        }
    }

    private void PairingCode_valueChanged(ChangeEvent<string> evt)
    {
        if (!string.IsNullOrEmpty(evt.newValue))
        {
            adbAdapterEditorSaveData.PairingCode = evt.newValue;
            PairingCodeTxt.value = evt.newValue;
        }
    }

    private void PairingPort_valueChanged(ChangeEvent<string> evt)
    {
        if (!string.IsNullOrEmpty(evt.newValue))
        {
            adbAdapterEditorSaveData.PairingPort = evt.newValue;
            PairingPortTxt.value = evt.newValue;
        }
    }

    private void PairingIP_valueChanged(ChangeEvent<string> evt)
    {
        if (!string.IsNullOrEmpty(evt.newValue))
        {
            adbAdapterEditorSaveData.PairingIP = evt.newValue;
            PairingIpTxt.value = evt.newValue;
        }
    }

    private void AdbServerForce_valueChanged(ChangeEvent<bool> evt)
    {
        if (evt != null)
        {
            adbAdapterEditorSaveData.AdbForceStartNew = evt.newValue;
            AdbServerForceTgl.value = evt.newValue;
        }
    }

    private void AdbServerPort_valueChanged(ChangeEvent<string> evt)
    {
        if (!string.IsNullOrEmpty(evt.newValue))
        {
            adbAdapterEditorSaveData.AdbServerPort = evt.newValue;
            AdbServerPortTxt.value = evt.newValue;
        }
    }

    private void AdbServerIp_valueChanged(ChangeEvent<string> evt)
    {
        if (!string.IsNullOrEmpty(evt.newValue))
        {
            adbAdapterEditorSaveData.AdbServerIP = evt.newValue;
            AdbServerIpTxt.value = evt.newValue;
        }
    }

    private void AdbCustomPath_valueChanged(ChangeEvent<string> evt)
    {
        if (!string.IsNullOrEmpty(evt.newValue))
        {
            adbAdapterEditorSaveData.AdbCustomPath = evt.newValue;
            AdbCustomPathTxt.value = evt.newValue;
        }
    }
    #endregion

    #region Core Function Buttons OnClicked
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
    #endregion

    #region Functionality Buttons OnClicked
    private void ApkBuildPathBrowseBtn_clicked()
    {
        string apkFilePath = EditorUtility.OpenFilePanel("Select APK File", "", "apk");
        //adbAdapterEditorSaveData.AppApkBuildPath = apkFilePath;
        ApkBuildPathTxt.value = apkFilePath;
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
    #endregion

    private void LoadSavedValues()
    {
        AdbCustomPathTxt.value = adbAdapterEditorSaveData.AdbCustomPath;
        AdbServerIpTxt.value = adbAdapterEditorSaveData.AdbServerIP;
        AdbServerPortTxt.value = adbAdapterEditorSaveData.AdbServerPort;
        AdbServerForceTgl.value = adbAdapterEditorSaveData.AdbForceStartNew;

        PairingIpTxt.value = adbAdapterEditorSaveData.PairingIP;
        PairingPortTxt.value = adbAdapterEditorSaveData.PairingPort;
        PairingCodeTxt.value = adbAdapterEditorSaveData.PairingCode;

        DeviceConnectionIpTxt.value = adbAdapterEditorSaveData.DeviceConnectionIP;
        DeviceConnectionPortTxt.value = adbAdapterEditorSaveData.DeviceConnectionPort;

        PkgIdentifierTxt.value = adbAdapterEditorSaveData.AppPackageIdentifier;
        ApkBuildPathTxt.value = adbAdapterEditorSaveData.AppApkBuildPath;
    }

}
