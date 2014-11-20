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
    public class AuditColumnsDefinition
    {
        #region IdTableDefinition

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdTableDefinition { get; set; }
        public virtual AuditTablesDefinition TableDefinition { get; set; }
        
        #endregion

        #region ColumnName

        [Required]
        public string ColumnName { get; set; }
        
        #endregion

        #region IsKey

        [Required]
        public Boolean IsKey { get; set; }
        
        #endregion

        #region wasPublished

        [Required]
        public Boolean wasPublished { get; set; }
        
        #endregion

        #region isDisable

        [Required]
        public Boolean isDisable { get; set; }
        
        #endregion

        #region DataType

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(100)]
        public string DataType { get; set; }
        
        #endregion

        #region DataLength

        [Required]
        public int DataLength { get; set; }
        
        #endregion
    }
}
