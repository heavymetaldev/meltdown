using System.Text;

namespace Meltdown.UI;

public class ProgressWriter(IProgressReporter progress) : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;
    private StringBuilder lineBuffer = new StringBuilder();
    public override void Write(char value)
    {
        switch (value)
        {
            case '\r':
                // Ignore carriage return
                return;
            case '\n':
                progress.Log("console", lineBuffer.ToString()).GetAwaiter().GetResult();
                lineBuffer.Clear();
                break;
            default:
                lineBuffer.Append(value);
                break;
        }
    }
}
