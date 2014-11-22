using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ultimus.ComponentManager.Models
{
    //[Table("CatProcesses", Schema = "Catalogs")]
    public class CatProcesses
    {
        public CatProcesses()
        {
            this.CatSteps = new HashSet<CatSteps>();
            this.HistoryConfiguration = new HashSet<HistoryConfiguration>();
            this.WebAplicationDetails = new HashSet<WebAplicationDetails>();
        }

        #region IdProcess

        [DisplayName("#")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdProcess { get; set; }
        
        #endregion

        #region ProcessName

        [DisplayName("Name")]
        [Required(ErrorMessage = "Root Location is required")]
        [Column(TypeName = "NVARCHAR")]
        [MaxLength(100)]
        public string ProcessName { get; set; }
        
        #endregion

        //#region RootLocation

        //[DisplayName("Root location")]
        ////[Required(ErrorMessage = "Root Location is required")]
        //public string RootLocation { get; set; }

        //#endregion

        #region ProcessVersion

        [DisplayName("Version")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Process Version only accept numbers")]
        [Required]
        public int ProcessVersion { get; set; }
        
        #endregion

        #region One Process have one

        public virtual AuditConnectionProcess AuditConnectionProcess { get; set; }
        
        #endregion

        #region One Process have many

        public virtual ICollection<CatSteps> CatSteps { get; set; }
        public virtual ICollection<HistoryConfiguration> HistoryConfiguration { get; set; }
        public virtual ICollection<WebAplicationDetails> WebAplicationDetails { get; set; }

        #endregion

    }
}
