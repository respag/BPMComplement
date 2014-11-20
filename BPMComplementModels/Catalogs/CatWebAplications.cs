using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ultimus.ComponentManager.Models
{
    //[Table("CatWebAplications", Schema = "Catalogs")]
    public class CatWebAplications
    {

        public CatWebAplications()
        {
            this.CatForms = new HashSet<CatForms>();
            this.WebAplicationDetails = new HashSet<WebAplicationDetails>();
        }

        #region IdWebAplication

        [DisplayName("#")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdWebAplication { get; set; }

        #endregion

        #region WebAplicationName

        [DisplayName("Web Aplication Name")]
        [Required(ErrorMessage = "Web Aplication Name is required")]
        [Column(TypeName = "NVARCHAR")]
        [MaxLength(300)]
        public string WebAplicationName { get; set; }

        #endregion

        #region WebAplicationPath

        [DisplayName("Web Aplication Path")]
        [Required(ErrorMessage = "Web Aplication Path is required")]
        public string WebAplicationPath { get; set; }

        #endregion

        #region One Web Aplication have many

        public virtual ICollection<CatForms> CatForms { get; set; }
        public virtual ICollection<WebAplicationDetails> WebAplicationDetails { get; set; }

        #endregion

        #region Not Mapped Columns


        [NotMapped]
        public Boolean isUpdatedOrCreated { get; set; }

        [NotMapped]
        public Boolean isDeleted { get; set; }

        [NotMapped]
        public Boolean inUse { get; set; }

        [NotMapped]
        public Boolean toUpdate { get; set; }

        [NotMapped]
        public int IdProcess { get; set; }

        [NotMapped]
        public Boolean isAddedToProcess { get; set; }

        #endregion

    }
}
