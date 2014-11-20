namespace ApiConectores.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CatConnection
    {
        [Key]
        public int IdConnections { get; set; }

        [Required]
        public string ConnectionString { get; set; }

        [Required]
        [StringLength(100)]
        public string ConnectionName { get; set; }

        public int IdConnectionType { get; set; }
    }
}
