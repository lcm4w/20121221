using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("TourWriter.Client")]
[assembly: AssemblyDescription("TourWriter client application")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
//
// In order to sign your assembly you must specify a key to use. Refer to the 
// Microsoft .NET Framework documentation for more information on assembly signing.
//
// Use the attributes below to control which key is used for signing. 
//
// Notes: 
//   (*) If no key is specified, the assembly is not signed.
//   (*) KeyName refers to a key that has been installed in the Crypto Service
//       Provider (CSP) on your machine. KeyFile refers to a file which contains
//       a key.
//   (*) If the KeyFile and the KeyName values are both specified, the 
//       following processing occurs:
//       (1) If the KeyName can be found in the CSP, that key is used.
//       (2) If the KeyName does not exist and the KeyFile does exist, the key 
//           in the KeyFile is installed into the CSP and used.
//   (*) In order to create a KeyFile, you can use the sn.exe (Strong Name) utility.
//       When specifying the KeyFile, the location of the KeyFile should be
//       relative to the project output directory which is
//       %Project Directory%\obj\<configuration>. For example, if your KeyFile is
//       located in the project directory, you would specify the AssemblyKeyFile 
//       attribute as [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
//       documentation for more information on this.
//
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]


#region Helper class to get information for the About form.
// This class uses the System.Reflection.Assembly class to
// access assembly meta-data.
// This class is not a normal feature of AssemblyInfo.cs
public class AssemblyInfo
{
	// Used by Helper functions to access information from Assembly Attributes
	private System.Type appType; 

	public AssemblyInfo()
	{
	    appType = typeof (TourWriter.Forms.MainForm);
	}

	public string CodeBase
	{
		get
		{
			return appType.Assembly.CodeBase;
		}
	}

	public string Copyright
	{
		get
		{			
			System.Type aType = typeof(AssemblyCopyrightAttribute);
			System.Object[] array = appType.Assembly.GetCustomAttributes(aType, false);
			AssemblyCopyrightAttribute atrib = array[0] as AssemblyCopyrightAttribute;
			return atrib.Copyright;
		}
	}

	public string Company
	{
		get
		{			
			System.Type aType = typeof(AssemblyCompanyAttribute);
			System.Object[] array = appType.Assembly.GetCustomAttributes(aType, false);
			AssemblyCompanyAttribute atrib = array[0] as AssemblyCompanyAttribute;
			return atrib.Company;
		}
	}

	public string Description
	{
		get
		{			
			System.Type aType = typeof(AssemblyDescriptionAttribute);
			System.Object[] array = appType.Assembly.GetCustomAttributes(aType, false);
			AssemblyDescriptionAttribute atrib = array[0] as AssemblyDescriptionAttribute;
			return atrib.Description;
		}
	}

	public string Product
	{
		get
		{			
			System.Type aType = typeof(AssemblyProductAttribute);
			System.Object[] array = appType.Assembly.GetCustomAttributes(aType, false);
			AssemblyProductAttribute atrib = array[0] as AssemblyProductAttribute;
			return atrib.Product;
		}
	}

	public string Title
	{
		get
		{			
			System.Type aType = typeof(AssemblyTitleAttribute);
			System.Object[] array = appType.Assembly.GetCustomAttributes(aType, false);
			AssemblyTitleAttribute atrib = array[0] as AssemblyTitleAttribute;
			return atrib.Title;
		}
	}

    public static string FileVersion
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly.Location == null) return "0";
            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return string.Format("{0}.{1}.{2}.{3}", 
                fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
        }
    }

    public static string InformationalVersion
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly.Location == null) return "0";
            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.ProductVersion;
        }
    }

    public static string RevisionNumber
    {
        get
        {
            const string pattern = @"(\w*)(?=\))";
            return Regex.Match(InformationalVersion, pattern).Value;
        }
    }

    public static string AssemblyDirectory
    {
        get
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
#endregion