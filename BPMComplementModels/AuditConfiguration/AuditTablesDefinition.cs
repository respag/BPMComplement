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
    public class AuditTablesDefinition
    {
        public AuditTablesDefinition()
        {
            this.ColumnsDefinitions = new HashSet<AuditColumnsDefinition>();
            this.AuditTablesVsSteps = new HashSet<AuditTablesVsSteps>();
        }

        #region IdTableDefinition

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTableDefinition { get; set; }
        
        #endregion

        #region TableName

        [Required]
        [Column(TypeName = "NVARCHAR")]
        [MaxLength(200)]
        public string TableName { get; set; }
        
        #endregion

        #region IdTablesStatus -> Foreing key

        [Required]
        [Column(TypeName = "CHAR")]
        [MaxLength(4)]
        public string IdTablesStatus { get; set; }
        public virtual CatAuditTablesStatus CatAuditTablesStatus { get; set; }
        
        #endregion
       
        #region One Process have many

        public virtual ICollection<AuditColumnsDefinition> ColumnsDefinitions { get; set; }
        public virtual ICollection<AuditTablesVsSteps> AuditTablesVsSteps { get; set; }

        #endregion

        #region No Mapped Columns

        [NotMapped]
        public int IdProcess { get; set; }

        #endregion

    }
}
