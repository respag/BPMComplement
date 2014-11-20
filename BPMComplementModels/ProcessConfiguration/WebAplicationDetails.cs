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
    public class WebAplicationDetails
    {
        #region IdProcess

        [Key]
        [DisplayName("ID Process")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdProcess { get; set; }
        public virtual CatProcesses CatProcesses { get; set; }

        #endregion

        #region IdWebAplication

        [Key]
        [DisplayName("ID Web Aplication")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdWebAplication { get; set; }
        public virtual CatWebAplications CatWebAplications { get; set; }

        #endregion
    }
}
