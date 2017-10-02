using ConsultaEleitoral.Class;
using Novacode;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsultaEleitoral
{
    public partial class Form1 : Form
    {
        // incializa componetes
        // webbrowser ScriptErrorsSuppressed true;
        public Form1()
        {
            InitializeComponent();
        }

        // inicia requisição 
        private void btnRequest_Click(object sender, EventArgs e)
        {
            webBrowser.Navigate(urlTextBox.Text);            
        }
        
        // captura de imagem
        private void Captura_Imagem()
        {
            try
            {
                // captura imagem
                System.Drawing.Image captura = Tela.RetornaImagemControle(this.Handle, new Rectangle(0, 0, this.Width, this.Height));

                // salva imagem
                // modificar pasta na sua maquina
                captura.Save(@"e:\capturaConsulta.bmp", ImageFormat.Bmp);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            // gera arquivo
            MessageBox.Show("Gerar Arquivo?");
            Gera_Arquivo();

        }

        // gerar arquivo
        // Referencia: https://www.codeproject.com/Articles/660478/Csharp-Create-and-Manipulate-Word-Documents-Progra
        // Referencia: https://stackoverflow.com/questions/38658848/c-sharp-net-docx-add-an-image-to-a-docx-file
        // Install-Package DocX -Version 1.0.0.22
        private void Gera_Arquivo()
        {
            var eleitor = new Eleitor
            {
                nome = "Flavio de Assis Santos",
                data = "03/05/1980"
            };

            try
            {
                // modificar pasta na sua maquina
                string caminhoImage = @"e:\capturaConsulta.bmp";
                string caminhoPasta = @"e:\Temp\" + eleitor.nome.Replace(" ", "_") + "_" + eleitor.data.Replace("/", "_") + ".docx";

                // gera arquivo na memoria
                var doc = DocX.Create(caminhoPasta);

                // insere imagem no arquivo
                Novacode.Image image = doc.AddImage(caminhoImage);
                Picture picture = image.CreatePicture();
                Paragraph p1 = doc.InsertParagraph();
                p1.AppendPicture(picture);

                doc.Save();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // webBrowser document
        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var currentWindow = webBrowser.Document;

            var eleitor = new Eleitor
            {
                nome = "Flavio de Assis Santos",
                data = "03/05/1980"
            };

            // verifica se existe frame do documento 
            if (currentWindow.Window.Frames.Count > 0)
            {
                // busca elementos em cada frame
                foreach (HtmlWindow frame in currentWindow.Window.Frames)
                {
                    try
                    {
                        var inputs = (currentWindow.GetElementsByTagName("input"));

                        //loop lista elemento inputs
                        for (int i = 0; i < inputs.Count; i++)
                        {
                            if (inputs[i].Name == "nomeEleitor") //unico elemento com nome "nomeEleitor"
                            {
                                inputs[i].SetAttribute("value", eleitor.nome);
                            }
                            else if (inputs[i].Name == "dataNascimento") //unico elemento com nome "vb_login_username"
                            {
                                inputs[i].SetAttribute("value", eleitor.data);
                            }
                        }

                        for (int i = 0; i < inputs.Count; i++)
                        {
                            if (inputs[i].GetAttribute("value") == "Consultar") //unico elemento com nome "Consultar"
                            {
                                inputs[i].InvokeMember("click");
                                
                                // captura Imagem
                                MessageBox.Show("Capturar Imagem?");
                                Captura_Imagem();

                                break;
                            }
                        }

                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
            else
            {
                try
                {
                    var inputs = (currentWindow.GetElementsByTagName("input"));

                    //loop lista elemento inputs
                    for (int i = 0; i < inputs.Count; i++)
                    {
                        if (inputs[i].Name == "nomeEleitor") //unico elemento com nome "nomeEleitor"
                        {
                            inputs[i].SetAttribute("value", eleitor.nome);
                        }
                        else if (inputs[i].Name == "dataNascimento") //unico elemento com nome "vb_login_username"
                        {
                            inputs[i].SetAttribute("value", eleitor.data);
                        }
                    }

                    for (int i = 0; i < inputs.Count; i++)
                    {
                        if (inputs[i].GetAttribute("value") == "Consultar") //unico elemento com nome "Consultar"
                        {
                            inputs[i].InvokeMember("click");

                            // captura Imagem
                            MessageBox.Show("Capturar Imagem?");
                            Captura_Imagem();

                            break;
                        }
                    }

                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }

            /*
            // TODO: Analisar System.UnauthorizedAccessException
            // https://stackoverflow.com/questions/21839674/access-in-iframe-and-setattribute-value
            // https://social.msdn.microsoft.com/Forums/en-US/acc5a3e5-2406-4344-ad8d-6b816ad17d64/webbrowser-iframe?forum=vblanguage
            // verifica se existe frames no documento 
            //if (currentWindow.Window.Frames.Count > 0)
            //{
            //    HtmlWindowCollection iframes = currentWindow.Window.Frames;

            //    foreach(HtmlWindow frame in iframes)
            //    {
            //        for (int i = 0; i < iframes.Count; i++)
            //        {
            //            HtmlElementCollection inputs = iframes[i].Document.GetElementsByTagName("input");

            //            //loop lista elemento inputs
            //            for (int j = 0; j < inputs.Count; j++)
            //            {
            //                if (inputs[j].Name == "nomeEleitor") //unico elemento com nome "nomeEleitor"
            //                {
            //                    inputs[j].SetAttribute("value", eleitor.nome);
            //                }
            //                else if (inputs[j].Name == "dataNascimento") //unico elemento com nome "vb_login_username"
            //                {
            //                    inputs[j].SetAttribute("value", eleitor.data);
            //                }

            //            }
            //        }

            //    }

            //}
            */

        }
    }

}

/*
 // Outros metodos            
 
    HtmlWindow currentWindow = webBrowser.Document.Window;

            foreach (HtmlWindow frame in currentWindow.Frames)
            {
                if (frame.Name == "windowZ")
                {
                    HtmlElementCollection inputs = frame.Document.GetElementsByTagName("input");

                    foreach (HtmlElement curElement in inputs)
                    {
                        string controlName = curElement.GetAttribute("Name").ToString();

                        if (controlName == "nomeEleitor")
                        {
                            curElement.SetAttribute("value", eleitor.nome);
                        }
                        else if (controlName == "dataNascimento")
                        {
                            curElement.SetAttribute("value", eleitor.data);
                        }                        
                    }
                }
            }
 // inspeciona o documento  
        private void Documento_WebBrowser(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var currentWindow = ((WebBrowser)sender).Document.Window;
            
            // limpa para evitar loop no sender 
            ((WebBrowser)sender).DocumentCompleted -= Documento_WebBrowser;
            
            var eleitor = new Eleitor
            {
                nome = "Flavio de Assis Santos",
                data = "03/05/1980"
            };

            try
            {
                var inputs = (currentWindow.Document.GetElementsByTagName("input"));

                //loop lista elemento inputs
                for (int i = 0; i < inputs.Count; i++)
                {
                    if (inputs[i].Name == "nomeEleitor") //unico elemento com nome "nomeEleitor"
                    {
                        inputs[i].SetAttribute("value", eleitor.nome);
                    }
                    else if (inputs[i].Name == "dataNascimento") //unico elemento com nome "vb_login_username"
                    {
                        inputs[i].SetAttribute("value", eleitor.data);
                    }
                }

                for (int i = 0; i < inputs.Count; i++)
                {
                    if (inputs[i].GetAttribute("value") == "Consultar") //unico elemento com nome "Consultar"
                    {
                        inputs[i].InvokeMember("click");
                        break;
                    }
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        // trata url da box
        private void Navigate(String address)
        {
            if (String.IsNullOrEmpty(address) || address.Equals("about:blank"))
            {
                MessageBox.Show("Informe o endereço");
            }
            else
            {
                if (!address.StartsWith("http://") &&
                !address.StartsWith("https://"))
                {
                    address = "http://" + address;
                }
                try
                {
                    webBrowser.Navigate(new Uri(address));           

                }
                catch (System.UriFormatException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

*/
