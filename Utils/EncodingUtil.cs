using System.Runtime.InteropServices;
using System.Text;

namespace English.Utils;

public class EncodingUtil
{
    public void EncodingSetup()
    {
        // Скрываем курсор, чтобы он не бегал по экрану при перерисовке
        Console.CursorVisible = false;
        Console.OutputEncoding = Encoding.UTF8;   // для виводу
            
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Console.InputEncoding  = Encoding.Unicode;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            Console.InputEncoding  = Encoding.UTF8;
    }
}