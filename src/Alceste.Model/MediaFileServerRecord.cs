namespace Alceste.Model
{
    public class MediaFileServerRecord
    {
        public string Title { get; set; }
        public int Length { get; set; }

        public MediaFileServerRecord()
        {
            Length = 0;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
