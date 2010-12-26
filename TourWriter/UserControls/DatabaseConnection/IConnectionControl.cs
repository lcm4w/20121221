
namespace TourWriter.UserControls.DatabaseConnection
{
    public interface IConnectionControl
    {
        string GetServerName();
        string GetUserName();
        string GetPassword();
        string GetRemoteName();
        string GetRemoteConnection();
        bool ValidateAndFinalise();
    }
}
