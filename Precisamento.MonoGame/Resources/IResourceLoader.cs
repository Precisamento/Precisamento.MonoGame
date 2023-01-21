using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Resources
{
    public interface IResourceLoader : IDisposable
    {
        T Load<T>(string name);
        void Unload();
    }
}
