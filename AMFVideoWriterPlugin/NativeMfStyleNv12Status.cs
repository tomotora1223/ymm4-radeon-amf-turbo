using System.Runtime.InteropServices;

namespace AMFVideoWriterPlugin;

[StructLayout(LayoutKind.Sequential)]
internal struct NativeMfStyleNv12Status
{
    public uint Version, Requested, Active, SourceDxgiFormat;
    public uint EncoderSurfaceFormat, GpuOperationsPerFrame;
    public ulong DirectConversions;
    public ulong Conversions, Failures;
    public NativeProfileMetric ConversionCpu;

    public object ToReport()
    {
        if (Version is not (1 or 2) || Requested > 1 || Active > 1 || Active > Requested
            || (Active == 1 ? GpuOperationsPerFrame is not (1 or 2) : GpuOperationsPerFrame != 0)
            || DirectConversions > Conversions || (Version == 1 && DirectConversions != 0))
            throw new InvalidOperationException("MF方式NV12変換のDLLバージョンまたは状態が不正です。");
        return new
        {
            requested = Requested != 0,
            active = Active != 0,
            path = Active != 0 ? "d3d11_videoprocessor_nv12_then_amf" : "amf_bgra_rgba",
            source_dxgi_format = SourceDxgiFormat,
            encoder_surface_format = EncoderSurfaceFormat,
            gpu_operations_per_frame = GpuOperationsPerFrame,
            conversions = Conversions,
            direct_conversions = DirectConversions,
            rgb_staging_copies = Conversions - DirectConversions,
            failures = Failures,
            conversion_cpu = new
            {
                count = ConversionCpu.Count,
                total_ms = ConversionCpu.TotalNanoseconds / 1_000_000d,
                mean_ms = ConversionCpu.Count == 0 ? 0 : ConversionCpu.TotalNanoseconds / 1_000_000d / ConversionCpu.Count,
                max_ms = ConversionCpu.MaxNanoseconds / 1_000_000d,
                measurement = "cpu_command_wall_time_not_gpu_timestamp",
            },
        };
    }
}
