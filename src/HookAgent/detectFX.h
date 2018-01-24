#pragma once

#include <stdio.h>
#include <windows.h>
#include <tchar.h>
#include <strsafe.h>


// Function prototypes
bool CheckNetfxBuildNumber(const TCHAR*, const TCHAR*, const int, const int, const int, const int);
int GetNetfx10SPLevel();
int GetNetfxSPLevel(const TCHAR*, const TCHAR*);
bool IsCurrentOSTabletMedCenter();
bool IsNetfx10Installed();
bool IsNetfx11Installed();
bool IsNetfx20Installed();
bool IsNetfx30Installed();
bool IsNetfx35Installed();
bool IsNetfx40ClientInstalled();
bool IsNetfx40FullInstalled();
bool IsNetfx45Installed();
bool IsNetfx451Installed();
bool IsNetfx452Installed();
bool IsNetfx46Installed();
bool IsNetfx461Installed();
bool IsNetfx462Installed();
bool RegistryGetValue(HKEY, const TCHAR*, const TCHAR*, DWORD, LPBYTE, DWORD);