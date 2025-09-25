using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetCafe.Domain.Entities
{
    [Table("storages")]
    public class Storage : BaseEntity
    {
        [Column("file_name")]
        public string FileName { get; set; } = default!;

        [Column("content_type")]
        public string ContentType { get; set; } = string.Empty;
        [Column("size")]
        public long Size { get; set; }

        [Column("extension")]
        public string Extension { get; set; } = string.Empty;
        [Column("url")]
        public string Url { get; set; } = string.Empty;
    }
}