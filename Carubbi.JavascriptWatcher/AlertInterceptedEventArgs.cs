using System;

namespace Carubbi.JavascriptWatcher
{
    /// <inheritdoc />
    /// <summary>
    /// Argumentos enviados no evento de interceptação de alert javascript
    /// </summary>
    public class AlertInterceptedEventArgs : EventArgs
    {
        /// <summary>
        /// Texto do alert interceptado
        /// </summary>
        public string AlertText { get; set; }
    }
}
