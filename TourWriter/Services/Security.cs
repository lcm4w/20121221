using System.IO;
using System.Security.AccessControl;

namespace TourWriter.Services
{
    class Security
    {
        internal static void SetAcl(string path, string identity, FileSystemRights rights)
        {
            var info = new DirectoryInfo(path);
            var acl = info.GetAccessControl();

            var rule1 = new FileSystemAccessRule(identity, rights, AccessControlType.Allow);
            bool modified;
            acl.ModifyAccessRule(AccessControlModification.Reset, rule1, out modified);

            var rule2 = new FileSystemAccessRule(identity, rights,
                                                 InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                                                 PropagationFlags.InheritOnly, AccessControlType.Allow);

            acl.ModifyAccessRule(AccessControlModification.Add, rule2, out modified);
            info.SetAccessControl(acl);
        }
    }
}
