using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ultimus.ComponentManager.Models
{
    //[Table("CatConnectionTypes", Schema = "Catalogs")]
    public class CatConnectionTypes
    {
        public CatConnectionTypes()
        {
            this.Connections = new HashSet<CatConnections>();
        }

        #region IdConnectionType

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdConnectionType { get; set; }
        
        #endregion

        #region ConnectionName

        [Required]
        [Column(TypeName = "NVARCHAR")]
        [MaxLength(100)]
        public string ConnectionName { get; set; }
        
        #endregion

        #region One ConnectionTypes have many

        public virtual ICollection<CatConnections> Connections { get; set; }
        
        #endregion

        
    }
}
