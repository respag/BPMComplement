using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ultimus.UtilityLayer;

namespace Ultimus.ComponentManager.Models
{
    //[Table("CatConnections", Schema = "Catalogs")]
    public class CatConnections
    {

        public CatConnections()
        {
            this.AuditConnectionOrigin = new HashSet<AuditConnectionProcess>();
            this.AuditConnectionDestination = new HashSet<AuditConnectionProcess>();
        }

        #region IdConnections
        [DisplayName("#")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdConnections { get; set; }
        #endregion

        #region ConnectionString
        [DisplayName("Cadena de conexión")]
        [Required]
        public string ConnectionString { get; set; }
        #endregion

        #region ConnectionName
        [Required]
        [Column(TypeName = "NVARCHAR")]
        [MaxLength(100)]
        [DisplayName("Nombre del Conector")]
        public string ConnectionName { get; set; }
        #endregion

        #region IdConnectionType -> Foreing Key

        [Required]
        [DisplayName("Tipo de Conector")]
        public int IdConnectionType { get; set; }
        public virtual CatConnectionTypes CatConnectionType { get; set; }
        
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

        #endregion

        #region Calculated Columns


        [NotMapped]
        public string ConnectionStringDecrypted
        {
            get
            {
                Crypt crypt = new UtilityLayer.Crypt();
                string cs = crypt.DecryptString(this.ConnectionString);

                return cs;
            }
        }

        [NotMapped]
        public string ConnectionStringEncrypted
        {
            get
            {
                Crypt crypt = new UtilityLayer.Crypt();
                string cs = crypt.EncryptString(this.ConnectionString);

                return cs;
            }
        }

        #endregion

        #region One Process have many

        public virtual ICollection<AuditConnectionProcess> AuditConnectionOrigin { get; set; }
        public virtual ICollection<AuditConnectionProcess> AuditConnectionDestination { get; set; }
        
        #endregion
    }
}
