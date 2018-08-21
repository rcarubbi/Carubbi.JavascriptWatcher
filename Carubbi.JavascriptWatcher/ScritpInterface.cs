using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Carubbi.JavascriptWatcher
{
    public partial class JavascriptWatcher
    {
        /// <inheritdoc />
        /// <summary>
        /// Classe com os métodos de comunicação entre o javascript e o .net
        /// </summary>
        [ComVisible(true)]
        public class ScriptInterface : WebBrowser
        {
            private readonly JavascriptWatcher _context;

            private readonly WebBrowser _webBrowser;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="text"></param>
            /// <returns></returns>
            public bool InterceptConfirm(string text)
            {
                if (_context.ConfirmIntercepted == null) return false;
                var ea = new ConfirmInterceptedEventArgs() { Text = text };
                _context.ConfirmIntercepted(_webBrowser, ea);
                return ea.Result;
            }

            /// <inheritdoc />
            /// <summary>
            /// </summary>
            /// <param name="context"></param>
            /// <param name="webBrowser"></param>
            public ScriptInterface(JavascriptWatcher context, WebBrowser webBrowser)
            {
                _context = context;
                _webBrowser = webBrowser;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="windowToBeOpened"></param>
            public void InterceptWindowOpen(string windowToBeOpened)
            {
                if (_context.WindowOpenIntercepted == null) return;
                if (string.IsNullOrEmpty(windowToBeOpened))
                {
                    windowToBeOpened = "URL não informada";
                }
               
                _context.WindowOpenIntercepted(_webBrowser, new WindowOpenInterceptedEventArgs() { Url = new Uri(windowToBeOpened, UriKind.Relative) });
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="text"></param>
            public void InterceptAlerts(string text)
            {
                _context.AlertIntercepted?.Invoke(_webBrowser, new AlertInterceptedEventArgs() { AlertText = text });
            }
        }
    }
}
