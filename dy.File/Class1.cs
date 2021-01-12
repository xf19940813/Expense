using System;
using System.Web;
namespace dy.File
{
    public class Class1
    {
        public ContentResult UploadFileNew()
        {
            ResultData result = new ResultData();
            try
            {
                string path = "/tmp/";
                HttpPostedFile file = SyHttpContext.Current.Request.Files["File"]; //对应小程序 name
                string parameters = string.Format("postData:{0}", file.ToString());

                //获取文件
                if (file != null)
                {
                    Stream sr = file.InputStream;        //文件流
                    Bitmap bitmap = (Bitmap)Bitmap.FromStream(sr);
                    path += file.FileName;
                    string currentpath = System.Web.HttpContext.Current.Server.MapPath("~");

                    bitmap.Save(currentpath + path);
                }
                result.status = 1;
                result.data = path;
            }
            catch (Exception vErr)
            {
                result.status = -1;
                result.detail = vErr.Message;
                return result;
            }
            return result;
        }
    }
}
