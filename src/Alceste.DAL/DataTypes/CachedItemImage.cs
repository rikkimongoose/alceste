namespace Alceste.DAL.DataTypes
{
    public class CachedItemImage : AItemWithId
    {
        public CachedItemImage()
        {
            MediaImageStoredInDb = true;
        }

        public byte[] MediaImage { get; set; }

        public int? MediaImageWidth { get; set; }

        public int? MediaImageHeight { get; set; }

        public string MediaImagePath { get; set; }

        public bool MediaImageStoredInDb { get; set; }
    }
}
