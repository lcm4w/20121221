using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TourWriter.UserControls.DatabaseConfig
{
    static class InstallHelper
    {
        // http://www.microsoft.com/downloads/en/details.aspx?FamilyID=8B3695D9-415E-41F0-A079-25AB0412424B
        internal const string X32 = "http://go.microsoft.com/fwlink/?LinkId=186782&clcid=0x409"; // 32 bit
        internal const string X64 = "http://go.microsoft.com/fwlink/?LinkId=186784&clcid=0x409"; // 64 bit
        internal const string X86 = "http://go.microsoft.com/fwlink/?LinkId=186783&clcid=0x409"; // 32 bit WoW
        internal const string MSI45 = "http://www.microsoft.com/downloads/en/details.aspx?FamilyID=5a58b56f-60b6-4412-95b9-54d056d6f9f4&displaylang=en";
        
        #region Install args and sql cmds

        internal const string InstallArgs = @"
/QUIET
/HIDECONSOLE
/IACCEPTSQLSERVERLICENSETERMS
/ACTION=install
/FEATURES=SQLEngine
/INSTANCENAME=TourWriter
/TCPENABLED=1
/NPENABLED=1
/SQLSVCSTARTUPTYPE=Automatic
/BROWSERSVCSTARTUPTYPE=Automatic
/SAPWD=TwDbSaPsWd9191
/ERRORREPORTING=0
/SECURITYMODE=SQL
/SQLSVCACCOUNT=""NT AUTHORITY\SYSTEM""
";

        internal const string RestoreDbSql = @"
USE [master];
PRINT N'Dropping database';
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'TourWriter') 
    DROP DATABASE [TourWriter];
PRINT N'Creating database';
CREATE DATABASE [TourWriter];
PRINT N'Creating login';
IF NOT EXISTS (SELECT * FROM master.dbo.syslogins WHERE loginname = N'twuser')
    CREATE LOGIN [twuser] WITH PASSWORD = 'twu505', CHECK_POLICY = OFF, CHECK_EXPIRATION = OFF;
PRINT N'Restoring database';
ALTER DATABASE [TourWriter] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
RESTORE DATABASE [TourWriter] FROM DISK = '{0}' WITH REPLACE;
ALTER DATABASE [TourWriter] SET MULTI_USER;
PRINT 'The database update succeeded';
";

        internal const string CreateLoginUserSql = @"
USE [TourWriter];
PRINT N'Creating user';
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'twuser')
    CREATE USER [twuser] FOR LOGIN [twuser] WITH DEFAULT_SCHEMA=[dbo];
PRINT N'Creating role';
EXEC sp_addrolemember N'db_owner', N'twuser';
PRINT N'Updating user';
EXEC sp_change_users_login 'Update_One', 'twuser', 'twuser';
PRINT 'The database update succeeded';
";

        internal const string InitialiseDbSql = @"
USE [TourWriter];
PRINT N'Updating AppSettings';
UPDATE AppSettings SET InstallID = NEWID(), InstallName = N'trial';
PRINT 'The database update succeeded';
";

        #endregion

        #region Platform info

        // NOTE: .NET 4 has two new properties in the Environment class, Is64BitProcess and Is64BitOperatingSystem. 

        internal static bool Is64BitOperatingSystem()
        {
            var is64BitProcess = (IntPtr.Size == 8);
            return is64BitProcess || InternalCheckIsWow64();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (var p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    return IsWow64Process(p.Handle, out retVal) && retVal;
                }
            }
            return false;
        }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        #endregion

        #region Windows Installer 4.5

        public static bool IsWindowsInstaller45Installed()
        {
            FileVersionInfo info;
            var fileName = Path.Combine(Environment.SystemDirectory, "msi.dll");
            try
            {
                info = FileVersionInfo.GetVersionInfo(fileName);
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            return (info.FileMajorPart > 4 || info.FileMajorPart == 4 && info.FileMinorPart >= 5);
        }
        #endregion

        internal static string GetSqlCmdPath()
        {
            const string file1 = @"C:\Program Files (x86)\Microsoft SQL Server\90\Tools\Binn\SqlCmd.exe";
            const string file2 = @"C:\Program Files (x86)\Microsoft SQL Server\100\Tools\Binn\SqlCmd.exe";

            const string file3 = @"C:\Program Files\Microsoft SQL Server\90\Tools\Binn\SqlCmd.exe";
            const string file4 = @"C:\Program Files\Microsoft SQL Server\100\Tools\Binn\SqlCmd.exe";
            
            return 
                File.Exists(file1) ? file1 : 
                File.Exists(file2) ? file2 : 
                File.Exists(file3) ? file3 : 
                File.Exists(file4) ? file4 : "";
        }

        internal static bool TestLocalServerConnect(out string error)
        {
            const string test = "connected_to_tourwriter_server";
            try
            {
                error = "";
                return RunSql(string.Format("set nocount on; select N'{0}';", test)).Contains(test);
            }
            catch(Exception x)
            {
                error = x.Message;
                return false;
            }
        }

        internal static bool TestDatabaseConnect(out string error)
        {
            const string test = "connected_to_tourwriter_database";
            try
            {
                error = "";
                //return RunSql(string.Format("set nocount on; select N'{0}' from [sys].[databases] where [name] = N'TourWriter'", test)).Contains(test);
                return RunSql(string.Format("set nocount on; use TourWriter; select N'{0}' from AppSettings", test)).Contains(test);
            }
            catch (Exception x)
            {
                error = x.Message;
                return false;
            }
        }

        internal static string RunSql(string sql)
        {
            var output = "";
            using (var proc = new Process())
            {
                var cmd = GetSqlCmdPath();
                if (string.IsNullOrEmpty(cmd)) throw new Exception("Failed to find Sql Server SQLCMD application");

                proc.StartInfo.FileName = cmd;
                proc.StartInfo.Arguments = string.Format(@"-S {0}\TourWriter -Q ""{1}""", Environment.MachineName, sql);
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                output += proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                output += proc.StandardOutput.ReadToEnd();
                proc.Close();
            }
            return output.Replace("twu505", "");
        }

        internal static void DeleteFile(string file)
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Application.DoEvents();
                File.Delete(file);
            }
            catch { }
        }

        static public string AssemblyDirectory
        {
            get
            {
                // this is better, Assembly.Location property sometimes gives strange results...

                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
