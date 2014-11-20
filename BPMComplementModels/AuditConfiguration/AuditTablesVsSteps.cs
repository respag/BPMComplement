using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ultimus.ComponentManager.Models
{
    public class AuditTablesVsSteps
    {

        #region IdTableDefinition

        [Required]
        public int IdTableDefinition { get; set; }
        public virtual AuditTablesDefinition AuditTablesDefinition { get; set; }
        
        #endregion

        #region IdStep

        [Required]
        public int IdStep { get; set; }
        public virtual CatSteps CatSteps { get; set; }
        
        #endregion

        #region isDisabled

        public Boolean isDisabled { get; set; }
        
        #endregion

    }
}
