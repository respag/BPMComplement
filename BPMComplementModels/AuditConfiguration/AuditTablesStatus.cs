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
    public class CatAuditTablesStatus
    {
        public CatAuditTablesStatus()
        {
            this.AuditTablesConfiguration = new HashSet<AuditTablesDefinition>();
        }

        #region IdTablesStatus

        [Required]
        [Column(TypeName = "CHAR")]
        [MaxLength(4)]
        public string IdTablesStatus { get; set; }
        
        #endregion

        #region StatusName

        [Required]
        [Column(TypeName = "NVARCHAR")]
        [MaxLength(100)]
        public string StatusName { get; set; }
        
        #endregion

        #region One Process have many

        public virtual ICollection<AuditTablesDefinition> AuditTablesConfiguration { get; set; }
        
        #endregion
    }
}
