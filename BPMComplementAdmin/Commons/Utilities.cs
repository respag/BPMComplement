using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ultimus.ComponentManager.Models;
using Ultimus.UtilityLayer;

namespace Ultimus.AuditManager.Admin.Commons
{
    public static class Utilities
    {
        public static bool IsAuditInstalled()
        {
            string keyName = @"HKEY_CLASSES_ROOT\Ultimus.LA.Products";
            string valueName = "AuditInstalado";

            if (((string)Registry.GetValue(keyName, valueName, "no") == "no") ||
                (Registry.GetValue(keyName, valueName, "No") == null))
            {
                //code if key Not Exist
                return false;
            }
            else
            {
                //code if key Exist
                return true;
            }
        }

        public static bool IsProcessInstalled()
        {
            string keyName = @"HKEY_CLASSES_ROOT\Ultimus.LA.Products";
            string valueName = "ProcesosIntalado";

            if (((string)Registry.GetValue(keyName, valueName, "no") == "no") ||
                (Registry.GetValue(keyName, valueName, "no") == null))
            {
                //code if key Not Exist
                return false;
            }
            else
            {
                //code if key Exist
                return true;
            }
        }

        public static ComponentManagerContext GetComponentManagerContext()
        {
            try
            {
                CacheManager objCache = new CacheManager();
                ObtieneParametros _ObtieneParametros = new ObtieneParametros();
                ComponentManagerContext auditManagerContext;
                auditManagerContext = objCache.GetMyCachedItem("AuditManagerContext") as ComponentManagerContext;

                if (auditManagerContext == null)
                {

                    string _connectionString = _ObtieneParametros.ConnectionString("UltimusComponentManager", true);
                    auditManagerContext = new ComponentManagerContext(_connectionString);
                }

                return auditManagerContext;
            }
            catch (Exception ex) {

                throw new Exception(ex.ToString());
            }      
        }

        public static ComponentManagerContext GetComponentManagerContextForApi()
        {
            //string keyName = @"HKEY_CLASSES_ROOT\Ultimus.LA.Products";
            //string valueName = "AuditConnectionString";
            try 
            {
                CacheManager objCache = new CacheManager();
                ObtieneParametros _ObtieneParametros = new ObtieneParametros();
                ComponentManagerContext auditManagerContext;
                auditManagerContext = objCache.GetMyCachedItem("AuditManagerContext") as ComponentManagerContext;
                if (auditManagerContext == null)
                {
                    string _connectionString = _ObtieneParametros.ConnectionString("UltimusComponentManager", true);
                    auditManagerContext = new ComponentManagerContext(_connectionString, false);
                }
                return auditManagerContext;
            }
          catch (Exception ex)
          {
              throw new Exception(ex.ToString());
          }    
        }

        public static void AddCachedItem(string key, string value)
        {
            CacheManager objCache = new CacheManager();
            objCache.AddToMyCache(key, value, MyCachePriority.Default); 
        }

        public static string GetCachedItem(string key)
        {
            CacheManager objCache = new CacheManager();

            if (objCache.GetMyCachedItem(key) != null)
            {
                return objCache.GetMyCachedItem("AuditManagerContext").ToString();
            }
            else {

                return string.Empty;

            }
        }

        public static void RemoveCachedItem(string key)
        {
            CacheManager objCache = new CacheManager();
            objCache.RemoveMyCachedItem(key);
        }
      
    }
}