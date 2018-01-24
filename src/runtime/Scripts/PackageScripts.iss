; 脚本由 Inno Setup 脚本向导 生成！
; 有关创建 Inno Setup 脚本文件的详细资料请查阅帮助文档！

#define MyAppName "智能打印SDK"
#define MyAppVersion "1.1.0"
#define MyAppPublisher "北京我的公司"
#define MyAppURL "http://www.example.com/"
#define MyCopyright "(c)北京我的公司 版权所有"
#define WorkBenchPrcessName "SmartClient.Bootstraper"
[Setup]
; 注: AppId的值为单独标识该应用程序。
; 不要为其他安装程序使用相同的AppId值。
; (生成新的GUID，点击 工具|在IDE中生成GUID。)
AppId={{F797929C-42D4-4A37-8805-A1B3816C0B78}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName=C:\Spider
DisableDirPage=yes
DefaultGroupName={#MyAppName}
InfoBeforeFile=C:\SpiderPackageFiles\license.txt
OutputDir=C:\Output
OutputBaseFilename=SmartPrintSDK
SetupIconFile=C:\SpiderPackageFiles\printer.ico
;Compression=lzma
SolidCompression=yesVersionInfoCompany={#MyAppPublisher}
VersionInfoVersion={#MyAppVersion}
VersionInfoCopyright={#MyCopyright}
;no为不重启Yes为重启
AlwaysRestart=no 
RestartIfNeededByRun=no
PrivilegesRequired=admin

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"
[CustomMessages]     
;%n%n%n%n%n  at  here can be new line
ComponentDescription=安装菜鸟打印组件核心。注意：如果已经安装菜鸟打印组件，可以不再从新安装。否则必须安装组件！

[Files]

Source: "C:\SpiderPackageFiles\7z.dll"; DestDir: "{app}"; Flags: ignoreversion deleteafterinstall ;BeforeInstall: BeforeInstallCheck
Source: "C:\SpiderPackageFiles\7z.exe"; DestDir: "{app}"; Flags: ignoreversion deleteafterinstall ;BeforeInstall: TaskKill('SmartClient.Bootstraper.exe')
Source: "C:\SpiderPackageFiles\CaiNiaoPackage.7z"; DestDir: "{app}"; Flags: ignoreversion deleteafterinstall ;
Source: "C:\SpiderPackageFiles\license.txt"; DestDir: "{app}"; Flags: ignoreversion deleteafterinstall ;
Source: "C:\SpiderPackageFiles\printer.ico"; DestDir: "{app}"; Flags: ignoreversion deleteafterinstall ;
Source: "C:\SpiderPackageFiles\hook.exe"; DestDir: "{app}\bin"; Flags: ignoreversion deleteafterinstall ;
Source: "C:\SpiderPackageFiles\appPackage.7z"; DestDir: "{app}"; Flags: ignoreversion deleteafterinstall ;
Source: "C:\SpiderPackageFiles\system.7z"; DestDir: "{app}"; Flags: ignoreversion deleteafterinstall ;
Source: "C:\SpiderPackageFiles\vc.7z"; DestDir: "{app}"; Flags: ignoreversion deleteafterinstall ;
Source: "C:\SpiderPackageFiles\webserver.bin"; DestDir: "{app}"; Flags: ignoreversion deleteafterinstall ;
;Source: "C:\Spider\setup.bat"; DestDir: "{app}"; Flags: ignoreversion ;

; 注意: 不要在任何共享系统文件上使用“Flags: ignoreversion”  ;Flags: skipifsilent shellexec runhidden  nowait postinstall
[Code]


var ResultCode:Integer;

procedure BeforeInstallCheck();
begin
Exec(ExpandConstant('{cmd}'), ExpandConstant('/c if exist "{app}\bin\hook.exe" {app}\bin\hook.exe -remove pause'),ExpandConstant('{app}'), SW_SHOWNORMAL, ewNoWait, ResultCode);

end;

procedure TaskKill(FileName: String);
var
  ResultCode: Integer;
begin
    Exec(ExpandConstant('taskkill.exe'), '/f /im ' + '"' + FileName + '"', '', SW_HIDE,
     ewWaitUntilTerminated, ResultCode);
end;



function InitializeUninstall(): Boolean;
begin
  TaskKill('SmartClient.Bootstraper.exe')
  Result :=True;
end;

function NeedRestart (): Boolean;
begin
   Result :=False;
end;

function UninstallNeedRestart (): Boolean; 
begin

  Exec(ExpandConstant('{cmd}'), ExpandConstant('/c if exist "{app}" rd /s /q "{app}"'),ExpandConstant('{app}'), SW_SHOWNORMAL, ewNoWait, ResultCode);
  Result :=False;

end;


#IFDEF UNICODE
  #DEFINE AW "W"
#ELSE
  #DEFINE AW "A"
#ENDIF
type
  INSTALLSTATE = Longint;
const
  INSTALLSTATE_INVALIDARG = -2;  { An invalid parameter was passed to the function. }
  INSTALLSTATE_UNKNOWN = -1;     { The product is neither advertised or installed. }
  INSTALLSTATE_ADVERTISED = 1;   { The product is advertised but not installed. }
  INSTALLSTATE_ABSENT = 2;       { The product is installed for a different user. }
  INSTALLSTATE_DEFAULT = 5;      { The product is installed for the current user. }



  { Visual C++ 2015 Redistributable 14.0.23026 }
  VC_2015_REDIST_X86_MIN = '{A2563E55-3BEC-3828-8D67-E5E8B9E8B675}';

  VC_2015_REDIST_X86_ADD = '{BE960C1C-7BAD-3DE6-8B1A-2616FE532845}';




function MsiQueryProductState(szProduct: string): INSTALLSTATE; 
  external 'MsiQueryProductState{#AW}@msi.dll stdcall';

function VCVersionInstalled(const ProductID: string): Boolean;
begin
  Result := MsiQueryProductState(ProductID) = INSTALLSTATE_DEFAULT;
end;

function VCRedistNeedsInstall: Boolean;
begin
  { here the Result must be True when you need to install your VCRedist }
  { or False when you don't need to, so now it's upon you how you build }
  { this statement, the following won't install your VC redist only when }
  { the Visual C++ 2010 Redist (x86) and Visual C++ 2010 SP1 Redist(x86) }
  { are installed for the current user }
  Result := not (VCVersionInstalled(VC_2015_REDIST_X86_ADD));
end;

function WebServerRuntimeNotInstall: Boolean;
begin
  { check the webserver runtime  }
  Result := not RegKeyExists(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\.NETFramework\policy\v4.0');
      if Result=True then
       begin
        RegWriteStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\.NETFramework\policy', 'need','1'); 
       end;
end;

 function WebServerRuntimeHasNotV4SubKey: Boolean;
var
  token: String;
begin
  { check the webserver runtime  }
  Result :=True

  RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\.NETFramework\policy\v4.0','30319',token)
  if token='30319-30319' then
  begin
    // Successfully read the value
       Result :=False
  end;
end;

function WebServerRuntimeIsNeedInstall: Boolean;
var
  token: String;
begin
  { check the webserver runtime  }
  RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\.NETFramework\policy','need',token)
  if token='1' then
  begin
    // Successfully read the value
       Result :=True
  end;
end;

procedure CompletedInstallWebServerRuntime();
var
 token: String;
begin
  { check the webserver runtime  }
   RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\.NETFramework\policy','need',token)
  if token='1' then
  begin
     RegWriteStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\.NETFramework\policy', 'need','0');
  end;
end;

[Registry]
;4.0.net framework必须注册 installroot 这个项，X64机器下面 如果未安装，那么默认使用X86的.net framerok .不支持x64版本的.net framework
Root: HKLM; Subkey: "SOFTWARE\Microsoft\.NETFramework";Permissions:admins-full; ValueType: string; ValueName: "InstallRoot"; ValueData: "{win}\Microsoft.NET\Framework\";Check:WebServerRuntimeNotInstall
Root: HKLM; Subkey: "SOFTWARE\Microsoft\.NETFramework\policy\v4.0";Permissions:admins-full;Check:WebServerRuntimeNotInstall
Root: HKLM; Subkey: "SOFTWARE\Microsoft\.NETFramework\policy\v4.0"; Permissions:admins-full; ValueType: string; ValueName: "30319"; ValueData: "30319-30319";Check:WebServerRuntimeHasNotV4SubKey


[Run]

;Filename: "{app}\setup.bat"; Description: "Installing Microsoft Visual C++ Runtime ...";Flags:shellexecF  ---vc.exe StatusMsg: "Installing Microsoft Visual C++ Runtime ..."; Check: VCRedistNeedsInstall
Filename: "{app}\7z.exe"; Parameters: "x system.7z -o{win}\system32\ -aoa -y";StatusMsg: "Checking  Runtime Support Library...";Flags:shellexec runhidden; Check: WebServerRuntimeIsNeedInstall
Filename: "{app}\7z.exe"; Parameters: "x webserver.bin -o{win}\Microsoft.NET\ -aoa -y";StatusMsg: "Installing Microsoft WebServer Runtime ...";Flags:runhidden waituntilterminated; Check: WebServerRuntimeIsNeedInstall
Filename: "{app}\7z.exe";Parameters: "x appPackage.7z -o{app}\bin\app -aoa -y";StatusMsg: "Installing Application...";Flags:runhidden waituntilterminated;
Filename: "{app}\7z.exe";Parameters: "x vc.7z -o{app}\bin\ -aoa";StatusMsg: "Installing Microsoft VC++ Runtime ...";Flags:runhidden waituntilterminated;
Filename: "{app}\7z.exe";Parameters: "x CaiNiaoPackage.7z -o{app}\bin\CaiNiao -aoa";StatusMsg: "Installing CaiNiao Printer...";Flags:runhidden waituntilterminated;
;Filename: "{app}\bin\hook.exe"; Parameters: "-remove"
Filename: "{app}\bin\hook.exe"; Parameters: "-install";StatusMsg: "Start SmartPrint Service..."; Flags:runhidden shellexec waituntilterminated;;Filename: "{app}\CaiNiaoSetUp.exe";Description: "{cm:ComponentDescription}";Flags: postinstall  shellexec;AfterInstall:CompletedInstallWebServerRuntime() 


[UninstallRun]
Filename: "{app}\bin\hook.exe"; Parameters: "-remove";Flags:shellexec;


