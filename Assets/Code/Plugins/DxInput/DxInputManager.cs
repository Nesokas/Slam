using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DxInputManager : MonoBehaviour {

    private static DxInputManager instance = null;
    public static DxInputManager Instance
    {
        get
        {
            if (instance == null) {
                instance = MainComponentManager.AddMainComponent<DxInputManager>();
            }
            return instance;
        }
    }

    private Dictionary<string, int> m_DeviceTable;
    private Dictionary<string, int>[] m_ControlTable;
	
	// todo: add input bindings here, and methods to save and restore them to PlayerPrefs
	// actually it would be nice to abstract a set of inputs for each player or npc
	// so control scripts can operate using an abstract input set without caring if they 
	// correspond to real inputs, or a virtual "AI controller".

	// Use this for initialization
	public void Awake() {
        DxInput.Init();

        Log();

		// make some tables for name lookups
		
		print("DxInputManager: Creating name lookup tables");
        m_DeviceTable = new Dictionary<string, int>();
        int nDevices = DxInput.NumDevices();
        m_ControlTable = new Dictionary<string, int>[nDevices];
		
		for (int i=0; i<nDevices; i++) {
            m_DeviceTable[DxInput.DeviceName(i)] = i;

            int nControls = DxInput.NumDeviceControls(i);
            m_ControlTable[i] = new Dictionary<string, int>();
			for (int j = 0; j < nControls; j++) {
                m_ControlTable[i][DxInput.DeviceControlName(i, j)] = j;
            }
        }
		print("DxInputManager: Creating name lookup tables complete");
        
	}

    // Find the first occurrence of controlName on a particular device
    public bool Find(string controlName, int nDevice, out int nControl) {
        return m_ControlTable[nDevice].TryGetValue(controlName, out nControl);
    }

	// Find the first occurrence of controlName on any device
    public bool Find(string controlName, out int nDevice, out int nControl) {
        int nDevices = DxInput.NumDevices();
        nControl = 0;
        for (nDevice=0; nDevice<nDevices; nDevice++) {
            if (Find(controlName, nDevice, out nControl)) {
				return true;
            }
        }
        return false;
    }

    public float GetValue(int nDevice, string controlName) {
        int nControl;
        if (Find(controlName, nDevice, out nControl)) {
            return DxInput.DeviceControlValue(nDevice, nControl);
        }
        return 0.0f;
    }

	public float GetValue(string controlName) {
        int nDevice, nControl;
        if (Find(controlName, out nDevice, out nControl)) {
            return DxInput.DeviceControlValue(nDevice, nControl);
        }
        return 0.0f;
    }

    public void Log() {
        int nDevices = DxInput.NumDevices();
        for (int i = 0; i<nDevices; i++) {
            int nControls = DxInput.NumDeviceControls(i);
            Debug.Log(string.Format("DxInputManager: Device {0} found named {1} with {2} controls", i, DxInput.DeviceName(i), nControls));            
            for (int j=0; j<nControls; j++) {
                print(string.Format("DxInputManager: Control {0} type {1} named {2} has value {3}", j, (int)DxInput.DeviceControlType(i, j), DxInput.DeviceControlName(i, j), DxInput.DeviceControlValue(i, j)));
            }
        }
    }

    public void OnApplicationQuit() {
        DxInput.Close();
    }

    // Update is called once per frame
	public void Update () {
	    DxInput.Update();
	}
}
