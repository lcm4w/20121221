// Thanks to: Jouni Heikniemi, http://www.heikniemi.net/jhlib/
using System.IO;
using System.Text;

namespace TourWriter.Utilities.Strings
{
    /// <summary>
    /// A tool class for writing Csv and other char-separated field files.
    /// 
    /// EXAMPLE USAGE
    /// string tempFileName = Environment.GetEnvironmentVariable("TEMP") + "\\" + Guid.NewGuid().ToString();
    /// using (CsvWriter cw = new CsvWriter(tempFileName, Encoding.Unicode, '-', false)) {
    ///     cw.WriteFields("foo", "bar", "foo-bar");
    ///     cw.WriteFields("\"foo\"-bar");
    ///     cw.Flush();
    /// }
    /// </summary>
    public class CsvWriter : StreamWriter
    {
        #region Private variables

        private char separator;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Csv writer for the given filename (overwriting existing contents).
        /// </summary>
        /// <param name="filename">The name of the file being written to.</param>
        public CsvWriter(string filename)
            : this(filename, ',', false) { }

        /// <summary>
        /// Creates a new Csv writer for the given filename.
        /// </summary>
        /// <param name="filename">The name of the file being written to.</param>
        /// <param name="append">True if the contents shall be appended to the
        /// end of the possibly existing file.</param>
        public CsvWriter(string filename, bool append)
            : this(filename, ',', append) { }

        /// <summary>
        /// Creates a new Csv writer for the given filename and encoding.
        /// </summary>
        /// <param name="filename">The name of the file being written to.</param>
        /// <param name="enc">The encoding used.</param>
        /// <param name="append">True if the contents shall be appended to the
        /// end of the possibly existing file.</param>
        public CsvWriter(string filename, Encoding enc, bool append)
            : this(filename, enc, ',', append) { }

        /// <summary>
        /// Creates a new writer for the given filename and separator.
        /// </summary>
        /// <param name="filename">The name of the file being written to.</param>
        /// <param name="separator">The field separator character used.</param>
        /// <param name="append">True if the contents shall be appended to the
        /// end of the possibly existing file.</param>
        public CsvWriter(string filename, char separator, bool append)
            : base(filename, append) { this.separator = separator; }

        /// <summary>
        /// Creates a new writer for the given filename, separator and encoding.
        /// </summary>
        /// <param name="filename">The name of the file being written to.</param>
        /// <param name="enc">The encoding used.</param>
        /// <param name="separator">The field separator character used.</param>
        /// <param name="append">True if the contents shall be appended to the
        /// end of the possibly existing file.</param>
        public CsvWriter(string filename, Encoding enc, char separator, bool append)
            : base(filename, append, enc) { this.separator = separator; }

        /// <summary>
        /// Creates a new Csv writer for the given stream.
        /// </summary>
        /// <param name="s">The stream to write the CSV to.</param>
        public CsvWriter(Stream s)
            : this(s, ',') { }

        /// <summary>
        /// Creates a new writer for the given stream and separator character.
        /// </summary>
        /// <param name="s">The stream to write the CSV to.</param>
        /// <param name="separator">The field separator character used.</param>
        public CsvWriter(Stream s, char separator)
            : base(s) { this.separator = separator; }

        /// <summary>
        /// Creates a new writer for the given stream, separator and encoding.
        /// </summary>
        /// <param name="s">The stream to write the CSV to.</param>
        /// <param name="enc">The encoding used.</param>
        /// <param name="separator">The field separator character used.</param>
        public CsvWriter(Stream s, Encoding enc, char separator)
            : base(s, enc) { this.separator = separator; }

        #endregion

        #region Properties

        /// <summary>
        /// The separator character for the fields. Comma for normal CSV.
        /// </summary>
        public char Separator
        {
            get { return separator; }
            set { separator = value; }
        }

        #endregion

        public void WriteFields(params object[] content)
        {
            for (int i = 0; i < content.Length; ++i)
            {
                string s;
                s = (content[i] != null ? content[i].ToString() : "");
                if (s.IndexOfAny(new char[] { Separator, '"' }) >= 0)
                    // We have to quote the string
                    s = "\"" + s.Replace("\"", "\"\"") + "\"";
                Write(s);

                // Write the separator unless we're at the last position
                if (i < content.Length - 1)
                    Write(separator);
            }
            Write(NewLine);
        }
    }
}
