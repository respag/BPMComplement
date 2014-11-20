using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ultimus.AuditManager.Admin.Commons;
using Ultimus.ComponentManager.Models;
using System.Data;
using System.Data.Entity;
using System.Net.Http.Formatting;
using System.Diagnostics;
using System.Web;
using System.Configuration;

namespace Ultimus.AuditManager.Admin.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuditConfigurationController : ApiController
    {
        ComponentManagerContext db = Utilities.GetComponentManagerContextForApi();

        CacheManager objCache = new CacheManager();

        public IEnumerable<CatProcesses> GetAllProcess()
        {

            IEnumerable<CatProcesses> _CatProcesses = (from p in db.CatProcesses
                                                       orderby p.ProcessName
                                                       select p).ToList();
            return _CatProcesses;
        }

        public IEnumerable<CatSteps> GetStepByProcessId(int id)
        {

            IEnumerable<CatSteps> CatStepsList = (from d in db.CatSteps
                                                  where d.IdProcess == id
                                                  orderby d.StepName
                                                  select d).ToList();
            return CatStepsList;
        }

        public GetConnectionsResponse GetConnections(int id)
        {
            GetConnectionsResponse response = new GetConnectionsResponse();
            CatProcesses process = db.CatProcesses.Find(id);
            process.AuditConnectionProcess = db.AuditConnectionProcess.Find(process.IdProcess);

            if (process.AuditConnectionProcess != null)
            {
                response.SeletedOriginId = process.AuditConnectionProcess.IdConnectionOrigin;
                response.SeletedDestinationId = process.AuditConnectionProcess.IdConnectionDestination;
                response.haveConnectionConfig = true;
            }
            else
                response.haveConnectionConfig = false;

            response.CatConnectionsList = db.CatConnections.ToList();

            return response;
        }

        #region Table Definition Methods

        public IEnumerable<TableInformation> GetTablesByConnectionId(int id, int id2)
        {

            #region Local Parameters

            CatConnections catConnections;
            IEnumerable<AuditTablesVsSteps> AuditTablesVsStepsList;
            AuditTablesVsSteps auditTablesVsSteps;
            ICollection<TableInformation> TableInformationList = new HashSet<TableInformation>();
            System.Data.SqlClient.SqlDataReader SqlDR;
            DataBaseAccess da = new DataBaseAccess();

            #endregion

            try
            {
                catConnections = db.CatConnections.Find(id);

                #region Get all tables configurated for step

                AuditTablesVsStepsList = (from d in db.AuditTablesVsSteps
                                          where d.IdStep == id2
                                          select d).Include("AuditTablesDefinition").ToList();

                #endregion

                #region Query for get al tables in DB

              //  string[] listaSchemasQuitados = ConfigurationManager.AppSettings["ListaSchemasQuitados"].Split(',').Select(s => s.Trim()).ToArray();
                var @listaSchemasQuitados = ConfigurationManager.AppSettings["ListaSchemasQuitados"];
                string query = "SELECT '['+SCHEMA_NAME(schema_id)+'].['+name+']'AS SchemaTable FROM sys.tables " +
                               "WHERE (SCHEMA_NAME(schema_id)  NOT IN" +
                               "((SELECT * FROM dbo.Split('" + @listaSchemasQuitados + "',',')))) " +
                               "ORDER BY '['+SCHEMA_NAME(schema_id)+'].['+name+']', name";
                    

                #endregion

                da = new DataBaseAccess(catConnections.ConnectionStringDecrypted, query);

                SqlDR = da.SqlCom.ExecuteReader();

                while (SqlDR.Read())
                {

                    TableInformation info = new TableInformation();
                    info.TableName = SqlDR.GetString(0);

                    #region Validate if the table exist in configuration with particular step

                    auditTablesVsSteps = (from d in AuditTablesVsStepsList
                                          where d.AuditTablesDefinition.TableName == info.TableName
                                          select d).FirstOrDefault();

                    if (auditTablesVsSteps == null)
                    {
                        TableInformationList.Add(info);
                    }

                    #endregion
                }

                return TableInformationList;
            }
            catch (Exception ex)
            {
                Debug.Write("[Ultimus.ComponentManager] GetTablesByConnectionId ERROR " + ex.ToString());
                return null;
            }
            finally
            {
                da.CloseConnection();
            }
        }

        public string SaveTablesToAudit([FromBody] SaveTableConfigRequest resquest)
        {

            try
            {

                #region Validate Existing table with same configuration of Connections

                IEnumerable<CatProcesses> Processes;

                Processes = db.CatProcesses.Include("AuditConnectionProcess")
                    .Where(p => p.AuditConnectionProcess.IdConnectionOrigin == resquest.SeletedOriginId
                        && p.AuditConnectionProcess.IdConnectionDestination == resquest.SeletedDestinationId)
                        .Include("CatSteps").Include("CatSteps.AuditTablesVsSteps")
                        .Include("CatSteps.AuditTablesVsSteps.AuditTablesDefinition").ToList();

                #endregion


                foreach (var item in resquest.TablesToAudit)
                {
                    #region Add the Table and save changes for get the ID

                    AuditTablesDefinition auditTablesDefinition = new AuditTablesDefinition(); ;
                    Boolean TableExistInConfiguration = false;

                    foreach (var ProcessesItem in Processes)
                    {
                        foreach (var StepsItem in ProcessesItem.CatSteps)
                        {
                            foreach (var TablesVsStepsItem in StepsItem.AuditTablesVsSteps)
                            {
                                if (TablesVsStepsItem.AuditTablesDefinition.TableName == item.TableName)
                                {
                                    auditTablesDefinition = TablesVsStepsItem.AuditTablesDefinition;
                                    TableExistInConfiguration = true;
                                }
                            }
                        }

                    }

                    if (!TableExistInConfiguration)
                    {

                        auditTablesDefinition.TableName = item.TableName;
                        auditTablesDefinition.IdTablesStatus = "N";

                        db.AuditTablesDefinition.Add(auditTablesDefinition);

                        db.SaveChanges();

                    }

                    #endregion

                    #region Add the relation between step and table definition

                    AuditTablesVsSteps auditTablesVsSteps = new AuditTablesVsSteps();

                    auditTablesVsSteps.IdStep = resquest.SeletedStepId;
                    auditTablesVsSteps.IdTableDefinition = auditTablesDefinition.IdTableDefinition;

                    db.AuditTablesVsSteps.Add(auditTablesVsSteps);
                    db.SaveChanges();

                    #endregion
                }

                #region If the connection relation not exist, create them

                if (db.AuditConnectionProcess.Where(p => p.IdConnectionOrigin == resquest.SeletedOriginId
                    && p.IdConnectionDestination == resquest.SeletedDestinationId
                    && p.IdProcess == resquest.SeletedProcessId).FirstOrDefault() == null)
                {

                    db.AuditConnectionProcess.Add(
                        new AuditConnectionProcess()
                        {
                            IdProcess = resquest.SeletedProcessId,
                            IdConnectionOrigin = resquest.SeletedOriginId,
                            IdConnectionDestination = resquest.SeletedDestinationId
                        });
                }

                db.SaveChanges();

                #endregion

                return "DONE";
            }
            catch (Exception ex)
            {
                Debug.Write("[Ultimus.ComponentManager] SaveTablesToAudit ERROR " + ex.ToString());
                return "ERROR";
            }
        }

        public IEnumerable<AuditTablesVsSteps> GetTablesByStepId(int id)
        {

            #region Local Variables

            IEnumerable<AuditTablesVsSteps> AuditTablesVsStepsList;

            #endregion

            try
            {

                #region Get All AuditTablesConfiguration by Step Id with the relation of table AuditTablesVsStepsList

                AuditTablesVsStepsList = (from d in db.AuditTablesVsSteps
                                          where d.IdStep == id
                                          select d).
                           Include("AuditTablesDefinition").Include("AuditTablesDefinition.CatAuditTablesStatus").ToList();


                #endregion

                return AuditTablesVsStepsList;
            }
            catch (Exception ex)
            {

                Debug.Write("[Ultimus.ComponentManager] GetTablesByStepId ERROR " + ex.ToString());
                return null;
            }
        }

        #endregion

        #region Columns definition Methods

        public IEnumerable<ColumnInformation> GetColumnByTableId(int id)
        {
            #region Local Parameters

            ICollection<ColumnInformation> ColumnConfigList = new HashSet<ColumnInformation>();
            AuditColumnsDefinition auditColumnsDefinition;
            AuditTablesDefinition auditTablesConfiguration;
            string keyColumnName = string.Empty;
            DataBaseAccess da = new DataBaseAccess();

            #endregion


            auditTablesConfiguration = db.AuditTablesDefinition.Where(d => d.IdTableDefinition == id)
                .Include("ColumnsDefinitions")
                .Include("AuditTablesVsSteps")
                .Include("AuditTablesVsSteps.CatSteps")
                .Include("AuditTablesVsSteps.CatSteps.CatProcesses")
                .Include("AuditTablesVsSteps.CatSteps.CatProcesses.AuditConnectionProcess")
                .Include("AuditTablesVsSteps.CatSteps.CatProcesses.AuditConnectionProcess.ConnectionOrigin")
                .Include("AuditTablesVsSteps.CatSteps.CatProcesses.AuditConnectionProcess.ConnectionDestination").FirstOrDefault();



            #region Query for get primary key column
            var sch = (auditTablesConfiguration.TableName).Split('.')[0];
            var tab = (auditTablesConfiguration.TableName).Split('.')[1];

            string query = string.Format(@"SELECT B.COLUMN_NAME
                                   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS A, INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE B
                                   WHERE CONSTRAINT_TYPE = 'PRIMARY KEY'
                                   AND CONCAT('[',A.CONSTRAINT_SCHEMA, '].[',A.TABLE_NAME, ']') ='{0}'
                                   AND CONCAT('[', B.TABLE_SCHEMA , '].[',B.TABLE_NAME,']') ='{1}'", auditTablesConfiguration.TableName, auditTablesConfiguration.TableName);

//            string query = string.Format(@"SELECT B.COLUMN_NAME
//                                 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS A, INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE B
//                                 WHERE CONSTRAINT_TYPE = 'PRIMARY KEY' 
//                                AND A.CONSTRAINT_NAME = B.CONSTRAINT_NAME 
//                                AND A.TABLE_NAME = '{0}'", auditTablesConfiguration.TableName);

            #endregion

            try
            {
                #region Get Primary key Column


                string cs = auditTablesConfiguration.AuditTablesVsSteps.FirstOrDefault()
                     .CatSteps.CatProcesses.AuditConnectionProcess.ConnectionOrigin.ConnectionStringDecrypted;


                da = new DataBaseAccess(cs, query);

                System.Data.SqlClient.SqlDataReader SqlDR;
                SqlDR = da.SqlCom.ExecuteReader();
                List<string> listaColNames = new List<string>();
                while (SqlDR.Read())
                {

                    listaColNames.Add(SqlDR.GetValue(0).ToString());

                   // keyColumnName = SqlDR.GetValue(0).ToString();
                }

                da.CloseConnection();

                #endregion

                #region Get Columns

                #region Query for get primary key column
               
                query = string.Format("exec sp_columns '{0}', '{1}'", tab.Substring(1, tab.Length-2), sch.Substring(1, sch.Length-2));

                #endregion

                da = new DataBaseAccess(cs, query);

                SqlDR = da.SqlCom.ExecuteReader();

                while (SqlDR.Read())
                {
                    ColumnInformation ColumnConfig = new ColumnInformation();

                    ColumnConfig.ColumnName = SqlDR.GetString(3);
                    if (SqlDR.GetString(5).Contains("identity"))
                       ColumnConfig.DataType = SqlDR.GetString(5).Replace(" identity","");
                    else
                       ColumnConfig.DataType = SqlDR.GetString(5);

                    ColumnConfig.DataLength = int.Parse(SqlDR.GetValue(7).ToString());
                    ColumnConfig.IdTableDefinition = id;

                    #region Determine if the column is primary key

                    // si ColumnConfig.ColumnName esta incluido en listaColNames 

                    if(listaColNames.Contains(ColumnConfig.ColumnName))
                    {
                         ColumnConfig.IsKey = true;
                         ColumnConfig.IsAuditColumn = true;
                    }
                    //if (ColumnConfig.DataType.ToString().Contains("identity"))
                    //    ColumnConfig.DataType = ColumnConfig.DataType.Replace("identity", string.Empty);

                    //if (keyColumnName == ColumnConfig.ColumnName)
                    //{
                    //    ColumnConfig.IsKey = true;
                    //    ColumnConfig.IsAuditColumn = true;
                    //}

                    #endregion

                    if (ColumnConfig.ColumnName == "Process" ||
                        ColumnConfig.ColumnName == "Step" ||
                        ColumnConfig.ColumnName == "Incident" ||
                        ColumnConfig.ColumnName == "UserRequest" ||
                        ColumnConfig.ColumnName == "DateRequest" ||
                        ColumnConfig.ColumnName == "TimeRequest" ||
                        ColumnConfig.ColumnName == "IPAdress" ||
                        ColumnConfig.ColumnName == "MachineName" ||
                        ColumnConfig.ColumnName == "ActivityType" ||
                        ColumnConfig.ColumnName == "HostResponse" ||
                        ColumnConfig.ColumnName == "MACAdress"    ||
                        ColumnConfig.ColumnName == "BrowserName"   ||
                        ColumnConfig.ColumnName == "BrowserVersion")
                    {
                        ColumnConfig.IsKey = false;
                        ColumnConfig.IsAuditColumn = true;
                    }

                    #region Define if the column is already in Column Definition

                    auditColumnsDefinition = auditTablesConfiguration.ColumnsDefinitions.Where(
                        d => d.ColumnName.Trim() == ColumnConfig.ColumnName.Trim()).FirstOrDefault();

                    if (auditColumnsDefinition != null)
                        ColumnConfig.IsAuditColumn = true;

                    #endregion

                    ColumnConfigList.Add(ColumnConfig);
                }
                #endregion


            }
            catch (Exception ex)
            {
                Debug.Write("[Ultimus.ComponentManager] GetColumnByTableId ERROR " + ex.ToString());
            }
            finally
            {
                da.SqlCon.Close();
            }

            return ColumnConfigList;
        }

        public string SaveColumnToAudit([FromBody] IEnumerable<ColumnInformation> ColumnInformationList)
        {
            #region Local Variables

            AuditColumnsDefinition auditColumnsDefinition;
            AuditTablesDefinition auditTablesDefinition;
            int IdTableDefinition = 0;

            #endregion

            if (ColumnInformationList.FirstOrDefault() != null)
            {
                IdTableDefinition = ColumnInformationList.FirstOrDefault().IdTableDefinition;
            }

            try
            {

                foreach (var item in ColumnInformationList)
                {
                    auditColumnsDefinition = db.AuditColumnsDefinition.Where(d => d.ColumnName == item.ColumnName
                                                && d.IdTableDefinition == IdTableDefinition)
                                                .FirstOrDefault();

                    if (auditColumnsDefinition == null && item.IsAuditColumn)
                    {
                        auditColumnsDefinition = new AuditColumnsDefinition();
                        auditColumnsDefinition.ColumnName = item.ColumnName;
                        auditColumnsDefinition.DataType = item.DataType;
                        auditColumnsDefinition.IdTableDefinition = item.IdTableDefinition;
                        auditColumnsDefinition.IsKey = item.IsKey;
                        auditColumnsDefinition.DataLength = item.DataLength;
                        auditColumnsDefinition.wasPublished = false;

                        db.AuditColumnsDefinition.Add(auditColumnsDefinition);
                    }

                }

                #region Save the nnew status "Pending changes" for table definition.

                if (!(IdTableDefinition == 0))
                {

                    auditTablesDefinition = db.AuditTablesDefinition.Find(IdTableDefinition);

                    auditTablesDefinition.IdTablesStatus = "PC";
                    db.Entry(auditTablesDefinition).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
                else
                {

                    return "NO_CHANGES";

                }

                #endregion

                return "DONE";
            }
            catch (Exception ex)
            {
                Debug.Write("[Ultimus.ComponentManager] SaveColumnToAudit ERROR " + ex.ToString());
                return "ERROR";
            }
        }

        [HttpGet]
        public string PublishTable(int id)
        {
            #region Local Variables

            AuditTablesDefinition auditTablesConfiguration;
            string SqlScript = string.Empty;
            Boolean alterTable = false;

            #endregion

            try
            {
                #region Get Audit tables definition information

                auditTablesConfiguration = db.AuditTablesDefinition.Where(d => d.IdTableDefinition == id)
                   .Include("ColumnsDefinitions")
                   .Include("CatAuditTablesStatus")
                   .Include("AuditTablesVsSteps")
                   .Include("AuditTablesVsSteps.CatSteps")
                   .Include("AuditTablesVsSteps.CatSteps.CatProcesses")
                   .Include("AuditTablesVsSteps.CatSteps.CatProcesses.AuditConnectionProcess")
                   .Include("AuditTablesVsSteps.CatSteps.CatProcesses.AuditConnectionProcess.ConnectionDestination").FirstOrDefault();


                #endregion

                #region Check if the table is already publish

                if (auditTablesConfiguration.IdTablesStatus.Trim() == "P")
                {
                    return "TABLE_ALREADY_PUBLISHED";
                }

                #endregion

                #region Check if have Column to Publish

                if (auditTablesConfiguration.ColumnsDefinitions == null ||
                    auditTablesConfiguration.ColumnsDefinitions.Count() == 0)
                {
                    return "NO_COLUMNS";
                }

                #endregion

                #region Check if the logic is going to run a create or alter table script.

                int countPublished = auditTablesConfiguration.ColumnsDefinitions.Where(p => p.wasPublished == true).Count();
                int countUnPublished = auditTablesConfiguration.ColumnsDefinitions.Where(p => p.wasPublished == false).Count();

                if (countPublished > 0 && countUnPublished > 0)
                {
                    alterTable = true;
                }

               
                #endregion

                #region Create default columns in script

                if (!alterTable)
                {
                    //SqlScript += "[Process] [varchar] (100) NOT NULL,";
                    //SqlScript += "[Step] [varchar] (100) NOT NULL,";
                    //SqlScript += "[Incident] [varchar] (100) NOT NULL,";
                    //SqlScript += "[UserRequest] [varchar] (100) NOT NULL,";
                    //SqlScript += "[DateRequest] [datetime] NOT NULL,";
                    //SqlScript += "[TimeRequest] [int] NOT NULL,";
                    //SqlScript += "[IPAdress] [varchar] (100) NOT NULL,";
                    //SqlScript += "[MachineName] [varchar] (100) NOT NULL,";
                    //SqlScript += "[ActivityType] [varchar] (100) NOT NULL,";
                    //SqlScript += "[HostResponse] [varchar] (100) NOT NULL,";
                }

                #endregion

                #region Create columns in script

                foreach (var item in auditTablesConfiguration.ColumnsDefinitions.Where(p => p.wasPublished == false))
                {
                    if (item.DataType == "varchar" || item.DataType == "nvarchar")
                    {
                        if (!item.IsKey)
                            SqlScript += "[" + item.ColumnName + "] " + item.DataType + " (" + item.DataLength + ") NULL,";
                        else
                            SqlScript += "[" + item.ColumnName + "] " + item.DataType + " (" + item.DataLength + ") NOT NULL,";
                    }
                    else
                    {
                        if (!item.IsKey)
                            SqlScript += "[" + item.ColumnName + "] " + item.DataType + " NULL,";
                        else
                            SqlScript += "[" + item.ColumnName + "] " + item.DataType + " NOT NULL,";
                    }

                    item.wasPublished = true;

                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }

                SqlScript = SqlScript.Remove(SqlScript.Length - 1);

                var arr = auditTablesConfiguration.TableName.Split('.');

                

              
                #endregion

                #region Get Connection

                string cs = auditTablesConfiguration.AuditTablesVsSteps.FirstOrDefault()
                    .CatSteps.CatProcesses.AuditConnectionProcess.ConnectionDestination.ConnectionStringDecrypted;

                #endregion

                var ExistsSchema = "DECLARE @sch varchar(100) IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name ='" + arr[0].Substring(1, arr[0].Length - 2) + "')" +
                                   " SET @sch = 'CREATE SCHEMA " + arr[0].Substring(1, arr[0].Length - 2) + "' " + "ELSE SET @sch ='' SELECT @Sch Esquema";
                 DataBaseAccess obj = new DataBaseAccess(cs, ExistsSchema);

                 var esquema = "";
                  
                 var SqlDR = obj.SqlCom.ExecuteReader();

                 while (SqlDR.Read())
                 {
                     esquema = SqlDR.GetString(0);
                 }

               // int execute = obj.SqlCom.ExecuteNonQuery();
                obj.CloseConnection();

                if (alterTable)
                {
                    SqlScript = "ALTER TABLE " + auditTablesConfiguration.TableName + " ADD " + SqlScript;
                }
                else
                {
                    SqlScript = esquema + " CREATE TABLE " + arr[0] +"." + arr[1] + "( " + SqlScript + " )";
                        //auditTablesConfiguration.TableName + "( " + SqlScript + " )";
                }
                DataBaseAccess da = new DataBaseAccess(cs, SqlScript);

                int execute = da.SqlCom.ExecuteNonQuery();
                da.CloseConnection();

                if (!alterTable)
                {
                    string constr = "ALTER TABLE " + auditTablesConfiguration.TableName + " ADD CONSTRAINT [PK_" + arr[1].Substring(1, arr[1].Length - 2) + "] PRIMARY KEY CLUSTERED (";
                    foreach (var item in auditTablesConfiguration.ColumnsDefinitions)
                    {
                        if (item.IsKey)
                        {
                            constr += item.ColumnName + " ASC, ";
                        }
                    }
                    constr = constr.Remove(constr.Length - 2);
                    constr = constr + ")";

                    da = new DataBaseAccess(cs, constr);

                    execute = da.SqlCom.ExecuteNonQuery();
                    da.CloseConnection();
                }
                #region Change Status

                auditTablesConfiguration.IdTablesStatus = "P";

                db.Entry(auditTablesConfiguration).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


                #endregion

                return "DONE";
            }
            catch (Exception ex)
            {
                Debug.Write("[Ultimus.ComponentManager] SaveColumnToAudit ERROR " + ex.ToString());
                return "ERROR";
            }
        }


        #endregion
    }

    public class GetConnectionsResponse
    {
        public IEnumerable<CatConnections> CatConnectionsList { get; set; }
        public int SeletedOriginId { get; set; }
        public int SeletedDestinationId { get; set; }
        public bool haveConnectionConfig { get; set; }
    }

    public class SaveTableConfigRequest
    {
        public int SeletedProcessId { get; set; }
        public int SeletedStepId { get; set; }
        public int SeletedOriginId { get; set; }
        public int SeletedDestinationId { get; set; }

        public IEnumerable<TableInformation> TablesToAudit { get; set; }
    }

    public class ColumnInformation
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public int DataLength { get; set; }
        public Boolean IsKey { get; set; }
        public Boolean IsAuditColumn { get; set; }
        public int IdTableDefinition { get; set; }
    }

    public class TableInformation { public string TableName { get; set; } }

}