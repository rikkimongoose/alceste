using System.Drawing.Imaging;

namespace Alceste.RestService.DataControllers
{
    public sealed class ImageMIMETypeController : AKeyValueController<string, ImageFormat>
    {
        private ImageMIMETypeController()
        {
        }

        private static ImageMIMETypeController _instance;
        private static object _syncRoot = new object();

        public static ImageMIMETypeController Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            var temp = new ImageMIMETypeController();
                            System.Threading.Thread.MemoryBarrier();
                            _instance = temp;
                        }
                    }
                }
                return _instance;
            }
        }

        public override void LoadValues()
        {
            AddItem("image/jpeg", ImageFormat.Jpeg);
            AddItem("image/png", ImageFormat.Png);
            AddItem("image/gif", ImageFormat.Gif);
            AddItem("image/tiff", ImageFormat.Tiff);
        }

        protected override string NoKeyExceptionText
        {
            get { return "The MIME type {0} is not supported"; }
        }

        protected override string NoValueExceptionText
        {
            get { return "The image format {0} has no MIME type by web standarts"; }
        }
    }
}
