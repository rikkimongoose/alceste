using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alceste.DAL.DataTypes
{
    public class CachedItem
    {
        public CachedItem()
        {
            ChannelsCount = 1;
            ChannelNumber = 1;
        }
        
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AccountId { set; get; }

        public string FileItemId { get; set; }
        
        public string FilePath { get; set; }

        public double? Length { get; set; }

        public int ChannelsCount { get; set; }

        public int ChannelNumber { get; set; }

        public string WaveFormat { get; set; }

        public virtual ICollection<CachedItemImage> Images { get; set; }
    }
}
