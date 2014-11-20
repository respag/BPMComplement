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
    //[Table("FormsProcess", Schema = "Process")]
    public class FormsProcess
    {
        #region IdFormProcess

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int IdFormProcess { get; set; }

        #endregion

        #region CatSteps -> Foreing Key

        [DisplayName("Step")]
        [Required(ErrorMessage = "Please choose a Step")]
        public int IdStep { get; set; }
        public virtual CatSteps CatSteps { get; set; }
        
        #endregion

        #region FormOrder

        [DisplayName("Order")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Order only accept numbers")]
        [Required]
        public int FormOrder { get; set; }
        
        #endregion

        #region IdForm -> Foreing Key

        [Required]
        public int IdForm { get; set; }
        public virtual CatForms CatForms { get; set; }
        
        #endregion

        #region IdProcess

        [NotMapped]
        public int IdProcess { get; set; }

        [NotMapped]
        public int IdWebAplication { get; set; }
        
        #endregion
    }
}
