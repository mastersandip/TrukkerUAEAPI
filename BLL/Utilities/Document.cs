using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Configuration;
using BLL.Utilities;
using System.Data.SqlClient;

namespace BLL.Utilities
{
    public class Document : ServerBase
    {
        #region Constructor & Destructor.
        public Document()
        {
        }
        ~Document()
        {
            if (DBConnection != null)
            {
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
            }
        }
        #endregion

        private const int DOCUMENTNOPORTIONLENGTH = 6;
        #region GetNextDocNo

        public Boolean W_GetNextDocumentNo(ref IDbCommand comm, String DocumentType, String UserID, String HostName, ref String NextDocumentNo, ref String Message)
        {

            comm.CommandText = " SELECT next_doc_no FROM next_doc_no WITH (NOLOCK) WHERE " +
                                " doc_type = @document_type";

            comm.CommandTimeout = 120;
            comm.Parameters.Clear();

            IDataParameter para2 = DBObjectFactory.GetParameterObject();
            para2.ParameterName = "@document_type";
            para2.DbType = DbType.String;
            para2.Value = DocumentType;
            comm.Parameters.Add(para2);

            Object objRetVal = null;
            Decimal doc_no;
            //MessageBox.Show("Before getting the next document no");
            objRetVal = comm.ExecuteScalar();
            if (objRetVal != null)
                doc_no = Convert.ToDecimal(objRetVal);
            else
                doc_no = 0;

            if (doc_no <= 0)
            {
                Message = "An error occured while getting next document number for Document Type " + DocumentType + ".";
                return false;
            }

            //comm.CommandText = " UPDATE next_doc_no SET next_doc_no = next_doc_no + 1, " +
            //                    " last_modified_by = @UserId, last_modified_date = @AsOn," +
            //                    " last_modified_host = @HostName WHERE " +
            //                    " next_doc_no = @sa_id AND " +
            //                    " doc_type = @document_type AND next_doc_no = @doc_no";

            comm.CommandText = " UPDATE next_doc_no SET next_doc_no = next_doc_no + 1, " +
                             " last_modified_by = @UserId, last_modified_date = @AsOn," +
                             " last_modified_host = @HostName WHERE " +
                             " doc_type = @document_type AND next_doc_no = @doc_no";

            IDataParameter para3 = DBObjectFactory.GetParameterObject();
            para3.ParameterName = "@doc_no";
            para3.DbType = DbType.Decimal;
            para3.Value = doc_no;
            comm.Parameters.Add(para3);

            IDataParameter para4 = DBObjectFactory.GetParameterObject();
            para4.ParameterName = "@UserId";
            para4.DbType = DbType.String;
            para4.Value = UserID;
            comm.Parameters.Add(para4);

            IDataParameter para5 = DBObjectFactory.GetParameterObject();
            para5.ParameterName = "@AsOn";
            para5.DbType = DbType.DateTime;
            para5.Value = DateTime.UtcNow;
            comm.Parameters.Add(para5);

            IDataParameter para6 = DBObjectFactory.GetParameterObject();
            para6.ParameterName = "@HostName";
            para6.DbType = DbType.String;
            para6.Value = HostName;
            comm.Parameters.Add(para6);

            //MessageBox.Show("Before updating the document no");
            if (comm.ExecuteNonQuery() <= 0)
            {
                Message = "An error occured while Updating next document number for Document Type " + DocumentType + ".";
                return false;
            }

            NextDocumentNo = FormatDocumentNo(DocumentType, doc_no);
            //MessageBox.Show("Document no is " + NextDocumentNo);
            return true;
        }

        private String FormatDocumentNo(String DocumentType, Decimal doc_no)
        {
            //temporarily we have applied following logic to generate ipd,opd and private services numbers
            //in future it will be customezed and for that we need one table for this parameters.

            //right now it will work as follow.
            //e.g. for ipd it will be I050600001
            //here I indicates this number is for Ipd
            //next 4 digits 0506 indicates year duration
            //and last 6 digits indicates sr.no.            
            String FormattedNo = "";
            //Document Type 1 char
            FormattedNo = DocumentType.Substring(0, 1);
            //Current Year 2 chars

            String year = System.DateTime.Today.Year.ToString();
            FormattedNo += year.Substring(2);
            year = System.DateTime.Today.AddYears(1).Year.ToString().Substring(2);
            //next year 2 chars
            FormattedNo += year;

            int LengthOfCurrentDocNo = doc_no.ToString().Length;
            for (int i = 1; i <= DOCUMENTNOPORTIONLENGTH - LengthOfCurrentDocNo; i++)
                FormattedNo += "0";
            FormattedNo += doc_no.ToString();

            return FormattedNo;
        }

        public Boolean W_GetNextDocumentNoNew(ref IDbCommand comm, String DocumentType, String UserID, String HostName, ref String NextDocumentNo, ref String Message)
        {

            comm.CommandText = " SELECT next_doc_no FROM next_doc_no WITH (NOLOCK) WHERE " +
                                " doc_type = @document_type";

            comm.CommandTimeout = 120;
            comm.Parameters.Clear();

            IDataParameter para2 = DBObjectFactory.GetParameterObject();
            para2.ParameterName = "@document_type";
            para2.DbType = DbType.String;
            para2.Value = DocumentType;
            comm.Parameters.Add(para2);

            Object objRetVal = null;
            Decimal doc_no;
            //MessageBox.Show("Before getting the next document no");
            objRetVal = comm.ExecuteScalar();
            if (objRetVal != null)
                doc_no = Convert.ToDecimal(objRetVal);
            else
                doc_no = 0;

            if (doc_no <= 0)
            {
                Message = "An error occured while getting next document number for Document Type " + DocumentType + ".";
                return false;
            }

            //comm.CommandText = " UPDATE next_doc_no SET next_doc_no = next_doc_no + 1, " +
            //                    " last_modified_by = @UserId, last_modified_date = @AsOn," +
            //                    " last_modified_host = @HostName WHERE " +
            //                    " next_doc_no = @sa_id AND " +
            //                    " doc_type = @document_type AND next_doc_no = @doc_no";

            comm.CommandText = " UPDATE next_doc_no SET next_doc_no = next_doc_no + 1, " +
                             " last_modified_by = @UserId, last_modified_date = @AsOn," +
                             " last_modified_host = @HostName WHERE " +
                             " doc_type = @document_type AND next_doc_no = @doc_no";

            IDataParameter para3 = DBObjectFactory.GetParameterObject();
            para3.ParameterName = "@doc_no";
            para3.DbType = DbType.Decimal;
            para3.Value = doc_no;
            comm.Parameters.Add(para3);

            IDataParameter para4 = DBObjectFactory.GetParameterObject();
            para4.ParameterName = "@UserId";
            para4.DbType = DbType.String;
            para4.Value = UserID;
            comm.Parameters.Add(para4);

            IDataParameter para5 = DBObjectFactory.GetParameterObject();
            para5.ParameterName = "@AsOn";
            para5.DbType = DbType.DateTime;
            para5.Value = DateTime.UtcNow;
            comm.Parameters.Add(para5);

            IDataParameter para6 = DBObjectFactory.GetParameterObject();
            para6.ParameterName = "@HostName";
            para6.DbType = DbType.String;
            para6.Value = HostName;
            comm.Parameters.Add(para6);

            //MessageBox.Show("Before updating the document no");
            if (comm.ExecuteNonQuery() <= 0)
            {
                Message = "An error occured while Updating next document number for Document Type " + DocumentType + ".";
                return false;
            }

            NextDocumentNo = FormatDocumentNoNew(DocumentType, doc_no);
            //MessageBox.Show("Document no is " + NextDocumentNo);
            return true;
        }

        private String FormatDocumentNoNew(String DocumentType, Decimal doc_no)
        {
            //temporarily we have applied following logic to generate ipd,opd and private services numbers
            //in future it will be customezed and for that we need one table for this parameters.

            //right now it will work as follow.
            //e.g. for ipd it will be I050600001
            //here I indicates this number is for Ipd
            //next 4 digits 0506 indicates year duration
            //and last 6 digits indicates sr.no.            
            String FormattedNo = "";
            //Document Type 1 char
            FormattedNo = DocumentType.Substring(0, 2);
            //Current Year 2 chars

            String year = System.DateTime.Today.Year.ToString();
            FormattedNo += year.Substring(2);
            year = System.DateTime.Today.AddYears(1).Year.ToString().Substring(2);
            //next year 2 chars
            FormattedNo += year;

            int LengthOfCurrentDocNo = doc_no.ToString().Length;
            for (int i = 1; i <= DOCUMENTNOPORTIONLENGTH - LengthOfCurrentDocNo; i++)
                FormattedNo += "0";
            FormattedNo += doc_no.ToString();

            return FormattedNo;
        }

        #endregion


    }
}
