//#######################################################################
//Diversity Mobile Synchronization
//Project Homepage:  http://www.diversitymobile.net
//Copyright (C) 2011  Tobias Schneider, Lehrstuhl Angewndte Informatik IV, Universität Bayreuth
//
//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License along
//with this program; if not, write to the Free Software Foundation, Inc.,
//51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
//#######################################################################
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Data;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer;


namespace UBT.AI4.Bio.DivMobi.SyncBase.Util
{
    //public abstract class SyncDBManager
    //{
    //    public const String SQLSERVER = "SqlServer";
    //    public const String SQLCE = "SqlCe";

    //    protected IDictionary<String, String> _preferences;

    //    public static SyncDBManager createSyncDBManager(IDictionary<String, String> preferences)
    //    {
    //        String dbms = preferences["dbms"];

    //        if (dbms == SQLSERVER)
    //        {
    //            SqlServerManager manager = new SqlServerManager(preferences);
    //            return manager;
    //        }
    //        else if (dbms == SQLCE)
    //        {
    //            return new SqlCeManager(preferences);
    //        }
    //        else
    //        {
    //            throw new InvalidOperationException();
    //        }
    //    }

    //    internal IDictionary<String, String> Preferences { get { return _preferences; } set { _preferences = value; } }

    //    public abstract void CreateSyncRepository();

    //    public abstract Serializer CreateSyncSerializer();
    //}

    //internal class SqlServerManager : SyncDBManager
    //{
    //    private String _Username;
    //    private String _Password;
    //    private String _Datasource;
    //    private String _Catalog;
    //    private String _ConnectionString;

    //    private String _AccountUsername;
    //    private String _AccountPassword;
        


    //    public SqlServerManager(IDictionary<String, String> prefs)
    //    {
    //        Preferences = prefs;

    //        _Datasource = Preferences["datasource"];
    //        _Catalog = Preferences["catalog"];
    //        _AccountPassword = Preferences["accountpassword"];
    //        _AccountUsername = Preferences["accountusername"];

    //        try
    //        {
    //            _Username = Preferences["user"];
    //            _Password = Preferences["password"];

    //            StringBuilder tmp = new StringBuilder();

    //            tmp.Append("Data Source=").Append(_Datasource).Append(";");
    //            tmp.Append("Initial Catalog=").Append(_Catalog).Append(";");
    //            tmp.Append("Persist Security Info=True;");
    //            tmp.Append("User ID=").Append(_Username).Append(";");
    //            tmp.Append("Password=").Append(_Password).Append(";");

    //            _ConnectionString = tmp.ToString();
    //        }
    //        catch (KeyNotFoundException)
    //        {
    //            _ConnectionString = null;
    //        }
    //    }

    //    public override void CreateSyncRepository()
    //    {
    //        if (_ConnectionString == null) throw new InvalidOperationException();

    //        IDbConnection conn = new System.Data.SqlClient.SqlConnection(_ConnectionString);
    //        conn.Open();

    //        IDbTransaction t = conn.BeginTransaction();

    //        IDbCommand com = conn.CreateCommand();
    //        com.Transaction = t;

    //        StringBuilder createLogin = new StringBuilder();
    //        createLogin.Append("CREATE LOGIN ").Append(_AccountUsername).Append(" WITH PASSWORD='").Append(_AccountPassword).Append("'");
    //        com.CommandText = createLogin.ToString();
    //        com.ExecuteNonQuery();

    //        StringBuilder createUser = new StringBuilder();
    //        createUser.Append("CREATE USER ").Append(_AccountUsername).Append(" WITH DEFAULT_SCHEMA=").Append(_AccountUsername);
    //        com.CommandText = createUser.ToString();
    //        com.ExecuteNonQuery();

    //        StringBuilder createSchema = new StringBuilder();
    //        createSchema.Append("CREATE SCHEMA ").Append(_AccountUsername).Append(" AUTHORIZATION ").Append(_AccountUsername);
    //        com.CommandText = createSchema.ToString();
    //        com.ExecuteNonQuery();


    //        StringBuilder createSyncItem = new StringBuilder();
    //        createSyncItem.Append("CREATE TABLE ").Append(_AccountUsername).Append(".SyncItem").
    //            Append("(SyncID INT PRIMARY KEY IDENTITY, SyncFK INT, ClassID nvarchar(255), HashCode nvarchar(32), ").
    //            Append("SyncGuid UNIQUEIDENTIFIER, RowGuid UNIQUEIDENTIFIER)");
    //        com.CommandText = createSyncItem.ToString();
    //        com.ExecuteNonQuery();

    //        StringBuilder createFieldState = new StringBuilder();
    //        createFieldState.Append("CREATE TABLE ").Append(_AccountUsername).Append(".FieldState").
    //            Append("(SyncFK INT, FieldName nvarchar(255), HashCode nvarchar(32), ").
    //            Append("RowGuid UNIQUEIDENTIFIER, PRIMARY KEY (SyncFK, FieldName))");
    //        com.CommandText = createFieldState.ToString();
    //        com.ExecuteNonQuery();

    //        t.Commit();


    //        conn.Close();
    //        conn.Dispose();
    //    }

    //    public override Serializer CreateSyncSerializer()
    //    {
    //        Serializer s = new MS_SqlServerIPSerializer(_AccountUsername, _AccountPassword, _Datasource, _Catalog, String.Empty);
    //        s.RegisterType(typeof(SyncItem));
    //        s.RegisterType(typeof(FieldState));
    //        s.Activate();
    //        return s;
    //    }
    //}

    //internal class SqlCeManager : SyncDBManager
    //{
    //    private String _DbPath;

    //    public SqlCeManager(IDictionary<String, String> prefs)
    //    {
    //        Preferences = prefs;

    //        _DbPath = Preferences["dbpath"];
    //    }

    //    public override void CreateSyncRepository()
    //    {
    //        string connectionString = "Data Source=\"" + _DbPath + "\";Max Database Size=128;Default Lock Escalation=100;";
    //        IDbConnection conn = new SqlCeConnection(connectionString);
    //        conn.Open();

    //        IDbTransaction t = conn.BeginTransaction();

    //        IDbCommand com = conn.CreateCommand();
    //        com.Transaction = t;

    //        StringBuilder createSyncItem = new StringBuilder();
    //        createSyncItem.Append("CREATE TABLE SyncItem").
    //            Append("(SyncID INT PRIMARY KEY IDENTITY, SyncFK INT, ClassID nvarchar(255), HashCode nvarchar(32), ").
    //            Append("SyncGuid UNIQUEIDENTIFIER, RowGuid UNIQUEIDENTIFIER)");
    //        com.CommandText = createSyncItem.ToString();
    //        com.ExecuteNonQuery();

    //        StringBuilder createFieldState = new StringBuilder();
    //        createFieldState.Append("CREATE TABLE FieldState").
    //            Append("(SyncFK INT, FieldName nvarchar(255), HashCode nvarchar(32), ").
    //            Append("RowGuid UNIQUEIDENTIFIER, PRIMARY KEY (SyncFK, FieldName))");
    //        com.CommandText = createFieldState.ToString();
    //        com.ExecuteNonQuery();

    //        t.Commit();

    //        conn.Close();
    //        conn.Dispose();
    //    }

    //    public override Serializer CreateSyncSerializer()
    //    {
    //        Serializer s = new MS_SqlCeSerializer(_DbPath);
    //        s.RegisterType(typeof(SyncItem));
    //        s.RegisterType(typeof(FieldState));
    //        s.Activate();
    //        return s;
    //    }
    //}
}

