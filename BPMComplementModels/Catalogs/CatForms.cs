using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ultimus.ComponentManager.Models
{
    //[Table("CatForms", Schema = "Catalogs")]
    public class CatForms
    {
        public CatForms()
        {
            this.FormsProcess = new HashSet<FormsProcess>();
            this.HelpProcess = new HashSet<HelpProcess>();
        }

        #region IdForm

        [DisplayName("#")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int IdForm { get; set; }

        #endregion

        #region FormFile

        [DisplayName("Form File")]
        [Required(ErrorMessage = "Form File is required")]
        public string FormFile { get; set; }

        #endregion

        #region FormLabel

        [DisplayName("Form Label")]
        [Required(ErrorMessage = "Form Label is required")]
        [Column(TypeName = "NVARCHAR")]
        [MaxLength(200)]
        public string FormLabel { get; set; }

        #endregion

        #region IdWebAplication -> Foreign key

        [DisplayName("Web Aplication")]
        [Required]
        public int IdWebAplication { get; set; }
        public virtual CatWebAplications CatWebAplications { get; set; }

        #endregion

        #region One Process have many

        public virtual ICollection<FormsProcess> FormsProcess { get; set; }
        public virtual ICollection<HelpProcess> HelpProcess { get; set; }

        #endregion

        #region Calculate Columns

        #region FormFileLimit

        [NotMapped]
        public string FormFileLimit
        {
            get
            {
                const int MaxLength = 15;
                string label = FormFile;

                if (FormFile.Length > MaxLength)
                    label = FormFile.Substring(0, MaxLength) + "...";

                return label;
            }
        }

        #endregion

        #region FormFileLength

        [NotMapped]
        public bool FormFileLength
        {
            get
            {
                const int MaxLength = 15;

                if (FormFile.Length > MaxLength)
                    return true;

                return false;
            }
        }

        #endregion

        #endregion
    }
}
