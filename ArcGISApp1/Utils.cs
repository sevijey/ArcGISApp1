using iText.Html2pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArcGISApp1
{
    public static class Utils
    {
        /// <summary>
        /// Convert a Html to PDF with Itext7 library
        /// </summary>
        /// <param name="htmlOrig"></param>
        /// <param name="pdfDest"></param>
        public static void HtmlToPdf(string htmlOrig, string pdfDest)
        {
            HtmlConverter.ConvertToPdf(new FileStream(htmlOrig, FileMode.Open, FileAccess.Read, FileShare.Read), new FileStream(pdfDest, FileMode.Create));
        }

        /// <summary>
        /// Save a Stream to a File path
        /// </summary>
        /// <param name="streamOrig"></param>
        /// <param name="pathFileDest"></param>
        public static void StreamToFile(Stream streamOrig, string pathFileDest)
        {
            if (File.Exists(pathFileDest))
                File.Delete(pathFileDest);

            FileStream fs = new FileStream(pathFileDest, FileMode.Create);
            streamOrig.Seek(0, SeekOrigin.Begin);
            streamOrig.CopyTo(fs);
            fs.Close();
        }
    }
}
