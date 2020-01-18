namespace NRKernal.Record.Tool
{
    public interface IImageEncoder
    {
        void Commit(FrameData frame);

        void Stop();
    }
}
