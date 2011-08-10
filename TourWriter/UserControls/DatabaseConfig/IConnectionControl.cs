using System.Collections.Specialized;

namespace TourWriter.UserControls.DatabaseConfig
{
    public interface IConnectionControl
    {
        bool ValidateAndFinalise();
    }
}
