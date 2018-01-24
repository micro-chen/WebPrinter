#include "detectFX.h"


// In case the machine this is compiled on does not have the most recent platform SDK
// with these values defined, define them here
#ifndef SM_TABLETPC
#define SM_TABLETPC     86
#endif

#ifndef SM_MEDIACENTER
#define SM_MEDIACENTER  87
#endif

// Constants that represent registry key names and value names
// to use for detection
const TCHAR *g_szNetfx10RegKeyName = _T("Software\\Microsoft\\.NETFramework\\Policy\\v1.0");
const TCHAR *g_szNetfx10RegKeyValue = _T("3705");
const TCHAR *g_szNetfx10SPxMSIRegKeyName = _T("Software\\Microsoft\\Active Setup\\Installed Components\\{78705f0d-e8db-4b2d-8193-982bdda15ecd}");
const TCHAR *g_szNetfx10SPxOCMRegKeyName = _T("Software\\Microsoft\\Active Setup\\Installed Components\\{FDC11A6F-17D1-48f9-9EA3-9051954BAA24}");
const TCHAR *g_szNetfx11RegKeyName = _T("Software\\Microsoft\\NET Framework Setup\\NDP\\v1.1.4322");
const TCHAR *g_szNetfx20RegKeyName = _T("Software\\Microsoft\\NET Framework Setup\\NDP\\v2.0.50727");
const TCHAR *g_szNetfx30RegKeyName = _T("Software\\Microsoft\\NET Framework Setup\\NDP\\v3.0\\Setup");
const TCHAR *g_szNetfx30SpRegKeyName = _T("Software\\Microsoft\\NET Framework Setup\\NDP\\v3.0");
const TCHAR *g_szNetfx30RegValueName = _T("InstallSuccess");
const TCHAR *g_szNetfx35RegKeyName = _T("Software\\Microsoft\\NET Framework Setup\\NDP\\v3.5");
const TCHAR *g_szNetfx40ClientRegKeyName = _T("Software\\Microsoft\\NET Framework Setup\\NDP\\v4\\Client");
const TCHAR *g_szNetfx40FullRegKeyName = _T("Software\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full");
const TCHAR *g_szNetfx40SPxRegValueName = _T("Servicing");
const TCHAR *g_szNetfx45RegKeyName = _T("Software\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full");
const TCHAR *g_szNetfx45RegValueName = _T("Release");
const TCHAR *g_szNetfxStandardRegValueName = _T("Install");
const TCHAR *g_szNetfxStandardSPxRegValueName = _T("SP");
const TCHAR *g_szNetfxStandardVersionRegValueName = _T("Version");

// Version information for final release of .NET Framework 3.0
const int g_iNetfx30VersionMajor = 3;
const int g_iNetfx30VersionMinor = 0;
const int g_iNetfx30VersionBuild = 4506;
const int g_iNetfx30VersionRevision = 26;

// Version information for final release of .NET Framework 3.5
const int g_iNetfx35VersionMajor = 3;
const int g_iNetfx35VersionMinor = 5;
const int g_iNetfx35VersionBuild = 21022;
const int g_iNetfx35VersionRevision = 8;

// Version information for final release of .NET Framework 4
const int g_iNetfx40VersionMajor = 4;
const int g_iNetfx40VersionMinor = 0;
const int g_iNetfx40VersionBuild = 30319;
const int g_iNetfx40VersionRevision = 0;

// Version information for final release of .NET Framework 4.5
const int g_dwNetfx45ReleaseVersion = 378389;

// Version information for final release of .NET Framework 4.5.1
const int g_dwNetfx451ReleaseVersion = 378675;

// Version information for final release of .NET Framework 4.5.2
const int g_dwNetfx452ReleaseVersion = 379893;

// Version information for final release of .NET Framework 4.6
const int g_dwNetfx46ReleaseVersion = 393295;

// Version information for final release of .NET Framework 4.6.1
const int g_dwNetfx461ReleaseVersion = 394254;

// Version information for final release of .NET Framework 4.6.2
const int g_dwNetfx462ReleaseVersion = 394802;



/******************************************************************
Function Name:	IsNetfx10Installed
Description:	Uses the detection method recommended at
                http://msdn.microsoft.com/library/ms994349.aspx
                to determine whether the .NET Framework 1.0 is
                installed on the machine
Inputs:	        NONE
Results:        true if the .NET Framework 1.0 is installed
                false otherwise
******************************************************************/
bool IsNetfx10Installed()
{
	TCHAR szRegValue[MAX_PATH];
	return (RegistryGetValue(HKEY_LOCAL_MACHINE, g_szNetfx10RegKeyName, g_szNetfx10RegKeyValue, NULL, (LPBYTE)szRegValue, MAX_PATH));
}


/******************************************************************
Function Name:	IsNetfx11Installed
Description:	Uses the detection method recommended at
                http://msdn.microsoft.com/library/ms994339.aspx
                to determine whether the .NET Framework 1.1 is
                installed on the machine
Inputs:	        NONE
Results:        true if the .NET Framework 1.1 is installed
                false otherwise
******************************************************************/
bool IsNetfx11Installed()
{
	bool bRetValue = false;
	DWORD dwRegValue=0;

	if (RegistryGetValue(HKEY_LOCAL_MACHINE, g_szNetfx11RegKeyName, g_szNetfxStandardRegValueName, NULL, (LPBYTE)&dwRegValue, sizeof(DWORD)))
	{
		if (1 == dwRegValue)
			bRetValue = true;
	}

	return bRetValue;
}


/******************************************************************
Function Name:	IsNetfx20Installed
Description:	Uses the detection method recommended at
                http://msdn.microsoft.com/library/aa480243.aspx
                to determine whether the .NET Framework 2.0 is
                installed on the machine
Inputs:	        NONE
Results:        true if the .NET Framework 2.0 is installed
                false otherwise
******************************************************************/
bool IsNetfx20Installed()
{
	bool bRetValue = false;
	DWORD dwRegValue=0;

	if (RegistryGetValue(HKEY_LOCAL_MACHINE, g_szNetfx20RegKeyName, g_szNetfxStandardRegValueName, NULL, (LPBYTE)&dwRegValue, sizeof(DWORD)))
	{
		if (1 == dwRegValue)
			bRetValue = true;
	}

	return bRetValue;
}


/******************************************************************
Function Name:	IsNetfx30Installed
Description:	Uses the detection method recommended at
                http://msdn.microsoft.com/library/aa964979.aspx
                to determine whether the .NET Framework 3.0 is
                installed on the machine
Inputs:	        NONE
Results:        true if the .NET Framework 3.0 is installed
                false otherwise
******************************************************************/
bool IsNetfx30Installed()
{
	bool bRetValue = false;
	DWORD dwRegValue=0;

	// Check that the InstallSuccess registry value exists and equals 1
	if (RegistryGetValue(HKEY_LOCAL_MACHINE, g_szNetfx30RegKeyName, g_szNetfx30RegValueName, NULL, (LPBYTE)&dwRegValue, sizeof(DWORD)))
	{
		if (1 == dwRegValue)
			bRetValue = true;
	}

	// A system with a pre-release version of the .NET Framework 3.0 can
	// have the InstallSuccess value.  As an added verification, check the
	// version number listed in the registry
	return (bRetValue && CheckNetfxBuildNumber(g_szNetfx30RegKeyName, g_szNetfxStandardVersionRegValueName, g_iNetfx30VersionMajor, g_iNetfx30VersionMinor, g_iNetfx30VersionBuild, g_iNetfx30VersionRevision));
}


/******************************************************************
Function Name:	IsNetfx35Installed
Description:	Uses the detection method recommended at
                http://msdn.microsoft.com/library/cc160716.aspx
                to determine whether the .NET Framework 3.5 is
                installed on the machine
Inputs:	        NONE
Results:        true if the .NET Framework 3.5 is installed
                false otherwise
******************************************************************/
bool IsNetfx35Installed()
{
	bool bRetValue = false;
	DWORD dwRegValue=0;

	// Check that the Install registry value exists and equals 1
	if (RegistryGetValue(HKEY_LOCAL_MACHINE, g_szNetfx35RegKeyName, g_szNetfxStandardRegValueName, NULL, (LPBYTE)&dwRegValue, sizeof(DWORD)))
	{
		if (1 == dwRegValue)
			bRetValue = true;
	}

	// A system with a pre-release version of the .NET Framework 3.5 can
	// have the Install value.  As an added verification, check the
	// version number listed in the registry
	return (bRetValue && CheckNetfxBuildNumber(g_szNetfx35RegKeyName, g_szNetfxStandardVersionRegValueName, g_iNetfx35VersionMajor, g_iNetfx35VersionMinor, g_iNetfx35VersionBuild, g_iNetfx35VersionRevision));
}


/******************************************************************
Function Name:	IsNetfx40ClientInstalled
Description:	Uses the detection method recommended at
                http://msdn.microsoft.com/library/ee942965(v=VS.100).aspx
                to determine whether the .NET Framework 4 Client is
                installed on the machine
Inputs:         NONE
Results:        true if the .NET Framework 4 Client is installed
                false otherwise
******************************************************************/
bool IsNetfx40ClientInstalled()
{
	bool bRetValue = false;
	DWORD dwRegValue=0;

	if (RegistryGetValue(HKEY_LOCAL_MACHINE, g_szNetfx40ClientRegKeyName, g_szNetfxStandardRegValueName, NULL, (LPBYTE)&dwRegValue, sizeof(DWORD)))
	{
		if (1 == dwRegValue)
			bRetValue = true;
	}

	// A system with a pre-release version of the .NET Framework 4 can
	// have the Install value.  As an added verification, check the
	// version number listed in the registry
	return (bRetValue && CheckNetfxBuildNumber(g_szNetfx40ClientRegKeyName, g_szNetfxStandardVersionRegValueName, g_iNetfx40VersionMajor, g_iNetfx40VersionMinor, g_iNetfx40VersionBuild, g_iNetfx40VersionRevision));
}


/******************************************************************
Function Name:	IsNetfx40FullInstalled
Description:	Uses the detection method recommended at
                http://msdn.microsoft.com/library/ee942965(v=VS.100).aspx
                to determine whether the .NET Framework 4 Full is
                installed on the machine
Inputs:         NONE
Results:        true if the .NET Framework 4 Full is installed
                false otherwise
******************************************************************/
bool IsNetfx40FullInstalled()
{
	bool bRetValue = false;
	DWORD dwRegValue=0;

	if (RegistryGetValue(HKEY_LOCAL_MACHINE, g_szNetfx40FullRegKeyName, g_szNetfxStandardRegValueName, NULL, (LPBYTE)&dwRegValue, sizeof(DWORD)))
	{
		if (1 == dwRegValue)
			bRetValue = true;
	}

	// A system with a pre-release version of the .NET Framework 4 can
	// have the Install value.  As an added verification, check the
	// version number listed in the registry
	return (bRetValue && CheckNetfxBuildNumber(g_szNetfx40FullRegKeyName, g_szNetfxStandardVersionRegValueName, g_iNetfx40VersionMajor, g_iNetfx40VersionMinor, g_iNetfx40VersionBuild, g_iNetfx40VersionRevision));
}


/******************************************************************
Function Name:	IsNetfx45Installed
Description:	Uses the detection method recommended at
                http://msdn.microsoft.com/en-us/library/ee942965(v=vs.110).aspx
                to determine whether the .NET Framework 4.5 is
                installed on the machine
Inputs:         NONE
Results:        true if the .NET Framework 4.5 is installed
                false otherwise
******************************************************************/
bool IsNetfx45Installed()
{
	bool bRetValue = false;
	DWORD dwRegValue=0;

	if (RegistryGetValue(HKEY_LOCAL_MACHINE, g_szNetfx45RegKeyName, g_szNetfx45RegValueName, NULL, (LPBYTE)&dwRegValue, sizeof(DWORD)))
	{
		if (g_dwNetfx45ReleaseVersion <= dwRegValue)
			bRetValue = true;
	}

	return bRetValue;
}


/******************************************************************
Function Name:	IsNetfx451Installed
Description:	Uses the detection method recommended at
                http://msdn.microsoft.com/en-us/library/ee942965(v=vs.110).aspx
                to determine whether the .NET Framework 4.5.1 is
                installed on the machine
Inputs:         NONE
Results:        true if the .NET Framework 4.5.1 is installed
                false otherwise
******************************************************************/
bool IsNetfx451Installed()
{
	bool bRetValue = false;
	DWORD dwRegValue=0;

	if (RegistryGetValue(HKEY_LOCAL_MACHINE, g_szNetfx45RegKeyName, g_szNetfx45RegValueName, NULL, (LPBYTE)&dwRegValue, sizeof(DWORD)))
	{
		if (g_dwNetfx451ReleaseVersion <= dwRegValue)
			bRetValue = true;
	}

	return bRetValue;
}


/******************************************************************
Function Name:	IsNetfx452Installed
Description:	Uses the detection method recommended at
                http://msdn.microsoft.com/en-us/library/ee942965(v=vs.110).aspx
                to determine whether the .NET Framework 4.5.2 is
                installed on the machine
Inputs:         NONE
Results:        true if the .NET Framework 4.5.2 is installed
                false otherwise
******************************************************************/
bool IsNetfx452Installed()
{
	bool bRetValue = false;
	DWORD dwRegValue=0;

	if (RegistryGetValue(HKEY_LOCAL_MACHINE, g_szNetfx45RegKeyName, g_szNetfx45RegValueName, NULL, (LPBYTE)&dwRegValue, sizeof(DWORD)))
	{
		if (g_dwNetfx452ReleaseVersion <= dwRegValue)
			bRetValue = true;
	}

	return bRetValue;
}


/******************************************************************
Function Name:	IsNetfx46Installed
Description:	Uses the detection method recommended at
                http://msdn.microsoft.com/en-us/library/ee942965(v=vs.110).aspx
                to determine whether the .NET Framework 4.6 is
                installed on the machine
Inputs:         NONE
Results:        true if the .NET Framework 4.6 is installed
                false otherwise
******************************************************************/
bool IsNetfx46Installed()
{
	bool bRetValue = false;
	DWORD dwRegValue=0;

	if (RegistryGetValue(HKEY_LOCAL_MACHINE, g_szNetfx45RegKeyName, g_szNetfx45RegValueName, NULL, (LPBYTE)&dwRegValue, sizeof(DWORD)))
	{
		if (g_dwNetfx46ReleaseVersion <= dwRegValue)
			bRetValue = true;
	}

	return bRetValue;
}


/******************************************************************
Function Name:	IsNetfx461Installed
Description:	Uses the detection method recommended at
                http://msdn.microsoft.com/en-us/library/ee942965(v=vs.110).aspx
                to determine whether the .NET Framework 4.6.1 is
                installed on the machine
Inputs:         NONE
Results:        true if the .NET Framework 4.6.1 is installed
                false otherwise
******************************************************************/
bool IsNetfx461Installed()
{
	bool bRetValue = false;
	DWORD dwRegValue=0;

	if (RegistryGetValue(HKEY_LOCAL_MACHINE, g_szNetfx45RegKeyName, g_szNetfx45RegValueName, NULL, (LPBYTE)&dwRegValue, sizeof(DWORD)))
	{
		if (g_dwNetfx461ReleaseVersion <= dwRegValue)
			bRetValue = true;
	}

	return bRetValue;
}


/******************************************************************
Function Name:	IsNetfx462Installed
Description:	Uses the detection method recommended at
                http://msdn.microsoft.com/en-us/library/ee942965(v=vs.110).aspx
                to determine whether the .NET Framework 4.6.2 is
                installed on the machine
Inputs:         NONE
Results:        true if the .NET Framework 4.6.2 is installed
                false otherwise
******************************************************************/
bool IsNetfx462Installed()
{
	bool bRetValue = false;
	DWORD dwRegValue=0;

	if (RegistryGetValue(HKEY_LOCAL_MACHINE, g_szNetfx45RegKeyName, g_szNetfx45RegValueName, NULL, (LPBYTE)&dwRegValue, sizeof(DWORD)))
	{
		if (g_dwNetfx462ReleaseVersion <= dwRegValue)
			bRetValue = true;
	}

	return bRetValue;
}


/******************************************************************
Function Name:	GetNetfx10SPLevel
Description:	Uses the detection method recommended at
                http://blogs.msdn.com/astebner/archive/2004/09/14/229802.aspx
                to determine what service pack for the 
                .NET Framework 1.0 is installed on the machine
Inputs:	        NONE
Results:        integer representing SP level for .NET Framework 1.0
******************************************************************/
int GetNetfx10SPLevel()
{
	TCHAR szRegValue[MAX_PATH];
	TCHAR *pszSPLevel = NULL;
	int iRetValue = -1;
	bool bRegistryRetVal = false;

	// Need to detect what OS we are running on so we know what
	// registry key to use to look up the SP level
	if (IsCurrentOSTabletMedCenter())
		bRegistryRetVal = RegistryGetValue(HKEY_LOCAL_MACHINE, g_szNetfx10SPxOCMRegKeyName, g_szNetfxStandardVersionRegValueName, NULL, (LPBYTE)szRegValue, MAX_PATH);
	else
		bRegistryRetVal = RegistryGetValue(HKEY_LOCAL_MACHINE, g_szNetfx10SPxMSIRegKeyName, g_szNetfxStandardVersionRegValueName, NULL, (LPBYTE)szRegValue, MAX_PATH);

	if (bRegistryRetVal)
	{
		// This registry value should be of the format
		// #,#,#####,# where the last # is the SP level
		// Try to parse off the last # here
		pszSPLevel = _tcsrchr(szRegValue, _T(','));
		if (NULL != pszSPLevel)
		{
			// Increment the pointer to skip the comma
			pszSPLevel++;

			// Convert the remaining value to an integer
			iRetValue = _tstoi(pszSPLevel);
		}
	}

	return iRetValue;
}


/******************************************************************
Function Name:	GetNetfxSPLevel
Description:	Determine what service pack is installed for a 
                version of the .NET Framework using registry
				based detection methods documented in the 
				.NET Framework deployment guides.
Inputs:         pszNetfxRegKeyName - registry key name to use for detection
				pszNetfxRegValueName - registry value to use for detection
Results:        integer representing SP level for .NET Framework
******************************************************************/
int GetNetfxSPLevel(const TCHAR *pszNetfxRegKeyName, const TCHAR *pszNetfxRegValueName)
{
	DWORD dwRegValue=0;

	if (RegistryGetValue(HKEY_LOCAL_MACHINE, pszNetfxRegKeyName, pszNetfxRegValueName, NULL, (LPBYTE)&dwRegValue, sizeof(DWORD)))
	{
		return (int)dwRegValue;
	}

	// We can only get here if the .NET Framework is not
	// installed or there was some kind of error retrieving
	// the data from the registry
	return -1;
}


/******************************************************************
Function Name:	CheckNetfxBuildNumber
Description:	Retrieves the .NET Framework build number from
                the registry and validates that it is not a pre-release
                version number
Inputs:         NONE
Results:        true if the build number in the registry is greater
				than or equal to the passed in version; false otherwise
******************************************************************/
bool CheckNetfxBuildNumber(const TCHAR *pszNetfxRegKeyName, const TCHAR *pszNetfxRegKeyValue, const int iRequestedVersionMajor, const int iRequestedVersionMinor, const int iRequestedVersionBuild, const int iRequestedVersionRevision)
{
	TCHAR szRegValue[MAX_PATH];
	TCHAR *pszToken = NULL;
	TCHAR *pszNextToken = NULL;
	int iVersionPartCounter = 0;
	int iRegistryVersionMajor = 0;
	int iRegistryVersionMinor = 0;
	int iRegistryVersionBuild = 0;
	int iRegistryVersionRevision = 0;
	bool bRegistryRetVal = false;

	// Attempt to retrieve the build number registry value
	bRegistryRetVal = RegistryGetValue(HKEY_LOCAL_MACHINE, pszNetfxRegKeyName, pszNetfxRegKeyValue, NULL, (LPBYTE)szRegValue, MAX_PATH);

	if (bRegistryRetVal)
	{
		// This registry value should be of the format
		// #.#.#####.##.  Try to parse the 4 parts of
		// the version here
		pszToken = _tcstok_s(szRegValue, _T("."), &pszNextToken);
		while (NULL != pszToken)
		{
			iVersionPartCounter++;

			switch (iVersionPartCounter)
			{
			case 1:
				// Convert the major version value to an integer
				iRegistryVersionMajor = _tstoi(pszToken);
				break;
			case 2:
				// Convert the minor version value to an integer
				iRegistryVersionMinor = _tstoi(pszToken);
				break;
			case 3:
				// Convert the build number value to an integer
				iRegistryVersionBuild = _tstoi(pszToken);
				break;
			case 4:
				// Convert the revision number value to an integer
				iRegistryVersionRevision = _tstoi(pszToken);
				break;
			default:
				break;

			}

			// Get the next part of the version number
			pszToken = _tcstok_s(NULL, _T("."), &pszNextToken);
		}
	}

	// Compare the version number retrieved from the registry with
	// the version number of the final release of the .NET Framework
	// that we are checking
	if (iRegistryVersionMajor > iRequestedVersionMajor)
	{
		return true;
	}
	else if (iRegistryVersionMajor == iRequestedVersionMajor)
	{
		if (iRegistryVersionMinor > iRequestedVersionMinor)
		{
			return true;
		}
		else if (iRegistryVersionMinor == iRequestedVersionMinor)
		{
			if (iRegistryVersionBuild > iRequestedVersionBuild)
			{
				return true;
			}
			else if (iRegistryVersionBuild == iRequestedVersionBuild)
			{
				if (iRegistryVersionRevision >= iRequestedVersionRevision)
				{
					return true;
				}
			}
		}
	}

	// If we get here, the version in the registry must be less than the
	// version of the final release of the .NET Framework we are checking,
	// so return false
	return false;
}


bool IsCurrentOSTabletMedCenter()
{
	// Use GetSystemMetrics to detect if we are on a Tablet PC or Media Center OS  
	return ( (GetSystemMetrics(SM_TABLETPC) != 0) || (GetSystemMetrics(SM_MEDIACENTER) != 0) );
}


/******************************************************************
Function Name:	RegistryGetValue
Description:	Get the value of a reg key
Inputs:			HKEY hk - The hk of the key to retrieve
				TCHAR *pszKey - Name of the key to retrieve
				TCHAR *pszValue - The value that will be retrieved
				DWORD dwType - The type of the value that will be retrieved
				LPBYTE data - A buffer to save the retrieved data
				DWORD dwSize - The size of the data retrieved
Results:		true if successful, false otherwise
******************************************************************/
bool RegistryGetValue(HKEY hk, const TCHAR * pszKey, const TCHAR * pszValue, DWORD dwType, LPBYTE data, DWORD dwSize)
{
	HKEY hkOpened;

	// Try to open the key
	if (RegOpenKeyEx(hk, pszKey, 0, KEY_READ, &hkOpened) != ERROR_SUCCESS)
	{
		return false;
	}

	// If the key was opened, try to retrieve the value
	if (RegQueryValueEx(hkOpened, pszValue, 0, &dwType, (LPBYTE)data, &dwSize) != ERROR_SUCCESS)
	{
		RegCloseKey(hkOpened);
		return false;
	}
	
	// Clean up
	RegCloseKey(hkOpened);

	return true;
}

int APIENTRY _tWinMain(HINSTANCE hInstance,
                     HINSTANCE hPrevInstance,
                     LPTSTR    lpCmdLine,
                     int       nCmdShow)
{
	int iNetfx10SPLevel = -1;
	int iNetfx11SPLevel = -1;
	int iNetfx20SPLevel = -1;
	int iNetfx30SPLevel = -1;
	int iNetfx35SPLevel = -1;
	int iNetfx40ClientSPLevel = -1;
	int iNetfx40FullSPLevel = -1;
	int iNetfx45SPLevel = -1;
	int iNetfx451SPLevel = -1;
	int iNetfx452SPLevel = -1;
	int iNetfx46SPLevel = -1;
	int iNetfx461SPLevel = -1;
	int iNetfx462SPLevel = -1;
	TCHAR szMessage[MAX_PATH];
	TCHAR szOutputString[MAX_PATH*20];

	// Determine whether or not the .NET Framework
	// 1.0, 1.1, 2.0, 3.0, 3.5, 4, 4.5, 4.5.1, 4.5.2, 4.6, 4.6.1, or 4.6.2 are installed
	bool bNetfx10Installed = IsNetfx10Installed();
	bool bNetfx11Installed = IsNetfx11Installed();
	bool bNetfx20Installed = IsNetfx20Installed();

	// The .NET Framework 3.0 is an add-in that installs
	// on top of the .NET Framework 2.0.  For this version
	// check, validate that both 2.0 and 3.0 are installed.
	bool bNetfx30Installed = (IsNetfx20Installed() && IsNetfx30Installed());

	// The .NET Framework 3.5 is an add-in that installs
	// on top of the .NET Framework 2.0 and 3.0.  For this version
	// check, validate that 2.0, 3.0 and 3.5 are installed.
	bool bNetfx35Installed = (IsNetfx20Installed() && IsNetfx30Installed() && IsNetfx35Installed());

	bool bNetfx40ClientInstalled = (IsNetfx40ClientInstalled());
	bool bNetfx40FullInstalled = (IsNetfx40FullInstalled());

	bool bNetfx45Installed = (IsNetfx45Installed());
	bool bNetfx451Installed = (IsNetfx451Installed());
	bool bNetfx452Installed = (IsNetfx452Installed());
	bool bNetfx46Installed = (IsNetfx46Installed());
	bool bNetfx461Installed = (IsNetfx461Installed());
	bool bNetfx462Installed = (IsNetfx462Installed());

	// If .NET Framework 1.0 is installed, get the
	// service pack level
	if (bNetfx10Installed)
	{
		iNetfx10SPLevel = GetNetfx10SPLevel();

		if (iNetfx10SPLevel > 0)
			_stprintf_s(szMessage, MAX_PATH, _T(".NET Framework 1.0 service pack %i is installed."), iNetfx10SPLevel);
		else
			_stprintf_s(szMessage, MAX_PATH, _T(".NET Framework 1.0 is installed with no service packs."));

		_tcscpy_s(szOutputString, szMessage);
	}
	else
	{
		_tcscpy_s(szOutputString, _T(".NET Framework 1.0 is not installed."));
	}

	// If .NET Framework 1.1 is installed, get the
	// service pack level
	if (bNetfx11Installed)
	{
		iNetfx11SPLevel = GetNetfxSPLevel(g_szNetfx11RegKeyName, g_szNetfxStandardSPxRegValueName);

		if (iNetfx11SPLevel > 0)
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 1.1 service pack %i is installed."), iNetfx11SPLevel);
		else
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 1.1 is installed with no service packs."));

		_tcscat_s(szOutputString, szMessage);
	}
	else
	{
		_tcscat_s(szOutputString, _T("\n\n.NET Framework 1.1 is not installed."));
	}

	// If .NET Framework 2.0 is installed, get the
	// service pack level
	if (bNetfx20Installed)
	{
		iNetfx20SPLevel = GetNetfxSPLevel(g_szNetfx20RegKeyName, g_szNetfxStandardSPxRegValueName);

		if (iNetfx20SPLevel > 0)
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 2.0 service pack %i is installed."), iNetfx20SPLevel);
		else
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 2.0 is installed with no service packs."));

		_tcscat_s(szOutputString, szMessage);
	}
	else
	{
		_tcscat_s(szOutputString, _T("\n\n.NET Framework 2.0 is not installed."));
	}

	// If .NET Framework 3.0 is installed, get the
	// service pack level
	if (bNetfx30Installed)
	{
		iNetfx30SPLevel = GetNetfxSPLevel(g_szNetfx30SpRegKeyName, g_szNetfxStandardSPxRegValueName);

		if (iNetfx30SPLevel > 0)
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 3.0 service pack %i is installed."), iNetfx30SPLevel);
		else
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 3.0 is installed with no service packs."));

		_tcscat_s(szOutputString, szMessage);
	}
	else
	{
		_tcscat_s(szOutputString, _T("\n\n.NET Framework 3.0 is not installed."));
	}

	// If .NET Framework 3.5 is installed, get the
	// service pack level
	if (bNetfx35Installed)
	{
		iNetfx35SPLevel = GetNetfxSPLevel(g_szNetfx35RegKeyName, g_szNetfxStandardSPxRegValueName);

		if (iNetfx35SPLevel > 0)
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 3.5 service pack %i is installed."), iNetfx35SPLevel);
		else
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 3.5 is installed with no service packs."));

		_tcscat_s(szOutputString, szMessage);
	}
	else
	{
		_tcscat_s(szOutputString, _T("\n\n.NET Framework 3.5 is not installed."));
	}

	// If .NET Framework 4 Client is installed, get the
	// service pack level
	if (bNetfx40ClientInstalled)
	{
		iNetfx40ClientSPLevel = GetNetfxSPLevel(g_szNetfx40ClientRegKeyName, g_szNetfx40SPxRegValueName);

		if (iNetfx40ClientSPLevel > 0)
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4 client service pack %i is installed."), iNetfx40ClientSPLevel);
		else
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4 client is installed with no service packs."));

		_tcscat_s(szOutputString, szMessage);
	}
	else
	{
		_tcscat_s(szOutputString, _T("\n\n.NET Framework 4 client is not installed."));
	}

	// If .NET Framework 4 Full is installed, get the
	// service pack level
	if (bNetfx40FullInstalled)
	{
		iNetfx40FullSPLevel = GetNetfxSPLevel(g_szNetfx40FullRegKeyName, g_szNetfx40SPxRegValueName);

		if (iNetfx40FullSPLevel > 0)
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4 full service pack %i is installed."), iNetfx40FullSPLevel);
		else
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4 full is installed with no service packs."));

		_tcscat_s(szOutputString, szMessage);
	}
	else
	{
		_tcscat_s(szOutputString, _T("\n\n.NET Framework 4 full is not installed."));
	}

	// If .NET Framework 4.5 is installed, get the
	// service pack level
	if (bNetfx45Installed)
	{
		iNetfx45SPLevel = GetNetfxSPLevel(g_szNetfx45RegKeyName, g_szNetfx40SPxRegValueName);

		if (iNetfx45SPLevel > 0)
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4.5 service pack %i is installed."), iNetfx45SPLevel);
		else
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4.5 is installed with no service packs."));

		_tcscat_s(szOutputString, szMessage);
	}
	else
	{
		_tcscat_s(szOutputString, _T("\n\n.NET Framework 4.5 is not installed."));
	}

	// If .NET Framework 4.5.1 is installed, get the
	// service pack level
	if (bNetfx451Installed)
	{
		iNetfx451SPLevel = GetNetfxSPLevel(g_szNetfx45RegKeyName, g_szNetfx40SPxRegValueName);

		if (iNetfx451SPLevel > 0)
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4.5.1 service pack %i is installed."), iNetfx451SPLevel);
		else
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4.5.1 is installed with no service packs."));

		_tcscat_s(szOutputString, szMessage);
	}
	else
	{
		_tcscat_s(szOutputString, _T("\n\n.NET Framework 4.5.1 is not installed."));
	}

	// If .NET Framework 4.5.2 is installed, get the
	// service pack level
	if (bNetfx452Installed)
	{
		iNetfx452SPLevel = GetNetfxSPLevel(g_szNetfx45RegKeyName, g_szNetfx40SPxRegValueName);

		if (iNetfx452SPLevel > 0)
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4.5.2 service pack %i is installed."), iNetfx452SPLevel);
		else
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4.5.2 is installed with no service packs."));

		_tcscat_s(szOutputString, szMessage);
	}
	else
	{
		_tcscat_s(szOutputString, _T("\n\n.NET Framework 4.5.2 is not installed."));
	}

	// If .NET Framework 4.6 is installed, get the
	// service pack level
	if (bNetfx46Installed)
	{
		iNetfx46SPLevel = GetNetfxSPLevel(g_szNetfx45RegKeyName, g_szNetfx40SPxRegValueName);

		if (iNetfx46SPLevel > 0)
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4.6 service pack %i is installed."), iNetfx46SPLevel);
		else
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4.6 is installed with no service packs."));

		_tcscat_s(szOutputString, szMessage);
	}
	else
	{
		_tcscat_s(szOutputString, _T("\n\n.NET Framework 4.6 is not installed."));
	}

	// If .NET Framework 4.6.1 is installed, get the
	// service pack level
	if (bNetfx461Installed)
	{
		iNetfx461SPLevel = GetNetfxSPLevel(g_szNetfx45RegKeyName, g_szNetfx40SPxRegValueName);

		if (iNetfx461SPLevel > 0)
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4.6.1 service pack %i is installed."), iNetfx461SPLevel);
		else
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4.6.1 is installed with no service packs."));

		_tcscat_s(szOutputString, szMessage);
	}
	else
	{
		_tcscat_s(szOutputString, _T("\n\n.NET Framework 4.6.1 is not installed."));
	}

	// If .NET Framework 4.6.2 is installed, get the
	// service pack level
	if (bNetfx462Installed)
	{
		iNetfx462SPLevel = GetNetfxSPLevel(g_szNetfx45RegKeyName, g_szNetfx40SPxRegValueName);

		if (iNetfx462SPLevel > 0)
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4.6.2 service pack %i is installed."), iNetfx462SPLevel);
		else
			_stprintf_s(szMessage, MAX_PATH, _T("\n\n.NET Framework 4.6.2 is installed with no service packs."));

		_tcscat_s(szOutputString, szMessage);
	}
	else
	{
		_tcscat_s(szOutputString, _T("\n\n.NET Framework 4.6.2 is not installed."));
	}

	MessageBox(NULL, szOutputString, _T(".NET Framework Install Info"), MB_OK | MB_ICONINFORMATION);

	return 0;
}

