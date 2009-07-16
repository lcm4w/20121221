using System;
using System.IO;
using System.Data;
using System.Text;
using System.Web;

namespace TourWriter.Global
{
	public class Export
	{
		private static StreamWriter writer;

//		private static HttpResponse Response
//		{
//			get { return HttpContext.Current.Response; }
//		}

//		public static void DataSetToExcel(DataSet Data)
//		{
//			Response.Clear();
//			Response.ContentType = "application/ms-excel";
//			Response.AddHeader( "Content-Disposition",  "Filename=" + Data.DataSetName + ".xls" );
//
//			WriteWorkbookHeader();	
//		
//			foreach( DataTable table in Data.Tables )
//				WriteTable( table );
//
//			WriteWorkbookFooter();
//		}

		public static void DataSetToExcel(DataSet Data, string fileName)
		{
			writer = File.CreateText(fileName); 
		
			WriteWorkbookHeader();		
	
			foreach( DataTable table in Data.Tables )
				WriteTable( table );

			WriteWorkbookFooter();	
	
			writer.Close();
		}

		private static void WriteWorkbookHeader()
		{
			writer.Write( "<?xml version=\"1.0\"?>\r\n" );
			writer.Write( "<?mso-application progid=\"Excel.Sheet\"?>\r\n" );
			writer.Write( "<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" );
			writer.Write( "xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n" );
			writer.Write( "xmlns:x=\"urn:schemas-microsoft-com:office:excel\"\r\n" );
			writer.Write( "xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" );
			writer.Write( "xmlns:html=\"http://www.w3.org/TR/REC-html40\">\r\n" );
			writer.Write( "<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">\r\n" );
			writer.Write( "<LastAuthor>MSINC</LastAuthor>\r\n" );
			writer.Write( "  <Created>" + DateTime.Now.ToString() + "</Created>\r\n" );
			writer.Write( "  <Version>11.5703</Version>\r\n" );
			writer.Write( "</DocumentProperties>\r\n" );
			writer.Write( "<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">\r\n" );
			writer.Write( "  <ProtectStructure>False</ProtectStructure>\r\n" );
			writer.Write( "  <ProtectWindows>False</ProtectWindows>\r\n" );
			writer.Write( "</ExcelWorkbook>\r\n" );
			writer.Write( " <Styles>\r\n" );
			writer.Write( "  <Style ss:ID=\"s1\">\r\n" );
			writer.Write( "   <Font ss:Bold=\"1\"/>\r\n" );
			writer.Write( "  </Style>\r\n" );
			writer.Write( " </Styles>\r\n" );

//			Response.Write( "<?xml version=\"1.0\"?>\r\n" );
//			Response.Write( "<?mso-application progid=\"Excel.Sheet\"?>\r\n" );
//			Response.Write( "<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" );
//			Response.Write( "xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n" );
//			Response.Write( "xmlns:x=\"urn:schemas-microsoft-com:office:excel\"\r\n" );
//			Response.Write( "xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" );
//			Response.Write( "xmlns:html=\"http://www.w3.org/TR/REC-html40\">\r\n" );
//			Response.Write( "<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">\r\n" );
//			Response.Write( "<LastAuthor>MSINC</LastAuthor>\r\n" );
//			Response.Write( "  <Created>" + DateTime.Now.ToString() + "</Created>\r\n" );
//			Response.Write( "  <Version>11.5703</Version>\r\n" );
//			Response.Write( "</DocumentProperties>\r\n" );
//			Response.Write( "<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">\r\n" );
//			Response.Write( "  <ProtectStructure>False</ProtectStructure>\r\n" );
//			Response.Write( "  <ProtectWindows>False</ProtectWindows>\r\n" );
//			Response.Write( "</ExcelWorkbook>\r\n" );
//			Response.Write( " <Styles>\r\n" );
//			Response.Write( "  <Style ss:ID=\"s1\">\r\n" );
//			Response.Write( "   <Font ss:Bold=\"1\"/>\r\n" );
//			Response.Write( "  </Style>\r\n" );
//			Response.Write( " </Styles>\r\n" );
		}

		private static void WriteWorkbookFooter()
		{
			writer.Write( "</Workbook>\r\n" );
			//Response.Write( "</Workbook>\r\n" );
		}

		private static void WriteTableCaption( string tableName, int colSpan )
		{
			writer.Write( "<Row>\r\n" );
			writer.Write( "<Column colspan='" + colSpan + "'>" + tableName + "</Column>\r\n" );
			writer.Write( "</Row>\r\n" );

//			Response.Write( "<Row>\r\n" );
//			Response.Write( "<Column colspan='" + colSpan + "'>" + tableName + "</Column>\r\n" );
//			Response.Write( "</Row>\r\n" );
		}

		private static void WriteTableHeader( DataTable table )
		{
			foreach( DataColumn column in table.Columns )
				writer.Write( "<Column>" + column.ColumnName + "</Column>\r\n" );
				//Response.Write( "<Column>" + column.ColumnName + "</Column>\r\n" );

			writer.Write( "<Row>\r\n" );
			//Response.Write( "<Row>\r\n" );
		
			foreach( DataColumn column in table.Columns )
				writer.Write( "<Cell ss:StyleID=\"s1\"><Data ss:Type=\"String\">" + column.ColumnName + "</Data></Cell>\r\n" );
				//Response.Write( "<Cell ss:StyleID=\"s1\"><Data ss:Type=\"String\">" + column.ColumnName + "</Data></Cell>\r\n" );

			writer.Write( "</Row>\r\n" );
			//Response.Write( "</Row>\r\n" );

		}

		private static void WriteTable( DataTable table )
		{			
			writer.Write( "<Worksheet ss:Name='" + table.TableName + "'>\r\n" );
			writer.Write( "<Table ss:ExpandedColumnCount=\"" + table.Columns.Count + "\" ss:ExpandedRowCount=\"" + (table.Rows.Count + 1) + "\" x:FullColumns=\"1\" x:FullRows=\"1\">\r\n" );
			WriteTableHeader( table );
			WriteTableRows( table );
			writer.Write( "</Table>\r\n" );
			writer.Write( "</Worksheet>\r\n" );
				
//			Response.Write( "<Worksheet ss:Name='" + table.TableName + "'>\r\n" );
//			Response.Write( "<Table ss:ExpandedColumnCount=\"" + table.Columns.Count + "\" ss:ExpandedRowCount=\"" + (table.Rows.Count + 1) + "\" x:FullColumns=\"1\" x:FullRows=\"1\">\r\n" );
//			WriteTableHeader( table );
//			WriteTableRows( table );
//			Response.Write( "</Table>\r\n" );
//			Response.Write( "</Worksheet>\r\n" );
		}

		private static void WriteTableRows( DataTable table )
		{
			foreach( DataRow Row in table.Rows )
				WriteTableRow( Row );
		}

		private static bool IsNumber( string Value )
		{

			if( Value == "" )
				return false;

			char[] chars = Value.ToCharArray();

			foreach( char ch in chars )
			{
				if( ch != '$' && ch != '.' && ch != ',' && !char.IsNumber( ch ) )
					return false;
			}

			return true;

		}

		private static string GetExcelType( object Value )
		{

			if( Value == null || Value == DBNull.Value || Value is string )
				return "String";
				//			else if( Value is DateTime )
				//				return "Date";
			else if( IsNumber( Value.ToString() ) )
				return "Number";
			else
				return "String";

		}

		private static void WriteTableRow( DataRow Row )
		{
			writer.Write( "<Row>\r\n" );
			//Response.Write( "<Row>\r\n" );

			foreach( object loop in Row.ItemArray )
			{
				writer.Write( "<Cell><Data ss:Type=\"" + GetExcelType( loop ) + "\">" );
				//Response.Write( "<Cell><Data ss:Type=\"" + GetExcelType( loop ) + "\">" );

				if( loop != null && loop != DBNull.Value )
				{

					if( loop is byte[] )
					{
						writer.Write( "(...)" );
						//Response.Write( "(...)" );
					}
					else if( loop is decimal )
					{
						writer.Write( ((decimal) loop).ToString() );
						//Response.Write( ((decimal) loop).ToString() );
					}
					else if( loop is DateTime )
					{
						writer.Write( ((DateTime) loop).ToString( "yyyy-MM-dd HH:mm:ss" ) );
						//Response.Write( ((DateTime) loop).ToString( "yyyy-MM-dd HH:mm:ss" ) );
					}
					else
					{
						writer.Write( loop.ToString() );
						//Response.Write( loop.ToString() );
					}

				}
				writer.Write( "</Data></Cell>\r\n" );
				//Response.Write( "</Data></Cell>\r\n" );

			}
			writer.Write( "</Row>\r\n" );
			//Response.Write( "</Row>\r\n" );

		}

	}
}
