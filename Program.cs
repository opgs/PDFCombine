using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.IO;

namespace OPG.PDFCombine
{
    class Tools
    {
        public static void CopyPages(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }
    }

    class Program
    {
        static int Main(string[] args)
        {
            if(args.Length < 1 || args[0] == "/?")
            {
                Console.WriteLine("OPG PDFCombine");
                Console.WriteLine("Usage: pdfcombine.exe path filename [overwrite]");
                Console.WriteLine("       path - absolute directory, no trailing /");
                Console.WriteLine("       filename - filename of pdf to combine to");
                Console.WriteLine("       overwrite - if combined file exists, overwrite it [1/Y/true = overwrite]");
                return 0;
            }

            string pathIn = args[0];
            string fileNameIn = args[1];
            string overwriteIn = "";

            try
            {
                if (args.Length < 2)
                {
                    overwriteIn = "false";
                }
                else
                {
                    overwriteIn = args[2];
                }
            }
            catch (Exception)
            {
                overwriteIn = "false";
            }

            bool overwrite = (overwriteIn == "1" || overwriteIn == "Y" || overwriteIn == "true");

            if(!Directory.Exists(pathIn))
            {
                Console.WriteLine("Directory not found: " + pathIn);
                return 1;
            }

            string fullFileName = pathIn + "/" + fileNameIn;
            if(!fullFileName.EndsWith(".pdf"))
            {
                fullFileName += ".pdf";
            }

            if (File.Exists(fullFileName))
            {
                if(overwrite)
                {
                    Console.WriteLine("PDF already exists, overwriting: " + fullFileName);
                    File.Delete(fullFileName);
                }
                else
                {
                    Console.WriteLine("PDF already exists, overwrite not set: " + fullFileName);
                    return 2;
                }
            }

            MemoryStream combined = new MemoryStream();

            using (PdfDocument outPdf = new PdfDocument())
            {
                outPdf.Options.CompressContentStreams = true;
                outPdf.Version = 17;

                string[] pdfsIn = Directory.GetFiles(pathIn);
                foreach (string pdf in pdfsIn)
                {
                    PdfDocument inPdf = PdfReader.Open(pdf, PdfDocumentOpenMode.Import);
                    Tools.CopyPages(inPdf, outPdf);
                }

                outPdf.Save(fullFileName);
            }

            return 0;
        }
    }
}
