using System.Collections.Specialized;

namespace TourWriter.UserControls.DatabaseConnection
{
    public interface IConnectionControl
    {
        bool ValidateAndFinalise();
    }
}
