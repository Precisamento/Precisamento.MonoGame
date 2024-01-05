using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI
{
    public class GuiException : Exception
    {
        public GuiException()
        {
        }

        public GuiException(string? message) : base(message)
        {
        }

        public GuiException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected GuiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
