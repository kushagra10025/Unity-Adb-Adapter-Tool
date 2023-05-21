using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NP.AdapterCore;
using System.Net;
using System;

public class AdbAdapterTest : MonoBehaviour
{
    [Header("ADB Config")]
    [SerializeField] private string adbCustomPath = "";
    [SerializeField] private bool adbForceStartNew = true;
    
    [Space(5)]

    [Header("Pair Device")]
    [SerializeField] private string pairIPAddress;
    [SerializeField] private int pairIPPort;
    [SerializeField] private string pairCode;


    private AdbAdapter adbAdapter;

    private void Start()
    {
        adbAdapter = new AdbAdapter(adbCustomPath, adbForceStartNew);

        if(adbAdapter.Initialize() != AdbAdapter_InitStatus.Successful)
        {
            Debug.Log("ADB Adapter Init Failed");
        }
    }

    private void Update()
    {
        if (adbAdapter == null)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(adbAdapter.PairDevice(new IPEndPoint(IPAddress.Parse(pairIPAddress), pairIPPort), pairCode));
        }
    }
}
