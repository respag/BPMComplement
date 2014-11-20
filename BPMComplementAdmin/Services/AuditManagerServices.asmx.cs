using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;
using System.Xml;
using System.IO;
using Ultimus.ComponentManager.Models;
using Ultimus.AuditManager.Admin.Commons;
using System.Diagnostics;

namespace Ultimus.AuditManager.Admin.Services
{
    /// <summary>
    /// Summary description for AuditManagerServices
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AuditManagerServices : System.Web.Services.WebService
    {

        [WebMethod]
        public void AuditProcess(string Process, string Step, int incident)
        {
            Ultimus.UtilityLayer.LogFilecs a = new UtilityLayer.LogFilecs();

            Debug.Write("[Ultimus.AuditManagerServices] Process = " + Process);
            Debug.Write("[Ultimus.AuditManagerServices] Step = " + Step);
            Debug.Write("[Ultimus.AuditManagerServices] incident = " + incident.ToString());

            DataBaseAccess da = new DataBaseAccess();

            try
            {
                #region Get Process and Step information

                ComponentManagerContext db = Utilities.GetComponentManagerContextForApi();
                CatSteps info;

                info = db.CatSteps.Include("CatProcesses").Include("CatProcesses.AuditConnectionProcess")
                    .Include("CatProcesses.AuditConnectionProcess.ConnectionDestination")
                    .Include("CatProcesses.AuditConnectionProcess.ConnectionOrigin")
                    .Include("AuditTablesVsSteps").Include("AuditTablesVsSteps.AuditTablesDefinition")
                    .Include("AuditTablesVsSteps.AuditTablesDefinition.CatAuditTablesStatus")
                    .Include("AuditTablesVsSteps.AuditTablesDefinition.ColumnsDefinitions")
                    .Where(d => d.StepName == Step && d.CatProcesses.ProcessName == Process).FirstOrDefault();

                Debug.Write("[Ultimus.AuditManagerServices] Get Process and Step information");

                #endregion

                foreach (AuditTablesVsSteps tableInfo in info.AuditTablesVsSteps
                    .Where(d => d.AuditTablesDefinition.IdTablesStatus.Trim() == "P"))
                {

                    Debug.Write("[Ultimus.AuditManagerServices] tableName = " + tableInfo.AuditTablesDefinition.TableName);

                    #region Initiate Variables

                    string tableName = tableInfo.AuditTablesDefinition.TableName;
                    string queryGetInfo = "SELECT {0} FROM {1} WHERE Incident = {2} AND Process = '{3}' AND Step = '{4}'";
                    string columnList = string.Empty;

                    Debug.Write("[Ultimus.AuditManagerServices] Initiate Variables");

                    #endregion

                    #region Create select to get info
                    string[] arrColumnList=null;
                    foreach (AuditColumnsDefinition columnInfo in tableInfo.AuditTablesDefinition.ColumnsDefinitions)
                    {
                        columnList += string.Concat(columnInfo.ColumnName, ", ");
                        arrColumnList = columnList.Split(',');
                    }

                    //remove the last "," for not have a sintaxis error when run thw SQL script.
                    columnList = columnList.Remove(columnList.Length - 2);

                    //
                     int lugar = Array.IndexOf(arrColumnList, " Numero");

                    //Filter for get the info
                    queryGetInfo = string.Format(queryGetInfo, columnList, tableName, incident.ToString(), Process, Step);
                    //Quita el campo numero que no está en el origen
                    queryGetInfo = queryGetInfo.Replace("Numero,","");
                    Debug.Write("[Ultimus.AuditManagerServices] queryGetInfo=" + queryGetInfo);

                    Debug.Write("[Ultimus.AuditManagerServices] Create select to get info");

                    #endregion

                    #region Get Connection origin

                    string cs = info.CatProcesses.AuditConnectionProcess.ConnectionOrigin.ConnectionStringDecrypted;

                    Debug.Write("[Ultimus.AuditManagerServices] Get Connection origin");

                    #endregion

                    #region Execute the query in sql

                    da = new DataBaseAccess(cs, queryGetInfo);
                    System.Data.SqlClient.SqlDataReader SqlDR;

                    SqlDR = da.SqlCom.ExecuteReader();

                    Debug.Write("[Ultimus.AuditManagerServices] Execute the query in sql");

                    #endregion

                    string columnListData = string.Empty;
                    queryGetInfo = string.Empty;


                    Debug.Write("[Ultimus.AuditManagerServices] HasRows = " + SqlDR.HasRows.ToString());


                    if (SqlDR.HasRows)
                    {

                        while (SqlDR.Read())
                        {
                            #region Inizialize variables for bucle

                            int columnIndex = 0;
                            columnListData = string.Empty;

                            Debug.Write("[Ultimus.AuditManagerServices] Inizialize variables for bucle");

                            #endregion

                            #region Generate data to insert
                            var flag = false;
                            foreach (AuditColumnsDefinition columnInfo in tableInfo.AuditTablesDefinition.ColumnsDefinitions)
                            { 
                                if (columnIndex != lugar)//&& !flag)
                                {
                                    if (columnInfo.DataType == "int")
                                        columnListData += (SqlDR.GetValue(columnIndex).ToString() + " ,");
                                    else if (columnInfo.DataType == "varchar" || columnInfo.DataType == "text")
                                        columnListData += ("'" + SqlDR.GetValue(columnIndex).ToString() + "'" + " , ");
                                    else if (columnInfo.DataType == "datetime")
                                    {
                                        DateTime timeToFormat = DateTime.Parse(SqlDR.GetValue(columnIndex).ToString());
                                        string date = String.Format("{0:dd-MM-yyyy HH:mm:ss}", timeToFormat);

                                        columnListData += ("CONVERT (datetime, '" + date + "', 104)" + " , ");
                                    }
                                    else
                                        columnListData += ("'" + SqlDR.GetValue(columnIndex).ToString() + "'" + " , ");

                                    columnIndex++;
                                }
                                //else if (columnIndex != 12 && flag)
                                //{

                                //}
                                //else if (columnIndex == 12)
                                else if (columnIndex == lugar && !flag)
                                {
                                    cs = info.CatProcesses.AuditConnectionProcess.ConnectionDestination.ConnectionStringDecrypted;
                                    da = new DataBaseAccess(cs, "SELECT TOP 1 Numero FROM " + tableInfo.AuditTablesDefinition.TableName + " ORDER BY Numero DESC");
                                    var id = da.SqlCom.ExecuteScalar();
                                    columnListData += (((int)id + 1) + " ,");
                                    //columnIndex++;
                                    flag = true;
                                   // continue;
                                }
                                else if (columnIndex == lugar && flag)
                                {
                                    if (columnInfo.DataType == "int")
                                        columnListData += (SqlDR.GetValue(columnIndex).ToString() + " ,");
                                    else if (columnInfo.DataType == "varchar" || columnInfo.DataType == "text")
                                        columnListData += ("'" + SqlDR.GetValue(columnIndex).ToString() + "'" + " , ");
                                    else if (columnInfo.DataType == "datetime")
                                    {
                                        DateTime timeToFormat = DateTime.Parse(SqlDR.GetValue(columnIndex).ToString());
                                        string date = String.Format("{0:dd-MM-yyyy HH:mm:ss}", timeToFormat);

                                        columnListData += ("CONVERT (datetime, '" + date + "', 104)" + " , ");
                                    }
                                    columnIndex++;
                                }
                            }

                            Debug.Write("[Ultimus.AuditManagerServices] Generate data to insert");

                            #endregion

                            #region Generate insert script

                            columnListData = columnListData.Remove(columnListData.Length - 2);
                            queryGetInfo += string.Format(" INSERT INTO {0} ({1}) VALUES ({2}) \n", tableName, columnList, columnListData);


                            Debug.Write("[Ultimus.AuditManagerServices] Generate insert script");

                            #endregion
                        }

                        da.CloseConnection();

                        #region Get Destination connection

                        cs = info.CatProcesses.AuditConnectionProcess.ConnectionDestination.ConnectionStringDecrypted;

                        Debug.Write("[Ultimus.AuditManagerServices] Get Destination connection");

                        #endregion

                        #region Execute inserts



                        Debug.Write("[Ultimus.AuditManagerServices] queryGetInfo =" + queryGetInfo);

                        da = new DataBaseAccess(cs, queryGetInfo);

                        int execute = da.SqlCom.ExecuteNonQuery();

                        Debug.Write("[Ultimus.AuditManagerServices] Execute inserts");

                        da.CloseConnection();


                        #endregion

                    }


                }


            }
            catch (Exception ex)
            {
                Debug.Write("[Ultimus.AuditManagerServices] AuditProcess ERROR " + ex.ToString());

            }
        }
    }
}


