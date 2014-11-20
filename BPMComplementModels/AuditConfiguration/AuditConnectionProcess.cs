using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ultimus.ComponentManager.Models
{
    public class AuditConnectionProcess
    {
        #region IdProcess

        [DisplayName("#")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdProcess { get; set; }
        
        #endregion

        #region IdConnectionOrigin -> Foreing Key

        [DisplayName("Connection Origin")]
        [Required]
        public int IdConnectionOrigin { get; set; }
        public virtual CatConnections ConnectionOrigin { get; set; }
        
        #endregion

        #region IdConnectionDestination -> Foreing Key

        [DisplayName("Connection Destination")]
        [Required]
        public int IdConnectionDestination { get; set; }
        public virtual CatConnections ConnectionDestination { get; set; }
        
        #endregion
    }
}
