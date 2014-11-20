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
    //[Table("HelpProcess", Schema = "Process")]
    public class HelpProcess
    {
        #region IdHelpProcess

        [DisplayName("#")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int IdHelpProcess { get; set; }
        
        #endregion

        #region IdForm

        [Required]
        public int IdForm { get; set; }
        public virtual CatForms CatForms { get; set; }

        #endregion

        #region Control

        [DisplayName("Control Id")]
        [Required(ErrorMessage = "Control Id is required")]
        [Column(TypeName = "NVARCHAR")]
        [MaxLength(300)]
        public string Control { get; set; }
        
        #endregion

        #region Text

        [DisplayName("Help Text")]
        [Required(ErrorMessage = "Help Text is required")]
        [Column(TypeName = "text")]
        public string Text { get; set; }
        
        #endregion

        #region ControlLabel

        [DisplayName("Control Name")]
        [Required(ErrorMessage = "Control Name is required")]
        [Column(TypeName = "NVARCHAR")]
        [MaxLength(300)]
        public string ControlLabel { get; set; }

        #endregion
    }
}
