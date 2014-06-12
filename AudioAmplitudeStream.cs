using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace IoT.WinRT
{
    class AudioAmplitudeStream : IRandomAccessStream
    {
        public bool CanRead
        {
            get { return false; }
        }

        public bool CanWrite
        {
            get { return true; }
        }

        public IRandomAccessStream CloneStream()
        {
            throw new NotImplementedException();
        }

        public IInputStream GetInputStreamAt(ulong position)
        {
            throw new NotImplementedException();
        }

        public IOutputStream GetOutputStreamAt(ulong position)
        {
            throw new NotImplementedException();
        }

        public ulong Position
        {
            get { return 0; }
        }

        public void Seek(ulong position)
        {

        }

        public ulong Size
        {
            get
            {
                return 0;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {

        }

        public Windows.Foundation.IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            throw new NotImplementedException();
        }

        public Windows.Foundation.IAsyncOperation<bool> FlushAsync()
        {
            return AsyncInfo.Run<bool>(_ => Task.Run(() => true));
        }

        public Windows.Foundation.IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
        {

            return AsyncInfo.Run<uint, uint>((token, progress) =>
            {
                return Task.Run(() =>
                {
                    using (var memoryStream = new MemoryStream())
                    using (var outputStream = memoryStream.AsOutputStream())
                    {
                        outputStream.WriteAsync(buffer).AsTask().Wait();

                        var byteArray = memoryStream.ToArray();
                        var amplitude = Decode(byteArray).Select(Math.Abs).Average(x => x);

                        if (AmplitudeReading != null) this.AmplitudeReading(this, amplitude);

                        progress.Report((uint)memoryStream.Length);
                        return (uint)memoryStream.Length;
                    }
                });
            });
        }

        private IEnumerable<Int16> Decode(byte[] byteArray)
        {
            for (var i = 0; i < byteArray.Length - 1; i += 2)
            {
                yield return (BitConverter.ToInt16(byteArray, i));
            }
        }

        public delegate void AmplitudeReadingEventHandler(object sender, double reading);

        public event AmplitudeReadingEventHandler AmplitudeReading;

    }

}
