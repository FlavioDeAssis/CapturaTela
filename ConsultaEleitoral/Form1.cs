using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ConsultaEleitoral
{
    public partial class Form1 : Form
    {
        public Form1()
        {   
            // incializa componetes
            InitializeComponent();
        }
        
        // inicia requisisição do conteudo do site utilizando class WebBrowser 
        private void btnRequest_Click(object sender, EventArgs e)
        {
            Navigate(urlTextBox.Text);
        }

        // tratamento da url inserida na box de UL
        private void Navigate(String address)
        {
            if (String.IsNullOrEmpty(address)) return;
            if (address.Equals("about:blank")) return;
            if (!address.StartsWith("http://") &&
                !address.StartsWith("https://"))
            {
                address = "http://" + address;
            }
            try
            {
                webBrowser.Navigate(new Uri(address));
            }
            catch (System.UriFormatException)
            {
                return;
            }
        }

        // gerar a captura de imagem. salva imagem e arquivo
        // obs: alterar letra do diretorio de destino
        private void btnSave_Click(object sender, EventArgs e)
        {
            // captura imagem
            pictureBox.Image = Tela.RetornaImagemControle(this.Handle, new Rectangle(0, 0, this.Width, this.Height));
            // salva imagem
            pictureBox.Image.Save(@"e:\teste.bmp", ImageFormat.Bmp);

            // gerar arquivo word
            // Referencia: http://www.andrealveslima.com.br/blog/index.php/2016/06/29/gerando-arquivos-do-word-com-c-e-vb-net/
            // obs: talvez seja necessario obter o pacote atraves do gerenciado 
            var wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = false;
            var wordDoc = wordApp.Documents.Add();
            
            // adicionando imagem ao arquivo
            var paragrafo5 = wordDoc.Paragraphs.Add();
            paragrafo5.Range.InlineShapes.AddPicture(@"e:\teste.bmp");

            // salva arquivo
            wordDoc.SaveAs2(@"e:\" + txtNomeEleitor.Text + "_" + txtDataNascimento.Text +".docx");
            wordApp.DisplayAlerts = Microsoft.Office.Interop.Word.WdAlertLevel.wdAlertsNone;
            wordApp.Quit();
        }
        
    }

    // classe para gerar imagem
    //Referencia: https://msdn.microsoft.com/pt-br/library/dn630211.aspx
    public class Tela
    {
        [DllImport("user32.dll", EntryPoint = "GetDC")]
        static extern IntPtr GetDC(IntPtr ptr);
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        static extern IntPtr DeleteDC(IntPtr hDc);
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        static extern IntPtr GetDesktopWindow();
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        static extern IntPtr DeleteObject(IntPtr hDc);

        const int SRCCOPY = 13369376;

        public struct SIZE
        {
            public int cx;
            public int cy;
            
        }

        public static Bitmap RetornaImagemControle(IntPtr controle, Rectangle area)
        {
            SIZE size;
            IntPtr hBitmap;

            IntPtr hDC = GetDC(controle);
            IntPtr hMemDC = CreateCompatibleDC(hDC);

            size.cx = area.Width - area.Left;
            size.cy = area.Bottom - area.Top;

            hBitmap = CreateCompatibleBitmap(hDC, size.cx, size.cy);

            if (hBitmap != IntPtr.Zero)
            {
                IntPtr hOld = (IntPtr)SelectObject(hMemDC, hBitmap);
                BitBlt(hMemDC, 0, 0, size.cx, size.cy, hDC, 0, 0, SRCCOPY);
                SelectObject(hMemDC, hOld);
                DeleteDC(hMemDC);
                ReleaseDC(GetDesktopWindow(), hDC);
                Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap);
                DeleteObject(hBitmap);
                return bmp;
            }
            else
            {
                return null;
            }
        }

    }
}
