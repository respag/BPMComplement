using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ultimus.ComponentManager.Models
{
    public class CatSteps
    {

        public CatSteps()
        {
            this.FormsProcess = new HashSet<FormsProcess>();
            this.AuditTablesVsSteps = new HashSet<AuditTablesVsSteps>();
        }

        [DisplayName("#")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdStep { get; set; }

        [DisplayName("Step Name")]
        [Required(ErrorMessage = "Step Name is required")]
        [Column(TypeName = "NVARCHAR")]
        [MaxLength(200)]
        public string StepName { get; set; }

        [DisplayName("Process")]
        public int IdProcess { get; set; }
        public virtual CatProcesses CatProcesses { get; set; }

        public virtual ICollection<FormsProcess> FormsProcess { get; set; }
        public virtual ICollection<AuditTablesVsSteps> AuditTablesVsSteps { get; set; }

    }
}
