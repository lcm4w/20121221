using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TourWriter.Services;

namespace TourWriter.UserControls.Reports
{
    public class RdlcFileHelper
    {
        #region Regex patterns
        private const string RegexDataSources = @"<DataSet.*?<DataSourceName>(?<name>\w*)</DataSourceName>.*?<CommandText>(?<sql>.*?)</CommandText>.*?</DataSet>";
        private const string RegexTopMargin = @"(?<=<TopMargin>)\d*\.?\d+(?=cm</TopMargin>)";
        private const string RegexBottomMargin = @"(?<=<BottomMargin>)\d*\.?\d+(?=cm</BottomMargin>)";
        private const string RegexLeftMargin = @"(?<=<LeftMargin>)\d*\.?\d+(?=cm</LeftMargin>)";
        private const string RegexRightMargin = @"(?<=<RightMargin>)\d*\.?\d+(?=cm</RightMargin>)";
        private const string RegexSpacing = "(?<=<TablixRow>\r\n\\s*<Height>)\\d*\\.?\\d+(?=cm</Height>\r\n\\s*<TablixCells>\r\n\\s*<TablixCell>\r\n\\s*<CellContents>\r\n\\s*<Textbox Name=\"SpacerCell\">)";

        private const string RegexSqlExpression = @"(where|and|or) *\(? *(?<col>\w*) (([a-zA-Z=<>!]* ){1,3})(?<param>@\w*) *\)?";
        #endregion

        private readonly string _rdlcFile;
        private string _rdlc;

        public RdlcFileHelper(string rdlcFile)
        {
            _rdlcFile = rdlcFile;
            ReadFile(rdlcFile);
        }

        private void ReadFile(string file)
        {
            using (TextReader reader = new StreamReader(file))
                _rdlc = reader.ReadToEnd();
        }

        internal void WriteFile()
        {
            using (TextWriter writer = new StreamWriter(_rdlcFile))
                writer.Write(_rdlc);
        }

        private object GetValue(string pattern, ref bool exists)
        {
            return GetValue(pattern, ref exists, RegexOptions.Multiline);
        }

        private object GetValue(string pattern, ref bool exists, RegexOptions regexOption)
        {
            var match = Regex.Match(_rdlc, pattern, regexOption);
            exists = match.Success;
            return match.Value.XmlDecode();
        }

        private void SetValue(string pattern, object value)
        {
            _rdlc = new Regex(pattern, RegexOptions.Singleline).Replace(_rdlc, value.ToString());
        }

        #region Getter methods

        internal double? GetTopMargin(ref bool exists)
        {
            double d;
            var val = GetValue(RegexTopMargin, ref exists);
            return double.TryParse(val.ToString(), out d) ? (double?)d : null;
        }

        internal double? GetBottomMargin(ref bool exists)
        {
            double d;
            var val = GetValue(RegexBottomMargin, ref exists);
            return double.TryParse(val.ToString(), out d) ? (double?)d : null;
        }

        internal double? GetLeftMargin(ref bool exists)
        {
            double d;
            var val = GetValue(RegexLeftMargin, ref exists);
            return double.TryParse(val.ToString(), out d) ? (double?)d : null;
        }

        internal double? GetRightMargin(ref bool exists)
        {
            double d;
            var val = GetValue(RegexRightMargin, ref exists, RegexOptions.Singleline);
            return double.TryParse(val.ToString(), out d) ? (double?)d : null;
        }

        internal double? GetSpacing(ref bool exists)
        {
            double d;
            var val = GetValue(RegexSpacing, ref exists, RegexOptions.Singleline);
            return double.TryParse(val.ToString(), out d) ? (double?)d : null;
        }

        internal Dictionary<string, string> GetDataSourcesNameAndSql(ref bool exists)
        {
            var matches = Regex.Matches(_rdlc, RegexDataSources, RegexOptions.Singleline);
            exists = matches.Count > 0;

            var dataSources = new Dictionary<string, string>(matches.Count);
            foreach (Match match in matches)
                dataSources.Add(match.Groups["name"].Value.XmlDecode(), match.Groups["sql"].Value.XmlDecode());

            return dataSources;
        }

        internal List<SqlExpression> GetSqlExpressions()
        {
            var exists = false;
            var paramList = new List<SqlExpression>();
            foreach(var dataSource in GetDataSourcesNameAndSql(ref exists))
            {
                var mc = new Regex(RegexSqlExpression, RegexOptions.IgnoreCase).Matches(dataSource.Value);
                foreach (Match m in mc)
                {
                    var param = new SqlExpression(
                        m.Value.XmlDecode(), m.Groups["col"].Value.XmlDecode(), m.Groups["param"].Value.XmlDecode());
                    if (!paramList.Contains(param)) paramList.Add(param);
                }
            }
            return paramList;
        }
        #endregion

        #region Setter methods

        internal void SetTopMargin(double value)
        {
            SetValue(RegexTopMargin, value);
        }

        internal void SetBottomMargin(double value)
        {
            SetValue(RegexBottomMargin, value);
        }

        internal void SetLeftMargin(double value)
        {
            SetValue(RegexLeftMargin, value);
        }

        internal void SetRightMargin(double value)
        {
            SetValue(RegexRightMargin, value);
        }

        internal void SetSpacing(double? value)
        {
            SetValue(RegexSpacing, value);
        }

        #endregion

        public class SqlExpression
        {
            public string Expression { get; private set; }
            public string ColumnName { get; private set; }
            public string ParameterName { get; private set; }

            public SqlExpression(string expression, string column, string parameter)
            {
                Expression = expression; 
                ColumnName = column;
                ParameterName = parameter;
            }
        }
    }
}
