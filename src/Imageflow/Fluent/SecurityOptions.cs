using System.Text.Json.Nodes;

namespace Imageflow.Fluent;

public class SecurityOptions
{

    public FrameSizeLimit? MaxDecodeSize { get; set; }

    public FrameSizeLimit? MaxFrameSize { get; set; }

    public FrameSizeLimit? MaxEncodeSize { get; set; }

    /// <summary>
    /// Processing timeout in milliseconds. Operations exceeding this are cancelled.
    /// Default in libimageflow: 30000 (30 seconds). null = no timeout.
    /// Requires ABI 3.2+.
    /// </summary>
    public ulong? ProcessTimeoutMs { get; set; }

    /// <summary>
    /// Maximum threads for parallel encoding operations.
    /// 1 disables parallelism. null = codec default (typically auto-detect cores).
    /// Requires ABI 3.2+.
    /// </summary>
    public uint? MaxEncoderThreads { get; set; }

    /// <summary>
    /// Controls which image formats are enabled for decoding.
    /// Requires ABI 3.2+.
    /// </summary>
    public DecodeFormatConfig? DecodeFormats { get; set; }

    /// <summary>
    /// Controls which image formats are enabled for encoding.
    /// Requires ABI 3.2+.
    /// </summary>
    public EncodeFormatConfig? EncodeFormats { get; set; }

    /// <summary>
    /// Controls which codec implementations are used and their priority.
    /// Requires ABI 3.2+.
    /// </summary>
    public CodecConfig? Codecs { get; set; }

    public SecurityOptions SetMaxDecodeSize(FrameSizeLimit? limit)
    {
        MaxDecodeSize = limit;
        return this;
    }
    public SecurityOptions SetMaxFrameSize(FrameSizeLimit? limit)
    {
        MaxFrameSize = limit;
        return this;
    }
    public SecurityOptions SetMaxEncodeSize(FrameSizeLimit? limit)
    {
        MaxEncodeSize = limit;
        return this;
    }
    public SecurityOptions SetProcessTimeoutMs(ulong? timeoutMs)
    {
        ProcessTimeoutMs = timeoutMs;
        return this;
    }
    public SecurityOptions SetMaxEncoderThreads(uint? maxThreads)
    {
        MaxEncoderThreads = maxThreads;
        return this;
    }
    public SecurityOptions SetDecodeFormats(DecodeFormatConfig? config)
    {
        DecodeFormats = config;
        return this;
    }
    public SecurityOptions SetEncodeFormats(EncodeFormatConfig? config)
    {
        EncodeFormats = config;
        return this;
    }
    public SecurityOptions SetCodecs(CodecConfig? config)
    {
        Codecs = config;
        return this;
    }

    [Obsolete("Use ToJsonNode() instead")]
    internal object ToImageflowDynamic()
    {
        return new
        {
            max_decode_size = MaxDecodeSize?.ToImageflowDynamic(),
            max_frame_size = MaxFrameSize?.ToImageflowDynamic(),
            max_encode_size = MaxEncodeSize?.ToImageflowDynamic()
        };
    }

    internal JsonNode ToJsonNode()
    {
        var node = new JsonObject();
        if (MaxDecodeSize != null)
            node.Add("max_decode_size", MaxDecodeSize?.ToJsonNode());
        if (MaxFrameSize != null)
            node.Add("max_frame_size", MaxFrameSize?.ToJsonNode());
        if (MaxEncodeSize != null)
            node.Add("max_encode_size", MaxEncodeSize?.ToJsonNode());
        if (ProcessTimeoutMs != null)
            node.Add("process_timeout_ms", ProcessTimeoutMs);
        if (MaxEncoderThreads != null)
            node.Add("max_encoder_threads", MaxEncoderThreads);
        if (DecodeFormats != null)
            node.Add("decode_formats", DecodeFormats.ToJsonNode());
        if (EncodeFormats != null)
            node.Add("encode_formats", EncodeFormats.ToJsonNode());
        if (Codecs != null)
            node.Add("codecs", Codecs.ToJsonNode());
        return node;
    }
}
