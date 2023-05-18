using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


/// <summary>
/// Summary description for sqlinjec
/// </summary>
public class sqlInjection
{
    String[] mychar = new String[99];

    String[] mychar1 = new String[] { "!", "%", "^", "*", "(", ")", "~", "=", "?", ";", ";", "<", ">", "{", "}", "[", "]", "|", "'", "\"" };
    String[] mycharsk22 = new String[] { "!", "%", "*", "=", "<", ">", "'", "\"" };
    String[] mycharskcon = new String[] { "!", "%", "*", "<", ">", "'", "\"" };
    String[] sqlSyntax = new String[] { "update", "insert", "exec", "sp_", "delete"};
    String[] sqlSyntax_imp = new String[] { "drop","truncate","alter", "sleep", "table_name", "table_schema", "information_schema" };
    String[] sqlSyntax_Proc = new String[] { "update", "insert", "exec", "delete" };
    String[] sqlSyntax_Proc1 = new String[] { "update", "insert", "delete" };
    String[] ReportCharArray = new String[] { "@", "!", "$", "%", "^", "*", "(", ")", "~", ";", ":", ";", "<", ">", "{", "}", "[", "]", "/" };

    String[] AllCharArray = new String[] { "@", "!", "$", "%", "^", "*", "(", ")", "~", ";", ":", ";", "<", ">", "{", "}", "[", "]", "/", "\"", "'" };
    String[] ImpCharArray = new String[] { "^", "*",  ";", ";", "/", "\"", "'" };

    public sqlInjection()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public bool Check_PostMethod_Imp(object _Val, SqlDbType dbType = SqlDbType.VarChar)
    {
        bool retVal = false;
        string Val = Convert.ToString(_Val).ToLower();
        if (dbType == SqlDbType.Int)
        {
            retVal = Check_PostMethodInt(Val);
        }
        else //if (dbType == SqlDbType.VarChar)
        {
            retVal = Check_PostMethodStr(Val);
        }
        if (retVal)
        {
            Check_Imp(Val);
        }
        return retVal;
    }
    public bool Check_PostMethod(object _Val, SqlDbType dbType = SqlDbType.VarChar)
    {
        bool retVal = false;
        string Val = Convert.ToString(_Val).ToLower();
       
        if (dbType == SqlDbType.Int)
        {
            retVal = Check_PostMethodInt(Val);
        }
        else //if (dbType == SqlDbType.VarChar)
        {
            retVal = Check_PostMethodStr(Val);
        }
        if (retVal)
        {
            Check_Imp(Val);
        }
        return retVal;
    }
    public bool Check_PostMethodConn(object _Val, SqlDbType dbType = SqlDbType.VarChar)
    {
        bool retVal = false;
        string Val = Convert.ToString(_Val).ToLower();

        mychar = mycharskcon;
        retVal = Check_sql(Val);

        if (retVal)
        {
            Check_Imp(Val);
        }
        return retVal;
    }
    public bool Check_PostMethod_Proc(object _Val, int type, SqlDbType dbType = SqlDbType.VarChar)
    {
        bool retVal = false;
        string Val = Convert.ToString(_Val).ToLower();

        if (dbType == SqlDbType.Int)
        {
            retVal = Check_PostMethodInt(Val);
        }
        else //if (dbType == SqlDbType.VarChar)
        {
            retVal = Check_PostMethodStr(Val);
        }
        //if (retVal)
        //{
        //    if (type == 1)
        //        mychar = sqlSyntax_Proc;
        //    else if (type == 2)
        //        mychar = sqlSyntax_Proc1;
        //    for (int j = 0; j <= mychar.Length - 1; j++)
        //    {
        //        if (Val.IndexOf(mychar[j]) > -1)
        //            retVal = false;
        //    }
        //}
        if (retVal)
        {
            Check_Imp(Val);
        }
        return retVal;
    }


    bool Check_sql(string _Val)
    {
        for (int j = 0; j <= mychar.Length - 1; j++)
        {
            if (_Val.IndexOf(mychar[j]) > -1)
                return false;
        }
        return true;
    }
    bool Check_Imp(string _Val)
    {
        for (int j = 0; j <= sqlSyntax.Length - 1; j++)
        {
            if (_Val.IndexOf(sqlSyntax[j]) > -1)
                return false;
        }
        return true;
    }
    bool Check_PostMethodStr(string _Val)
    {
        mychar = ImpCharArray;
        for (int j = 0; j <= mychar.Length - 1; j++)
        {
            string Val = Convert.ToString(_Val);
            if (Val.IndexOf(mychar[j]) > -1)
                return false;
        }
        return true;
    }
    bool Check_PostMethodInt(string _Val)
    {
        mychar = AllCharArray;
        for (int j = 0; j <= mychar.Length - 1; j++)
        {
            string Val = Convert.ToString(_Val);
            if (Val.IndexOf(mychar[j]) > -1)
                return false;
        }
        return true;
    }
}