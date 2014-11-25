namespace Alceste.DAL.DataTypes
{
    public class CachedItemImage : AItemWithId
    {
        public byte[] MediaImage { get; set; }

        public int? MediaImageWidth { get; set; }

        public int? MediaImageHeight { get; set; }
    }
}
