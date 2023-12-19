using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;
using Precisamento.MonoGame.Dialogue;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Precisamento.MonoGame.Resources
{
    public class ResourceLoader : IResourceLoader
    {
        private static class Loader<T>
        {
            public static ResourceTypeReader<T> Reader;
        }

        public static void RegisterLoader<T>(ResourceTypeReader<T> reader)
        {
            Loader<T>.Reader = reader;
        }

        static ResourceLoader()
        {
            RegisterLoader<Sprite>(new SpriteReader());
            RegisterLoader<DialogueData>(new DialogueReader());
        }

        private Dictionary<string, object> _loadedResources = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        private List<IDisposable> _disposableResources = new List<IDisposable>();
        private bool _disposed;

        public ContentManager Content { get; }

        public ResourceLoader(IServiceProvider serviceProvider)
        {
            Content = new ContentManager(serviceProvider);
        }

        public ResourceLoader(IServiceProvider serviceProvider, string rootDirectory)
        {
            Content = new ContentManager(serviceProvider, rootDirectory);
        }

        ~ResourceLoader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public T Load<T>(string name)
        {
            if (Loader<T>.Reader != null)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name));

                if (_disposed)
                    throw new ObjectDisposedException("ResourceLoader");

                name = name.Replace('\\', '/');

                if (_loadedResources.TryGetValue(name, out var asset) && asset is T)
                {
                    return (T)asset;
                }

                T result = ReadResource<T>(name) ?? throw new InvalidOperationException($"Failed to read resource {name}");

                _loadedResources[name] = result;

                if(result is IDisposable disposable)
                {
                    if (!_disposableResources.Contains(disposable))
                        _disposableResources.Add(disposable);
                }

                return result;
            }
            else
            {
                // Default to content loader if no ResourceTypeReader has been registered for <T>.
                return Content.Load<T>(name);
            }
        }

        private T ReadResource<T>(string name)
        {
            name = Path.Combine(Content.RootDirectory, name) + Loader<T>.Reader.FileExtension;

            Stream stream;
            try
            {
                if (Path.IsPathRooted(name))
                    stream = File.OpenRead(name);
                else
                    stream = TitleContainer.OpenStream(name);

                // TODO: Implement Android speed increase based on MonoGame source.

                using var reader = new ResourceReader(this, stream);
                return Loader<T>.Reader.Read(reader);
            }
            catch (FileNotFoundException fileNotFound)
            {
                throw new ContentLoadException("The resource file was not found.", fileNotFound);
            }
            catch (Exception exception)
            {
                throw new ContentLoadException("Failed to read resource file.", exception);
            }
        }

        public void Unload()
        {
            Content.Unload();

            foreach (var disposable in _disposableResources)
                disposable.Dispose();

            _disposableResources.Clear();
            _loadedResources.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!_disposed)
            {
                if(disposing)
                {
                    Unload();
                    Content.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
