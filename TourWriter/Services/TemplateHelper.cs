using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace TourWriter.Services
{
    public class TemplateHelper
    {
        private List<string> orphanedTags;

        /// <summary>
        /// List of that did not have a corresponding source to obtain a value from.
        /// </summary>
        internal List<string> InvalidTags
        {
            get
            {
                if (orphanedTags == null)
                    orphanedTags = new List<string>();
                return orphanedTags;
            }
        }

        /// <summary>
        /// Tests if any tags were not valid.
        /// </summary>
        internal bool HasInvalidTags
        {
            get { return InvalidTags != null && InvalidTags.Count > 0; }
        }

        /// <summary>
        /// Replace the tags in a template with the matching column values.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        internal string Replace(string template, DataRow row)
        {
            return Replace(template, row, "");
        }

        /// <summary>
        /// Replace the tags in template with the matching column values, and use the specified format
        /// </summary>
        /// <param name="template"></param>
        /// <param name="row"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        internal string Replace(string template, DataRow row, string format)
        {
            var tags = GetTagNames(template);
            var keyValuePairs = GetKeyValuePairs(tags, row);

            return Replace(template, keyValuePairs,format);
        }
        /// <summary>
        /// Replace the tags in a template with the matching tag values.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="keyvals"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        internal static string Replace(string template, IEnumerable<KeyValuePair<string, string>> keyvals, string format)
        {
            foreach (var pair in keyvals)
            {
                var key = pair.Key;
                var value = pair.Value;
                template = Replace(template, key, value, format);
            }
            return template;
        }

        /// <summary>
        /// Get a list of tags and their values, from a data row where the tags are matched to the column names.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> GetKeyValuePairs(ICollection<Match> tags, DataRow row)
        {
            var keyvals = new List<KeyValuePair<string, string>>(tags.Count);

            foreach (var match in tags)
            {
                var tag = match.Value;

                if (row.Table.Columns.IndexOf(tag) > -1)
                {
                    var val = row[tag];

                    // format dates
                    if (row.Table.Columns[tag].DataType == typeof(System.DateTime))
                    {
                        val = ((System.DateTime)val).ToShortDateString();
                    }

                    keyvals.Add(new KeyValuePair<string, string>(tag, val.ToString()));
                }
                else
                    InvalidTags.Add(tag);
            }
            return keyvals;
        }

        /// <summary>
        /// Get a list of all tags in a template string.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        private static List<Match> GetTagNames(string template)
        {
            MatchCollection mc = Regex.Matches(template, @"(?<=\[!).*?(?=\])");
            var t = new Match[mc.Count];

            mc.CopyTo(t, 0);

            return new List<Match>(t);
        }

        /// <summary>
        /// Replace a tag with a value.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string Replace(string text, string tag, string value, string format)
        {
            value = value.Trim(). // clean
                Replace("\r\n", " ").Replace("\n", " ");  // and remove new-lines

            if (format == "csv")
                if (value.Contains(","))
                    value = string.Format("\"{0}\"", value); // escape embedded commas

            tag = "[!" + tag + "]";
            return text.Replace(tag, value);
        }
    }
}