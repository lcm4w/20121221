using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TourWriter.Services;

namespace TourWriter.UserControls.Reports
{
    public class RdlcFileHelper
    {
        #region Regex patterns
        private const string regexSql = @"(?<=<CommandText>).*(?=</CommandText>)";
        private const string regexTopMargin = @"(?<=<TopMargin>)\d*\.?\d+(?=cm</TopMargin>)";
        private const string regexBottomMargin = @"(?<=<BottomMargin>)\d*\.?\d+(?=cm</BottomMargin>)";
        private const string regexLeftMargin = @"(?<=<LeftMargin>)\d*\.?\d+(?=cm</LeftMargin>)";
        private const string regexRightMargin = @"(?<=<RightMargin>)\d*\.?\d+(?=cm</RightMargin>)";
        private const string regexSpacing = "(?<=<Textbox Name=\"SpacerCell\">.*<Height>)\\d*\\.?\\d+(?=cm</Height>)";
        private const string regexSqlExpression = @"(where|and|or) *\(? *(?<col>\w*) (([a-zA-Z=<>!]* ){1,3})(?<param>@\w*) *\)?";
        private const string regexLogo = "(?<=<Image Name=\"logo\">.*?<Value>).*?(?=</Value>.*?</Image>)";
        #endregion

        private readonly string rdlcFile;
        private string rdlc;

        public RdlcFileHelper(string rdlcFile)
        {
            this.rdlcFile = rdlcFile;
            ReadFile(rdlcFile);
        }

        private void ReadFile(string file)
        {
            using (TextReader reader = new StreamReader(file))
                rdlc = reader.ReadToEnd();
        }

        internal void WriteFile()
        {
            using (TextWriter writer = new StreamWriter(rdlcFile))
                writer.Write(rdlc);
        }

        private object GetValue(string pattern, ref bool exists)
        {
            return GetValue(pattern, ref exists, RegexOptions.Multiline);
        }

        private object GetValue(string pattern, ref bool exists, RegexOptions regexOption)
        {
            var match = Regex.Match(rdlc, pattern, regexOption);
            exists = match.Success;
            return match.Value.XmlDecode();
        }

        private void SetValue(string pattern, object value)
        {
            rdlc = new Regex(pattern, RegexOptions.Singleline).Replace(rdlc, value.ToString());
        }

        #region Getter methods

        internal double? GetTopMargin(ref bool exists)
        {
            double d;
            var val = GetValue(regexTopMargin, ref exists);
            return double.TryParse(val.ToString(), out d) ? (double?)d : null;
        }

        internal double? GetBottomMargin(ref bool exists)
        {
            double d;
            var val = GetValue(regexBottomMargin, ref exists);
            return double.TryParse(val.ToString(), out d) ? (double?)d : null;
        }

        internal double? GetLeftMargin(ref bool exists)
        {
            double d;
            var val = GetValue(regexLeftMargin, ref exists);
            return double.TryParse(val.ToString(), out d) ? (double?)d : null;
        }

        internal double? GetRightMargin(ref bool exists)
        {
            double d;
            var val = GetValue(regexRightMargin, ref exists, RegexOptions.Singleline);
            return double.TryParse(val.ToString(), out d) ? (double?)d : null;
        }

        internal double? GetSpacing(ref bool exists)
        {
            double d;
            var val = GetValue(regexSpacing, ref exists, RegexOptions.Singleline);
            return double.TryParse(val.ToString(), out d) ? (double?)d : null;
        }

        internal string GetLogo(ref bool exists)
        {
            return GetValue(regexLogo, ref exists, RegexOptions.Singleline).ToString();
        }

        internal string GetSql(ref bool exists)
        {
            return GetValue(regexSql, ref exists).ToString();
        }

        internal List<SqlExpression> GetSqlExpressions()
        {
            bool exists = false;
            var mc = new Regex(regexSqlExpression, RegexOptions.IgnoreCase).Matches(GetSql(ref exists));

            var result = new List<SqlExpression>(mc.Count);
            foreach (Match m in mc)
                result.Add(new SqlExpression(m.Value, m.Groups["col"].Value, m.Groups["param"].Value));
            return result;
        }
        #endregion

        #region Setter methods

        internal void SetTopMargin(double value)
        {
            SetValue(regexTopMargin, value);
        }

        internal void SetBottomMargin(double value)
        {
            SetValue(regexBottomMargin, value);
        }

        internal void SetLeftMargin(double value)
        {
            SetValue(regexLeftMargin, value);
        }

        internal void SetRightMargin(double value)
        {
            SetValue(regexRightMargin, value);
        }

        internal void SetSpacing(double? value)
        {
            SetValue(regexSpacing, value);
        }
        
        internal void SetLogo(string value)
        {
            const string s = "file:///";
            value = s + value.TrimStart(s.ToCharArray());
            SetValue(regexLogo, value);
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
