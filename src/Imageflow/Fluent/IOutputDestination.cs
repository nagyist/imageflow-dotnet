﻿using Imageflow.Bindings;

namespace Imageflow.Fluent
{
    public interface IOutputDestination : IDisposable
    {
        Task RequestCapacityAsync(int bytes);
        Task WriteAsync(ArraySegment<byte> bytes, CancellationToken cancellationToken);
        Task FlushAsync(CancellationToken cancellationToken);
    }

    // ReSharper disable once InconsistentNaming
    public static class IOutputDestinationExtensions
    {
        public static async Task CopyFromStreamAsync(this IOutputDestination dest, Stream stream,
            CancellationToken cancellationToken)
        {
            if (stream is { CanRead: true, CanSeek: true })
            {
                await dest.RequestCapacityAsync((int) stream.Length);
            }

            const int bufferSize = 81920;
            var buffer = new byte[bufferSize];

            int bytesRead;
            while ((bytesRead =
                       await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await dest.WriteAsync(new ArraySegment<byte>(buffer, 0, bytesRead), cancellationToken)
                    .ConfigureAwait(false);
            }

            await dest.FlushAsync(cancellationToken);
        }
    }

    public class BytesDestination : IOutputDestination
    {
        private MemoryStream? _m;
        public void Dispose()
        {
        }

        public Task RequestCapacityAsync(int bytes)
        {
            _m ??= new MemoryStream(bytes);
            
            if (_m.Capacity < bytes) _m.Capacity = bytes;
            return Task.CompletedTask;
        }

        public Task WriteAsync(ArraySegment<byte> bytes, CancellationToken cancellationToken)
        {
            if (_m == null) throw new ImageflowAssertionFailed("BytesDestination.WriteAsync called before RequestCapacityAsync");
            if (bytes.Array == null) throw new ImageflowAssertionFailed("BytesDestination.WriteAsync called with null array");
            return _m.WriteAsync(bytes.Array, bytes.Offset, bytes.Count, cancellationToken);
        }

        public Task FlushAsync(CancellationToken cancellationToken)
        {
            if (_m == null) throw new ImageflowAssertionFailed("BytesDestination.FlushAsync called before RequestCapacityAsync");
            return _m.FlushAsync(cancellationToken);
        }
        
        public ArraySegment<byte> GetBytes()
        {
            if (_m == null) throw new ImageflowAssertionFailed("BytesDestination.GetBytes called before RequestCapacityAsync");
            if (!_m.TryGetBuffer(out var bytes))
            {
                throw new ImageflowAssertionFailed("MemoryStream TryGetBuffer should not fail here");
            }
            return bytes;
        }
    }

    public class StreamDestination(Stream underlying, bool disposeUnderlying) : IOutputDestination
    {
        public void Dispose()
        {
            if (disposeUnderlying) underlying?.Dispose();
        }

        public Task RequestCapacityAsync(int bytes)
        {
            if (underlying is { CanSeek: true, CanWrite: true }) underlying.SetLength(bytes);
            return Task.CompletedTask;
        }

        public Task WriteAsync(ArraySegment<byte> bytes, CancellationToken cancellationToken)
        {
            if (bytes.Array == null) throw new ImageflowAssertionFailed("StreamDestination.WriteAsync called with null array");
            return underlying.WriteAsync(bytes.Array, bytes.Offset, bytes.Count, cancellationToken);
        }

        public Task FlushAsync(CancellationToken cancellationToken)
        {
            if (underlying is { CanSeek: true, CanWrite: true } 
                && underlying.Position < underlying.Length)
                underlying.SetLength(underlying.Position);
            return underlying.FlushAsync(cancellationToken);
        }
    }
}