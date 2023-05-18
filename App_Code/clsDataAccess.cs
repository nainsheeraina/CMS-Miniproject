using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.Script.Serialization;
using System.Globalization;

public class clsDataAccessMACET
{
    string Query = "";
    public string Message = "";
    public string exMessage = "";

    static string ConState = "";
    public static string ConnString = "";
    public static string ConnMessage = "";

    static string Opened = "Opened";
    static string Closed = "Closed";
    bool BeginTransactionStatus = false;

    static SqlConnection con;
    SqlCommand cmd;
    SqlDataReader dr;
    SqlTransaction tr;

    public GetData getData;
    public StoreProcedure storeProcedure;
    sqlInjection objsqlInjection;
    public clsDataAccessMACET()
    {
        getData = new GetData();
        storeProcedure = new StoreProcedure();
        objsqlInjection = new sqlInjection();
        ConState = "";
        Clear();
        // = new SqlConnectionStringBuilder(ConnectionString);

        if (ConnString.Trim().Length == 0)
            ConnString = System.Configuration.ConfigurationManager.ConnectionStrings["ConMACETDB"].ConnectionString;
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnString);
        if (objsqlInjection.Check_PostMethodConn(builder.ConnectionString))
        {
            con = new SqlConnection(builder.ConnectionString);
        }
        builder = null;
    }


    void Clear()
    {
        Message = "";
        exMessage = "";
        ConnMessage = "";
    }
    bool ConOpen()
    {
        Clear();
        try
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
            ConState = Opened;
            return true;
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        return false;
    }
    bool ConClose()
    {
        Clear();
        try
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
                Dispose();
            }
            ConState = Closed;
            return true;
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        return false;
    }
    void Dispose()
    {
        if (cmd != null)
        {
            cmd.Dispose();
            cmd = null;
        }
        if (dr != null)
        {
            dr.Dispose();
            dr = null;
        }
        if (tr != null)
        {
            tr.Dispose();
            tr = null;
        }
    }
    //---End-------------Connection------------------

    //---Start-------------Transaction------------------

    public bool Transaction_Begin()
    {
        Clear();
        BeginTransactionStatus = false;
        try
        {
            ConOpen();
            tr = con.BeginTransaction();
            BeginTransactionStatus = true;
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        return BeginTransactionStatus;
    }
    public bool Transaction_Commit()
    {
        Clear();
        BeginTransactionStatus = false;
        try
        {
            tr.Commit();
            ConClose();
            return true;
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        return false;
    }
    public bool Transaction_Rollback()
    {
        Clear();
        BeginTransactionStatus = false;
        try
        {
            tr.Rollback();
            return true;
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        finally
        {
            ConClose();
        }
        return false;
    }
    string FildWithParameter(string ConditionalField)
    {
        Clear();
        string RetVal = "";
        string[] CFN = ConditionalField.Split(',');

        for (int i = 0; i < CFN.Length; i++)
        {
            if (i == 0)
                RetVal = CFN[i].Trim() + "=" + "@" + CFN[i].Trim();
            else
                RetVal += "," + CFN[i].Trim() + "=" + "@" + CFN[i].Trim();
        }
        return RetVal;
    }
    string FildWithParameterWhere(string ConditionalField)
    {
        Clear();
        string RetVal = "";
        string[] CFN = ConditionalField.Split(',');

        for (int i = 0; i < CFN.Length; i++)
        {
            if (i == 0)
                RetVal = CFN[i].Trim() + "=" + "@" + CFN[i].Trim() + "_confprm";
            else
                RetVal += " and " + CFN[i].Trim() + "=" + "@" + CFN[i].Trim() + "_confprm";
        }
        return RetVal;
    }
    bool ExecuteCmd()
    {
        if (cmd != null)
        {
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            cmd.Dispose();
            cmd = null;
            if (!BeginTransactionStatus)
            {
                exMessage = ConState;
                ConClose();
            }
            return true;
        }
        return false;
    }
    //---End-------------Transaction------------------

    //---Start-------------Insert------------------
    public bool Insert(string TblName, string FieldName, object[] FieldValues)
    {
        Clear();
        try
        {
            if (objsqlInjection.Check_PostMethod(TblName))
            {
                string[] FN = FieldName.Split(',');
                if (FN.Length != FieldValues.Length)
                {
                    Message = "No. of field and values are not same.";
                }
                else
                {
                    string Str = "";
                    for (int i = 0; i < FN.Length; i++)
                    {
                        if (i > 0)
                            Str += ",";

                        Str += "@" + FN[i].Trim();
                        //if (i == 0)                    
                        //    Str += "@" + FN[i].Trim();                    
                        //else                    
                        //    Str += "," + "@" + FN[i].Trim();

                    }
                    Query = "Insert into " + TblName + " (" + FieldName + ") values(" + Str + ")";
                    if (objsqlInjection.Check_PostMethod_Proc(Query, 1))
                    {
                        FieldName = null;

                        if (BeginTransactionStatus == true)
                            cmd = new SqlCommand(Query, con, tr);
                        else
                        {
                            if (ConOpen())
                                cmd = new SqlCommand(Query, con);
                        }
                        if (cmd != null)
                        {
                            for (int i = 0; i < FieldValues.Length; i++)
                            {

                                cmd.Parameters.AddWithValue("@" + FN[i].Trim(), FieldValues[i]);
                            }
                        }
                        return ExecuteCmd();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        finally
        {
            ConClose();
        }

        return false;
    }
    //---End-------------Insert------------------

    //---Start-------------Update------------------
    public bool Update(string TblName, string FieldName, object[] FieldValues)
    {
        Clear();
        try
        {
            if (objsqlInjection.Check_PostMethod(TblName))
            {
                if (FieldName.Trim().Length > 0)
                {
                    string[] FN = FieldName.Split(',');
                    FieldName = "";
                    if (FN.Length != FieldValues.Length)
                    {
                        Message = "No. of field and values are not same.";
                    }
                    else
                    {
                        for (int i = 1; i < FN.Length; i++)
                        {
                            if (i == 1)
                                FieldName = FN[i].Trim() + "=" + "@" + FN[i].Trim();
                            else
                                FieldName += "," + FN[i].Trim() + "=" + "@" + FN[i].Trim();
                        }

                        if (FieldName.Trim().Length > 0)
                        {
                            Query = "Update " + TblName + " set " + FieldName + " where " + FN[0].Trim() + "=@" + FN[0].Trim() + "";
                            FieldName = null;
                            if (objsqlInjection.Check_PostMethod_Proc(Query, 1))
                            {
                                if (BeginTransactionStatus == true)
                                    cmd = new SqlCommand(Query, con, tr);
                                else
                                {
                                    ConOpen();
                                    cmd = new SqlCommand(Query, con);
                                }
                                if (cmd != null)
                                {
                                    for (int i = 0; i < FieldValues.Length; i++)
                                    {
                                        cmd.Parameters.AddWithValue("@" + FN[i].Trim() + "", FieldValues[i]);
                                    }
                                }
                                return ExecuteCmd();
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        finally
        {
            ConClose();
        }
        return false;
    }
    public bool Update(string TblName, string FieldName, object[] FieldValues, string Condition)
    {
        Clear();
        try
        {
            string[] FN = FieldName.Split(',');
            if ((FN.Length != FieldValues.Length) || FN.Length == 0)
            {
                Message = "No. of field and values are not same.";
            }
            else
            {
                if (objsqlInjection.Check_PostMethod(TblName) && objsqlInjection.Check_PostMethod(Condition))
                {
                    if (!Condition.Trim().ToLower().Contains("where"))
                        Condition = "where " + Condition;
                    Condition = Condition.Trim();
                    Query = ("Update " + TblName + " set " + FildWithParameter(FieldName) + " " + Condition).Trim();
                    FieldName = null;
                    if (objsqlInjection.Check_PostMethod_Proc(Query, 1))
                    {
                        if (BeginTransactionStatus == true)
                            cmd = new SqlCommand(Query, con, tr);
                        else
                        {
                            ConOpen();
                            cmd = new SqlCommand(Query, con);
                        }
                        if (cmd != null)
                        {
                            for (int i = 0; i < FieldValues.Length; i++)
                            {
                                cmd.Parameters.AddWithValue("@" + FN[i].Trim(), FieldValues[i]);
                            }
                        }
                        return ExecuteCmd();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        finally
        {
            ConClose();
        }
        return false;
    }
    public bool Update(string TblName, string FieldName, object[] FieldValues, string ConditionalField, object[] ConditionalValue)
    {
        Clear();
        try
        {
            if (objsqlInjection.Check_PostMethod(TblName))
            {
                string[] FN = FieldName.Split(',');
                string[] CFN = ConditionalField.Split(',');
                if ((FN.Length == FieldValues.Length) && (CFN.Length == ConditionalValue.Length))
                {
                    Query = "Update " + TblName + " set " + FildWithParameter(FieldName) + " where " + FildWithParameterWhere(ConditionalField);

                    if (objsqlInjection.Check_PostMethod_Proc(Query, 1))
                    {
                        FieldName = null;
                        ConditionalField = null;
                        if (BeginTransactionStatus == true)
                            cmd = new SqlCommand(Query, con, tr);
                        else
                        {
                            ConOpen();
                            cmd = new SqlCommand(Query, con);
                        }
                        if (cmd != null)
                        {
                            for (int i = 0; i < FieldValues.Length; i++)
                            {
                                cmd.Parameters.AddWithValue("@" + FN[i].Trim(), FieldValues[i]);
                            }
                            for (int i = 0; i < ConditionalValue.Length; i++)
                            {
                                cmd.Parameters.AddWithValue("@" + CFN[i].Trim() + "_confprm", ConditionalValue[i]);
                            }
                        }
                        return ExecuteCmd();
                    }
                }
                else
                {
                    Message = "No. of field and values are not same.";
                }
            }
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        return false;
    }
    //---End-------------Update------------------

    //---Start-------------Delete------------------
    public bool Delete(string TblName)
    {
        Clear();
        try
        {
            if (objsqlInjection.Check_PostMethod(TblName))
            {
                Query = "delete from " + TblName;
                if (objsqlInjection.Check_PostMethod_Proc(Query, 1))
                {
                    if (BeginTransactionStatus == true)
                        cmd = new SqlCommand(Query, con, tr);
                    else
                    {
                        ConOpen();
                        cmd = new SqlCommand(Query, con);
                    }
                    return ExecuteCmd();
                }
            }
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        finally
        {
            ConClose();
        }
        return false;
    }
    public bool Delete(string TblName, string Condition)
    {
        Clear();
        try
        {
            if (objsqlInjection.Check_PostMethod(TblName) && objsqlInjection.Check_PostMethod(Condition))
            {
                if (!Condition.Trim().ToLower().Contains("where"))
                    Condition = "where " + Condition;
                Condition = Condition.Trim();

                Query = ("delete from " + TblName.Trim() + " " + Condition.Trim()).Trim();

                if (objsqlInjection.Check_PostMethod_Proc(Query, 1))
                {
                    if (BeginTransactionStatus == true)
                        cmd = new SqlCommand(Query, con, tr);
                    else
                    {
                        ConOpen();
                        cmd = new SqlCommand(Query, con);
                    }
                    return ExecuteCmd();
                }
            }
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        finally
        {
            ConClose();
        }
        return false;
    }
    public bool Delete(string TblName, string ConditionalField, object[] ConditionalValue)
    {
        Clear();
        try
        {
            if (objsqlInjection.Check_PostMethod(TblName))
            {
                string[] CFN = ConditionalField.Split(',');
                if (CFN.Length == ConditionalValue.Length)
                {
                    Query = "delete from " + TblName + " where " + FildWithParameterWhere(ConditionalField);

                    if (objsqlInjection.Check_PostMethod_Proc(Query, 1))
                    {
                        ConditionalField = null;
                        for (int i = 0; i < ConditionalValue.Length; i++)
                            cmd.Parameters.AddWithValue("@" + CFN[i].Trim() + "_confprm", ConditionalValue[i]);

                        if (BeginTransactionStatus == true)
                            cmd = new SqlCommand(Query, con, tr);
                        else
                        {
                            ConOpen();
                            cmd = new SqlCommand(Query, con);
                        }
                        return ExecuteCmd();
                    }
                }
                else
                {
                    Message = "No. of field and values are not same.";
                }
            }
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        finally
        {
            ConClose();
        }
        return false;
    }
    //---End-------------Delete------------------
    public bool RunQuery(string query)
    {
        Clear();
        try
        {
            //if (objsqlInjection.Check_PostMethod(query))
            //{
            if (BeginTransactionStatus == true)
                cmd = new SqlCommand(query, con, tr);
            else
            {
                ConOpen();
                cmd = new SqlCommand(query, con);
            }
            return ExecuteCmd();
        }
        // }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        finally
        {
            ConClose();
        }
        return false;
    }

    //---Start-------------CreateId------------------
    public Int64 CreateId(string ColumnName, string TableName)
    {
        Int64 RetVal = 0;
        try
        {
            if (objsqlInjection.Check_PostMethod(ColumnName) && objsqlInjection.Check_PostMethod(TableName))
            {
                if (BeginTransactionStatus)
                {
                    Message = "Transaction is Begin. First Stop Transaction, then Retray....";
                    return RetVal;
                }
                Query = "select max(" + ColumnName + ") from " + TableName;
                if (objsqlInjection.Check_PostMethod_Proc(Query, 1))
                {
                    ConOpen();
                    cmd = new SqlCommand(Query, con);
                    dr = cmd.ExecuteReader();
                    if (dr.HasRows == true)
                    {
                        dr.Read();
                        RetVal = Convert.ToInt64(dr[0].ToString());
                        dr.Close();
                    }
                    RetVal++;
                }
            }
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        finally
        {
            ConClose();
        }

        return RetVal;
    }
    public Int64 CreateId(string ColumnName, string TableName, string Condition)
    {
        Int64 RetVal = 0;
        try
        {
            if (objsqlInjection.Check_PostMethod(ColumnName) && objsqlInjection.Check_PostMethod(TableName) && objsqlInjection.Check_PostMethod(Condition))
            {
                if (BeginTransactionStatus)
                {
                    Message = "Transaction is Begin. First Stop Transaction, then Retray....";
                    return RetVal;
                }

                if (!Condition.Trim().ToLower().Contains("where"))
                    Condition = "where " + Condition;
                Condition = Condition.Trim();

                Query = ("select max(" + ColumnName + ") from " + TableName + " " + Condition).Trim();
                if (objsqlInjection.Check_PostMethod_Proc(Query, 1))
                {
                    ConOpen();
                    cmd = new SqlCommand(Query, con);
                    dr = cmd.ExecuteReader();
                    if (dr.HasRows == true)
                    {
                        dr.Read();
                        RetVal = Convert.ToInt64(dr[0].ToString());
                        dr.Close();
                    }
                    RetVal++;
                }
            }
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        finally
        {
            ConClose();
        }
        return RetVal;
    }

    //---End-------------CreateId------------------
    public bool CheckDublicate(string TableName, string Condition)
    {
        bool Flag = false;
        try
        {
            if (objsqlInjection.Check_PostMethod(TableName) && objsqlInjection.Check_PostMethod(Condition))
            {
                if (BeginTransactionStatus)
                {
                    Message = "Transaction is Begin. First Stop Transaction, then Retray....";
                    return false;
                }
                try
                {
                    if (!Condition.Trim().ToLower().Contains("where"))
                        Condition = "where " + Condition;

                    Query = ("select top 1 * from " + TableName + " " + Condition).Trim();
                    if (objsqlInjection.Check_PostMethod_Proc(Query, 1))
                    {
                        ConOpen();
                        cmd = new SqlCommand(Query, con);
                        dr = cmd.ExecuteReader();
                        if (dr.HasRows == true)
                        {
                            Flag = true;
                        }
                    }
                }
                catch
                {
                }
            }
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        finally
        {
            ConClose();
        }
        return Flag;
    }
    public bool CheckDublicate(string TableName, string ConditionalField, object[] ConditionalValue)
    {
        bool Flag = false;
        try
        {
            if (objsqlInjection.Check_PostMethod(TableName))
            {
                if (BeginTransactionStatus)
                {
                    Message = "Transaction is Begin. First Stop Transaction, then Retray....";
                    return Flag;
                }
                string[] CFN = ConditionalField.Split(',');
                if (CFN.Length == ConditionalValue.Length)
                {
                    Query = "select top 1 * from " + TableName + " where " + FildWithParameterWhere(ConditionalField);

                    if (objsqlInjection.Check_PostMethod_Proc(Query, 1))
                    {
                        ConditionalField = null;
                        ConOpen();
                        cmd = new SqlCommand(Query, con);
                        for (int i = 0; i < ConditionalValue.Length; i++)
                        {
                            cmd.Parameters.AddWithValue("@" + CFN[i].Trim() + "_confprm", ConditionalValue[i]);
                        }
                        dr = cmd.ExecuteReader();
                        if (dr.HasRows == true)
                            Flag = true;
                        Dispose();
                    }
                }
                else
                {
                    Message = "No. of field and values are not same.";
                }
            }
        }
        catch (Exception ex)
        {
            exMessage = ex.Message;
        }
        finally
        {
            ConClose();
        }
        return Flag;
    }

    public class GetData
    {
        string Query = "";
        public string Message = "";
        public string exMessage = "";
        SqlCommand cmd;
        SqlDataAdapter da;
        SqlDataReader dr;
        SqlTransaction tr;
        DataTable dt;
        DataSet ds;
        sqlInjection objsqlInjection;
        public GetData()
        {
            Clear();
            Dispose();
            objsqlInjection = new sqlInjection();
        }
        void Clear()
        {
            Query = "";
            Message = "";
            exMessage = "";
        }
        void Dispose(bool IsDisposeDatatable = true)
        {
            if (cmd != null)
            {
                cmd.Dispose();
                cmd = null;
            }
            if (da != null)
            {
                da.Dispose();
                da = null;
            }
            if (dr != null)
            {
                dr.Dispose();
                dr = null;
            }
            if (IsDisposeDatatable && dt != null)
            {
                if (dt != null)
                {
                    dt.Dispose();
                    dt = null;
                }
                if (ds != null)
                {
                    ds.Dispose();
                    ds = null;
                }
            }
        }
        string FildWithParameterWhere(string ConditionalField)
        {
            Clear();
            string RetVal = "";
            string[] CFN = ConditionalField.Split(',');

            for (int i = 0; i < CFN.Length; i++)
            {
                if (i == 0)
                    RetVal = CFN[i].Trim() + "=" + "@" + CFN[i].Trim();
                else
                    RetVal += " and " + CFN[i].Trim() + "=" + "@" + CFN[i].Trim();
            }
            return RetVal;
        }
        public string Count(string TableName, string Condition = "")
        {
            Clear();
            string RetVal = "";
            if (objsqlInjection.Check_PostMethod(TableName) && objsqlInjection.Check_PostMethod(Condition))
            {
                if (!Condition.Trim().ToLower().Contains("where"))
                    Condition = "where " + Condition;
                Condition = Condition.Trim();

                dt = new DataTable();
                Query = ("select count(*) from " + TableName + " " + Condition).Trim();

                if (objsqlInjection.Check_PostMethod(Query))
                {
                    cmd = new SqlCommand(Query, con);
                    cmd.Prepare();
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    if (dt != null && dt.Rows.Count > 0)
                        RetVal = dt.Rows[0][0].ToString();
                    Dispose();
                }
            }
            return RetVal;
        }
        public DataTable RunQuery(string Query)
        {
            Clear();
            dt = new DataTable();
            //if (objsqlInjection.Check_PostMethod(Query))
            // {
            da = new SqlDataAdapter(Query, con);
            if (ConnectionState.Open == con.State || ConnectionState.Broken == con.State)
            {
                con.Close();
            }
            da.Fill(dt);
            Dispose(false);
            // }

            return dt;
        }

        public string AsScalar(string ColumnName, string TableName, string Condition = "")
        {
            Clear();
            string Data = "";

            if (objsqlInjection.Check_PostMethod(ColumnName) && objsqlInjection.Check_PostMethod(TableName) && objsqlInjection.Check_PostMethod(Condition))
            {
                dt = AsMultiScalar(ColumnName, TableName, Condition);
                if (dt != null && dt.Rows.Count > 0)
                    Data = dt.Rows[0][0].ToString();
                Dispose();
            }
            return Data;
        }
        public string AsScalar(string ColumnName, string TableName, string ConditionalField, object[] ConditionalValue)
        {
            Clear();
            string Data = "";
            dt = AsMultiScalar(ColumnName, TableName, ConditionalField, ConditionalValue);
            if (dt != null && dt.Rows.Count > 0)
                Data = dt.Rows[0][0].ToString();
            Dispose();
            return Data;
        }

        public DataTable AsMultiScalar(string ColumnName, string TableName)
        {
            Clear();
            dt = new DataTable();
            if (objsqlInjection.Check_PostMethod(ColumnName) && objsqlInjection.Check_PostMethod(TableName))
            {
                Query = ("select " + ColumnName + " from " + TableName).Trim();
                da = new SqlDataAdapter(Query, con);
                if (ConnectionState.Open == con.State || ConnectionState.Broken == con.State)
                {
                    con.Close();
                }
                da.Fill(dt);
                Dispose(false);
            }

            return dt;
        }
        public DataTable AsMultiScalar(string ColumnName, string TableName, string Condition)
        {
            Clear();
            dt = new DataTable();
            if (objsqlInjection.Check_PostMethod(ColumnName) && objsqlInjection.Check_PostMethod(TableName) && objsqlInjection.Check_PostMethod(Condition))
            {
                if (Condition.Trim().Length > 0)
                {
                    if (!Condition.Trim().ToLower().Contains("where"))
                        Condition = "where " + Condition;

                    Condition = Condition.Trim();
                }
                Query = ("select " + ColumnName + " from " + TableName + " " + Condition).Trim();
                da = new SqlDataAdapter(Query, con);
                if (ConnectionState.Open == con.State || ConnectionState.Broken == con.State)
                {
                    con.Close();
                }
                da.Fill(dt);
                Dispose(false);
            }

            return dt;
        }
        public DataTable AsMultiScalar(string ColumnName, string TableName, string Condition, string OrderBy)
        {
            Clear();
            dt = new DataTable();
            if (objsqlInjection.Check_PostMethod(ColumnName) && objsqlInjection.Check_PostMethod(TableName) && objsqlInjection.Check_PostMethod(Condition) && objsqlInjection.Check_PostMethod(OrderBy))
            {
                if (Condition.Trim().Length > 0)
                {
                    if (!Condition.Trim().ToLower().Contains("where"))
                        Condition = "where " + Condition;

                    Condition = Condition.Trim();
                }
                if (OrderBy.Trim().Length > 0)
                {
                    if (!OrderBy.Trim().ToLower().Contains("order by"))
                        OrderBy = "order by " + OrderBy;

                    OrderBy = OrderBy.Trim();
                }
                Query = ("select " + ColumnName + " from " + TableName + " " + Condition + " " + OrderBy).Trim();
                da = new SqlDataAdapter(Query, con);
                if (ConnectionState.Open == con.State || ConnectionState.Broken == con.State)
                {
                    con.Close();
                }
                da.Fill(dt);
                Dispose(false);
            }
            return dt;
        }
        public DataTable AsMultiScalar(string ColumnName, string TableName, string ConditionalField, object[] ConditionalValue, string OrderBy = "")
        {
            Clear();
            dt = new DataTable();
            if (objsqlInjection.Check_PostMethod(ColumnName) && objsqlInjection.Check_PostMethod(TableName) && objsqlInjection.Check_PostMethod(OrderBy))
            {
                string[] CFN = ConditionalField.Split(',');
                if (CFN.Length != ConditionalValue.Length)
                {
                    Message = "No. of field and values are not same.";
                }
                else
                {
                    if (OrderBy.Trim().Length > 0)
                        OrderBy = " order by " + OrderBy;

                    dt = new DataTable();
                    cmd = new SqlCommand("select " + ColumnName + " from " + TableName + " where " + FildWithParameterWhere(ConditionalField) + OrderBy, con);
                    for (int i = 0; i < ConditionalValue.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@" + CFN[i].Trim(), ConditionalValue[i]);
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    Dispose(false);
                }
            }
            return dt;
        }

        public DataTable AsDataTable(string TableName)
        {
            Clear();
            dt = new DataTable();
            if (objsqlInjection.Check_PostMethod(TableName))
            {
                Query = ("select * from " + TableName).Trim();
                da = new SqlDataAdapter(Query, con);
                if (ConnectionState.Open == con.State || ConnectionState.Broken == con.State)
                {
                    con.Close();
                }
                da.Fill(dt);
                Dispose(false);
            }

            return dt;
        }
        public DataTable AsDataTable(string TableName, string Condition)
        {
            Clear();
            dt = new DataTable();
            if (objsqlInjection.Check_PostMethod(TableName) && objsqlInjection.Check_PostMethod(Condition))
            {
                if (Condition.Trim().Length > 0)
                {
                    if (!Condition.Trim().ToLower().Contains("where"))
                        Condition = "where " + Condition;

                    Condition = Condition.Trim();
                }
                Query = ("select * from " + TableName + " " + Condition).Trim();
                da = new SqlDataAdapter(Query, con);
                if (ConnectionState.Open == con.State || ConnectionState.Broken == con.State)
                {
                    con.Close();
                }
                da.Fill(dt);
                Dispose(false);
            }

            return dt;
        }
        public DataTable AsDataTable(string TableName, string Condition, string OrderBy)
        {
            Clear();
            dt = new DataTable();
            if (objsqlInjection.Check_PostMethod(TableName) && objsqlInjection.Check_PostMethod(Condition) && objsqlInjection.Check_PostMethod(OrderBy))
            {
                if (Condition.Trim().Length > 0)
                {
                    if (!Condition.Trim().ToLower().Contains("where"))
                        Condition = "where " + Condition;

                    Condition = Condition.Trim();
                }
                if (OrderBy.Trim().Length > 0)
                {
                    if (!OrderBy.Trim().ToLower().Contains("order by"))
                        OrderBy = "order by " + OrderBy;

                    OrderBy = OrderBy.Trim();
                }
                Query = ("select * from " + TableName + " " + Condition + " " + OrderBy).Trim();
                da = new SqlDataAdapter(Query, con);
                if (ConnectionState.Open == con.State || ConnectionState.Broken == con.State)
                {
                    con.Close();
                }
                da.Fill(dt);
                Dispose(false);
            }
            return dt;
        }
        public DataTable AsDataTable(string TableName, string ConditionalField, object[] ConditionalValue, string OrderBy = "")
        {
            Clear();
            dt = new DataTable();
            if (objsqlInjection.Check_PostMethod(TableName) && objsqlInjection.Check_PostMethod(OrderBy))
            {
                string[] CFN = ConditionalField.Split(',');
                if (CFN.Length != ConditionalValue.Length)
                {
                    Message = "No. of field and values are not same.";
                }
                else
                {
                    if (OrderBy.Trim().Length > 0)
                        OrderBy = " order by " + OrderBy;
                    dt = new DataTable();
                    cmd = new SqlCommand("select * from " + TableName + " where " + FildWithParameterWhere(ConditionalField) + OrderBy, con);
                    for (int i = 0; i < ConditionalValue.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@" + CFN[i].Trim(), ConditionalValue[i]);
                    }
                    da = new SqlDataAdapter(cmd);
                    if (ConnectionState.Open == con.State || ConnectionState.Broken == con.State)
                    {
                        con.Close();
                    }
                    da.Fill(dt);

                    Dispose(false);
                }
            }
            return dt;
        }

        public DataSet AsDataSet(string Query)
        {
            Clear();
            ds = new DataSet();
            if (objsqlInjection.Check_PostMethod(Query))
            {
                da = new SqlDataAdapter(Query, con);
                da.Fill(ds);
                Dispose(false);
            }
            return ds;
        }
    }
    public class StoreProcedure
    {
        SqlCommand cmd;
        SqlDataAdapter da;
        SqlDataReader dr;
        SqlTransaction tr;
        DataTable dt;
        DataSet ds;
        sqlInjection objsqlInjection;

        string Query = "";
        public string Message = "";
        public string exMessage = "";
        bool UseParameter = false;
        public string FieldName;
        public string[] ArrFieldName;
        public object[] FieldValue;

        public StoreProcedure()
        {
            UseParameter = false;
            objsqlInjection = new sqlInjection();
            Clear();
            ClearField();
            Dispose();
        }
        void Clear()
        {
            Query = "";
            Message = "";
            exMessage = "";
        }
        void Dispose(bool IsDisposeDatatable = true)
        {
            if (cmd != null)
            {
                cmd.Dispose();
                cmd = null;
            }
            if (da != null)
            {
                da.Dispose();
                da = null;
            }
            if (dr != null)
            {
                dr.Dispose();
                dr = null;
            }
            if (IsDisposeDatatable && dt != null)
            {
                if (dt != null)
                {
                    dt.Dispose();
                    dt = null;
                }
                if (ds != null)
                {
                    ds.Dispose();
                    ds = null;
                }
            }
            UseParameter = false;
            ClearField();
        }
        void ClearField()
        {
            FieldName = "";
            FieldValue = null;
            ArrFieldName = null;
            FieldValue = null;
        }
        bool ConOpen()
        {
            Clear();
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                ConState = Opened;
                return true;
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            return false;
        }
        bool ConClose()
        {
            Clear();
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                ConState = Closed;
                return true;
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            return false;
        }
        void ParameterAsArray()
        {
            Clear();
            if (FieldName.Trim().Length > 0)
                ArrFieldName = FieldName.Split(',');
            if (ArrFieldName != null && ArrFieldName != null)
            {
                if (ArrFieldName.Length > 0 && ArrFieldName.Length > 0)
                {
                    if (ArrFieldName.Length != FieldValue.Length)
                    {
                        Message = "No. of field and values are not same";
                    }
                    else
                    {
                        for (int i = 0; i < ArrFieldName.Length; i++)
                        {
                            if (ArrFieldName[0].ToString() == "@")
                                cmd.Parameters.Add(ArrFieldName[i], SqlDbType.VarChar).Value = FieldValue[i];
                            else
                            {
                                cmd.Parameters.Add("@" + ArrFieldName[i], SqlDbType.VarChar).Value = FieldValue[i];
                            }
                        }
                    }
                }
                else
                {
                }
            }
        }
        public void NewStoreProcedure(string ProcName)
        {
            if (ProcName.Length > 0 && objsqlInjection.Check_PostMethod_Proc(ProcName, 1))
            {
                cmd = new SqlCommand(ProcName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Prepare();
            }
            else
            {
                Message = "Invalid procedure name.";
            }
        }

        public void AddWithValue(string fieldName, object fieldValue)
        {
            if (cmd == null)
            {
                cmd = new SqlCommand("", con);
            }
            if (fieldName[0].ToString() == "@")
                cmd.Parameters.Add(fieldName, SqlDbType.VarChar).Value = fieldValue;
            else
            {
                cmd.Parameters.Add("@" + fieldName, SqlDbType.VarChar).Value = fieldValue;
            }
            UseParameter = true;
        }
        public void AddWithValue(string fieldName, object fieldValue, SqlDbType DbType)
        {
            if (cmd == null)
            {
                cmd = new SqlCommand("", con);
                cmd.CommandType = CommandType.StoredProcedure;
            }
            if (fieldName[0].ToString() == "@")
                cmd.Parameters.Add(fieldName, SqlDbType.VarChar).Value = fieldValue;
            else
            {
                cmd.Parameters.Add("@" + fieldName, DbType).Value = fieldValue;
            }
            UseParameter = true;
        }

        public bool ExecuteNonQuery()
        {
            Clear();
            bool flag = false;
            try
            {
                if (UseParameter == false)
                    ParameterAsArray();

                ConOpen();
                cmd.ExecuteNonQuery();
                ConClose();
                flag = true;
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            Dispose();
            return flag;
        }
        public bool ExecuteNonQuery(string ProcName)
        {
            Clear();
            bool retVal = false;
            try
            {
                if (ProcName.Length > 0 && objsqlInjection.Check_PostMethod_Proc(ProcName, 1))
                {
                    if (cmd == null)
                        NewStoreProcedure(ProcName);
                    else
                    {
                        cmd.CommandText = ProcName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Prepare();
                    }
                    if (UseParameter == false)
                        ParameterAsArray();

                    ConOpen();
                    cmd.ExecuteNonQuery();
                    retVal = true;
                    ConClose();
                }
                else
                {
                    Message = "Invalid procedure name.";
                }
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            Dispose();
            return retVal;
        }
        public bool ExecuteNonQuery(string ProcName, object[] fieldValue)
        {
            Clear();
            bool retVal = false;
            try
            {
                if (ProcName.Length > 0 && objsqlInjection.Check_PostMethod_Proc(ProcName, 1))
                {
                    string Query = "Exec " + ProcName;

                    for (int i = 0; i < fieldValue.Length; i++)
                    {
                        if (i == 0)
                            Query += " '" + fieldValue[i] + "'";
                        else
                            Query += ",'" + fieldValue[i] + "'";
                    }
                    if (objsqlInjection.Check_PostMethod_Proc(Query, 2))
                    {
                        ConOpen();
                        cmd = new SqlCommand(Query.Trim(), con);
                        cmd.ExecuteNonQuery();
                        retVal = true;
                    }
                }
                else
                {
                    Message = "Invalid procedure name.";
                }
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            Dispose(false);
            return retVal;
        }
        public bool ExecuteNonQuery(string ProcName, string fieldName, object[] fieldValue)
        {
            Clear();
            bool retVal = false;
            try
            {
                if (ProcName.Length > 0 && objsqlInjection.Check_PostMethod_Proc(ProcName, 1))
                {
                    string[] FName = fieldName.Split(',');
                    if (FName.Length == fieldValue.Length)
                    {
                        NewStoreProcedure(ProcName);
                        for (int i = 0; i < fieldValue.Length; i++)
                        {
                            AddWithValue(Convert.ToString(FName[i]), fieldValue[i]);
                        }

                        retVal = ExecuteNonQuery();
                        retVal = true;
                    }
                    else
                    {
                        Message = "Field name and value are not matched.";
                    }
                }
                else
                {
                    Message = "Invalid procedure name.";
                }
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            Dispose(false);
            return retVal;
        }

        public DataTable getData()
        {
            Clear();
            dt = new DataTable();
            try
            {
                if (UseParameter == false)
                    ParameterAsArray();
                da = new SqlDataAdapter(cmd);
                if (ConnectionState.Open == con.State || ConnectionState.Broken == con.State)
                {
                    con.Close();
                }
                da.Fill(dt);

            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            Dispose(false);
            return dt;
        }
        public DataTable getData(string ProcName)
        {
            Clear();
            dt = new DataTable();
            try
            {
                if (ProcName.Length > 0 && objsqlInjection.Check_PostMethod_Proc(ProcName, 1))
                {
                    NewStoreProcedure(ProcName);
                    da = new SqlDataAdapter(cmd);
                    if (ConnectionState.Open == con.State || ConnectionState.Broken == con.State)
                    {
                        con.Close();
                    }
                    da.Fill(dt);
                }
                else
                {
                    Message = "Invalid procedure name.";
                }
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            Dispose(false);
            return dt;
        }
        public DataTable getData(string ProcName, object fieldValue)
        {
            Clear();
            dt = new DataTable();
            try
            {
                if (ProcName.Length > 0 && objsqlInjection.Check_PostMethod_Proc(ProcName, 1))
                {
                    string Query = "Exec " + ProcName + " '" + fieldValue + "'";
                    if (objsqlInjection.Check_PostMethod_Proc(Query, 2))
                    {
                        cmd = new SqlCommand(Query, con);
                        cmd.Prepare();
                        da = new SqlDataAdapter(cmd);
                        if (ConnectionState.Open == con.State || ConnectionState.Broken == con.State)
                        {
                            con.Close();
                        }
                        da.Fill(dt);
                    }
                }
                else
                {
                    Message = "Procedure name is not in Parameter.";
                }
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            Dispose(false);
            return dt;
        }
        public DataTable getData(string ProcName, object[] fieldValue)
        {
            Clear();
            dt = new DataTable();
            try
            {
                if (ProcName.Length > 0 && objsqlInjection.Check_PostMethod_Proc(ProcName, 1))
                {
                    string Query = "Exec " + ProcName;
                    for (int i = 0; i < fieldValue.Length; i++)
                    {
                        if (i == 0)
                            Query += " '" + fieldValue[i] + "'";
                        else
                            Query += ",'" + fieldValue[i] + "'";
                    }
                    if (objsqlInjection.Check_PostMethod_Proc(Query, 2))
                    {
                        cmd = new SqlCommand(Query, con);
                        cmd.Prepare();
                        da = new SqlDataAdapter(cmd);
                        if (ConnectionState.Open == con.State || ConnectionState.Broken == con.State)
                        {
                            con.Close();
                        }
                        da.Fill(dt);
                    }
                }
                else
                {
                    Message = "Invalid procedure name.";
                }
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            Dispose(false);
            return dt;
        }
        public DataTable getData(string ProcName, string fieldName, object[] fieldValue)
        {
            Clear();
            dt = new DataTable();
            try
            {
                if (ProcName.Length > 0 && objsqlInjection.Check_PostMethod_Proc(ProcName, 1))
                {
                    string[] FName = fieldName.Split(',');
                    if (FName.Length == fieldValue.Length)
                    {
                        NewStoreProcedure(ProcName);
                        for (int i = 0; i < FName.Length; i++)
                        {
                            AddWithValue(Convert.ToString(FName[i]), fieldValue[i]);
                        }
                        if (UseParameter == false)
                            ParameterAsArray();

                        dt = getData();
                    }
                    else
                    {
                        Message = "Field name and value are not matched.";
                    }
                }
                else
                {
                    Message = "Invalid procedure name.";
                }
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            Dispose(false);
            return dt;
        }

        public DataSet getDataSet()
        {
            Clear();
            ds = new DataSet();
            try
            {
                if (UseParameter == false)
                    ParameterAsArray();
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            Dispose(false);
            return ds;
        }
        public DataSet getDataSet(string ProcName)
        {
            Clear();
            ds = new DataSet();
            try
            {
                if (ProcName.Length > 0 && objsqlInjection.Check_PostMethod_Proc(ProcName, 1))
                {
                    NewStoreProcedure(ProcName);
                    da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }
                else
                {
                    Message = "Invalid procedure name.";
                }
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            Dispose(false);
            return ds;
        }
        public DataSet getDataSet(string ProcName, object fieldValue)
        {
            Clear();
            ds = new DataSet();
            try
            {
                if (ProcName.Length > 0 && objsqlInjection.Check_PostMethod_Proc(ProcName, 1))
                {
                    string Query = "Exec " + ProcName + " '" + fieldValue + "'";
                    if (objsqlInjection.Check_PostMethod_Proc(Query, 2))
                    {
                        da = new SqlDataAdapter(Query.Trim(), con);
                        da.Fill(ds);
                    }
                }
                else
                {
                    Message = "Procedure name is not in Parameter.";
                }
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            Dispose(false);
            return ds;
        }
        public DataSet getDataSet(string ProcName, object[] fieldValue)
        {
            Clear();
            ds = new DataSet();
            try
            {
                if (ProcName.Length > 0 && objsqlInjection.Check_PostMethod_Proc(ProcName, 1))
                {
                    string Query = "Exec " + ProcName;

                    for (int i = 0; i < fieldValue.Length; i++)
                    {
                        if (i == 0)
                            Query += " '" + fieldValue[i] + "'";
                        else
                            Query += ",'" + fieldValue[i] + "'";
                    }
                    if (objsqlInjection.Check_PostMethod_Proc(Query, 2))
                    {
                        da = new SqlDataAdapter(Query.Trim(), con);
                        da.Fill(ds);
                    }
                }
                else
                {
                    Message = "Invalid procedure name.";
                }
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            Dispose(false);
            return ds;
        }
        public DataSet getDataSet(string ProcName, string fieldName, object[] fieldValue)
        {
            Clear();
            ds = new DataSet();
            try
            {
                if (ProcName.Length > 0 && objsqlInjection.Check_PostMethod_Proc(ProcName, 1))
                {
                    string[] FName = fieldName.Split(',');
                    if (FName.Length == fieldValue.Length)
                    {
                        NewStoreProcedure(ProcName);
                        for (int i = 0; i < FName.Length; i++)
                        {
                            AddWithValue(Convert.ToString(FName[i]), fieldValue[i]);
                        }
                        if (UseParameter == false)
                            ParameterAsArray();

                        ds = getDataSet();
                    }
                    else
                    {
                        Message = "Field name and value are not matched.";
                    }
                }
                else
                {
                    Message = "Invalid procedure name.";
                }
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            Dispose(false);
            return ds;
        }
    }

    public DataTable GetDataTable(string query)
    {
        DataTable dt = new DataTable();
        try
        {
            con.Open();
            //con.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = query;
            SqlDataAdapter adap1 = new SqlDataAdapter();
            cmd.Connection = con;
            adap1.SelectCommand = cmd;
            adap1.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            //return ex.Message.ToString();
            return dt;
        }

        finally
        {
            con.Close();
        }

    }


    public int ExecuteSql(string Query)
    {


        try
        {
            con.Open();
            SqlCommand cmd = new SqlCommand();

            string strCommand = Query;
            cmd.CommandText = strCommand;
            cmd.Connection = con;
            return cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            return 0;

        }

        finally
        {
            con.Close();
        }

    }

    public string ExecuteScalar(string strSql)
    {
        SqlCommand cmd = new SqlCommand();
        try
        {

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strSql;
            cmd.Connection = con;
            cmd.Connection.Open();
            object obj = cmd.ExecuteScalar();

            if (obj == null || obj == DBNull.Value)
                return "";
            else
                return obj.ToString();

        }
        catch (Exception ex)
        {
            return "";
        }
        finally
        {
            cmd.Connection.Close();

        }
    }

    public void Alert()
    {
        string Message = "AlertClick()";

        if (HttpContext.Current.CurrentHandler is Page)
        {
            Page page = (Page)HttpContext.Current.CurrentHandler;

            if (ScriptManager.GetCurrent(page) != null)
            {
                ScriptManager.RegisterStartupScript(page, typeof(Page), "Message", Message, true);
            }
            else
            {
                page.ClientScript.RegisterStartupScript(typeof(Page), "Message", Message, true);
            }
        }
    }


    public bool DeleteFile(string FolderPath)
    {

        Boolean FileSaved = false;

        try
        {
            string ServerPath = HttpContext.Current.Server.MapPath(FolderPath);


            FileInfo file = new FileInfo(ServerPath);
            if (file.Exists)//check file exsit or not  
            {
                file.Delete();
                FileSaved = true;
            }

        }
        catch (Exception ex)
        {
            FileSaved = false;
        }
        return FileSaved;
    }



    public string UploadFile(FileUpload fileName, string FolderPath)
    {
        string itime = DateTime.UtcNow.AddHours(5).AddMinutes(30).AddMilliseconds(10).ToString("HHmmssff");
        string ImagePath = "";
        string rename = "";
        Boolean FileOK = false;
        Boolean FileSaved = false;
        int k = 0;
        if (fileName.HasFile)
        {
            if (fileName.FileBytes.Length < 5000000)
            {
                string FileExtension = Path.GetExtension(fileName.FileName.ToLower());
                rename = itime + FileExtension;
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".PNG", ".JPG", ".JPEG" };
                for (int i = 0; i < allowedExtensions.Length; i++)
                {
                    k++;
                    if (FileExtension == allowedExtensions[i])
                    {
                        FileOK = true;
                        break;
                    }
                    else
                    {
                    }
                }
            }

            else
            {
                return "File Size Large";
            }

            if (FileOK)
            {
                try
                {
                    string ServerPath = HttpContext.Current.Server.MapPath("~/" + FolderPath);
                    if (!Directory.Exists(ServerPath))
                    {
                        Directory.CreateDirectory(ServerPath);
                    }


                    fileName.SaveAs(ServerPath + rename);
                    FileSaved = true;
                }
                catch (Exception ex)
                {
                    FileSaved = false;
                }
            }
            else
            {
            }
            if (FileSaved)
            {
                ImagePath = FolderPath + Path.GetFileName(rename);
            }
        }
        return ImagePath;
    }

    public string UploadPDF(FileUpload fileName, string FolderPath)
    {
        string itime = DateTime.UtcNow.AddHours(5).AddMinutes(30).AddMilliseconds(10).ToString("ddMMyyyyHHmmssff");
        string ImagePath = "";
        string rename = "";
        Boolean FileOK = false;
        Boolean FileSaved = false;
        int k = 0;
        if (fileName.HasFile)
        {
            if (fileName.FileBytes.Length < 5000000)
            {
                string FileExtension = Path.GetExtension(fileName.FileName.ToLower());
                rename = itime + FileExtension;
                string[] allowedExtensions = { ".pdf", ".PDF" };
                for (int i = 0; i < allowedExtensions.Length; i++)
                {
                    k++;
                    if (FileExtension == allowedExtensions[i])
                    {
                        FileOK = true;
                        break;
                    }
                    else
                    {
                        ImagePath = "Please Select Pdf File Only";
                    }
                }
            }

            else
            {

            }

            if (FileOK)
            {
                try
                {
                    string ServerPath = HttpContext.Current.Server.MapPath("~/" + FolderPath);
                    if (!Directory.Exists(ServerPath))
                    {
                        Directory.CreateDirectory(ServerPath);
                    }
                    fileName.SaveAs(ServerPath + rename);
                    FileSaved = true;
                }
                catch (Exception ex)
                {
                    FileSaved = false;
                }
            }
            else
            {
            }
            if (FileSaved)
            {
                ImagePath = FolderPath + Path.GetFileName(rename);
            }
        }
        return ImagePath;
    }

    public string UploadVideo(FileUpload fileName, string FolderPath)
    {
        string itime = DateTime.UtcNow.ToString("HHmmssff");
        string ImagePath = "";
        string rename = "";
        Boolean FileOK = false;
        Boolean FileSaved = false;
        int k = 0;
        if (fileName.HasFile)
        {
            if (fileName.FileBytes.Length < 56017962)
            {
                string FileExtension = Path.GetExtension(fileName.FileName.ToLower());
                rename = itime + FileExtension;
                string[] allowedExtensions = { ".flv", ".avi", ".mov", ".mp4", ".mpg", ".wmv" };
                for (int i = 0; i < allowedExtensions.Length; i++)
                {
                    k++;
                    if (FileExtension == allowedExtensions[i])
                    {
                        FileOK = true;
                        break;
                    }
                    else
                    {
                    }
                }
            }

            else
            {
                return "";
            }

            if (FileOK)
            {
                try
                {
                    string ServerPath = HttpContext.Current.Server.MapPath("../" + FolderPath);
                    fileName.SaveAs(ServerPath + rename);
                    FileSaved = true;
                }
                catch (Exception ex)
                {
                    FileSaved = false;
                }
            }
            else
            {
            }
            if (FileSaved)
            {
                ImagePath = FolderPath + Path.GetFileName(rename);
            }
        }
        return ImagePath;
    }

    public string ConvertDateFormat(string CurrentDate, string CurrentFormat, string NewFormat) // yyyy-MM-dd  To dd/MM/yyyy
    {
        DateTime dtDate = DateTime.ParseExact(CurrentDate, CurrentFormat, CultureInfo.InvariantCulture);
        return dtDate.ToString(NewFormat);
    }
    public string time()
    {
        return DateTime.UtcNow.ToString("HH:mm:ss tt");
    }
    public string date()
    {
        return DateTime.UtcNow.ToString("dd/MM/yyyy");
    }

    public string iDate()
    {
        return DateTime.UtcNow.ToString("yyyyMMdd");
    }
    public string SqlStringFor_iDate(string FieldName)
    {
        return "convert(int, convert(varchar, convert(datetime, " + FieldName + ", 103), 112))";
    }

    public string GenerateRandomNumber(int start, int end)
    {  //10000000  99999999
        Random random = new Random();
        int temp = random.Next(start, end);
        return temp.ToString();
    }

    public string UniqueUserID()  //(int start, int end)
    {
        int start = 10000000; int end = 99999999;  // Upto 8 digit.
        string id = GenerateRandomNumber(start, end);
        bool ExistStatus = true;
        while (ExistStatus)
        {
            id = GenerateRandomNumber(start, end);
        }

        return id.ToString();
    }

    public void PopulateDropDownSql(DropDownList DropDown, string Sql)
    {
        try
        {
            DataTable DatatableName = GetDataTable(Sql);
            if (DatatableName.Rows.Count > 0)
            {
                DropDown.DataSource = DatatableName;
                DropDown.DataBind();
                if (DropDown.Items[0].Text != "NA")
                {
                    DropDown.Items.Insert(0, new ListItem("Select", "0"));
                }
            }
            else
            {
                DropDown.Items.Insert(0, new ListItem("No Data", "0"));
                DropDown.DataSource = null;
                DropDown.DataBind();
            }
        }
        catch (Exception ex)
        {
        }
    }
    public void PopulateDropDown(DropDownList DropDown, string Sql, string text, string value)
    {
        try
        {
            DataTable DatatableName = GetDataTable(Sql);
            if (DatatableName.Rows.Count > 0)
            {
                DropDown.DataSource = DatatableName;
                DropDown.DataBind();
                DropDown.DataTextField = text;
                DropDown.DataValueField = value;
                DropDown.DataBind();
                if (DropDown.Items[0].Text != "NA")
                {
                    DropDown.Items.Insert(0, new ListItem("Select", "0"));
                }
            }
            else
            {
                DropDown.Items.Insert(0, new ListItem("No Data", "0"));
                DropDown.DataSource = null;
                DropDown.DataBind();
            }
        }
        catch (Exception ex)
        {
        }
    }

    //public string SearchAutoCompete(string keyword)
    //{
    //    string sql = "select id, productName from Product_details  where productName LIKE '%" + keyword + "%'";

    //    DataTable dt = GetDataTable(sql);
    //    if (dt.Rows.Count != 0)
    //    {
    //        List<SearchWomen> list = new List<SearchWomen>();
    //        foreach (DataRow dr in dt.Rows)
    //        {
    //            SearchWomen s = new SearchWomen();
    //            s.Name = dr["productName"].ToString();
    //            s.ID = dr["id"].ToString();
    //            list.Add(s);
    //        }

    //        JavaScriptSerializer js = new JavaScriptSerializer();
    //        keyword = js.Serialize(list);
    //    }
    //    return keyword;
    //}
}

