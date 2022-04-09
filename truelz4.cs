// Reference: K4os.Compression.LZ4
// Reference: K4os.Compression.LZ4.Streams
// Reference: K4os.Compression.LZ4.Legacy
// Reference: K4os.Hash.xxHash
using System.Diagnostics;
using System.IO;
using K4os.Compression.LZ4.Legacy;
using K4os.Compression.LZ4.Streams;
using Oxide.Core.Libraries.Covalence;
using LZ4Stream = K4os.Compression.LZ4.Streams.LZ4Stream;

namespace Oxide.Plugins
{
    [Info("truelz4", "rkkm", "1.0")]
    [Description("This plugin produces compressed variant of the map that is compatible with official LZ4 Frame Format ")]
    public class truelz4 : RustPlugin
    {
        private void Init()
        {
            Puts("truelz4 is enabled!");
           
        }
        
        [ConsoleCommand("runcompression")]
        private void RunCompressionMigration(ConsoleSystem.Arg arg)
        {
            try
            {
                string folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"/server/rust";
                string[] files = Directory.GetFiles(folder, "*.map");
                var fileName = files[0];
                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Puts("Found map file named " + fileName);
                    Puts("Trying to read it...");
                    using (var binaryReader = new BinaryReader(fileStream))
                    {
                        binaryReader.ReadUInt32();
                        Puts("Success!");
                        var settings = new LZ4EncoderSettings();
                        settings.ChainBlocks = false;
                        using (var source = LZ4Legacy.Decode(fileStream))
                        using (var fileStreamOut = LZ4Stream.Encode(new FileStream(fileName + ".truelz4", FileMode.Create, FileAccess.ReadWrite, FileShare.Read), settings))
                        {
                            Puts("Coping from old file to the new one...");
                            source.CopyTo(fileStreamOut);
                            Puts("Done!");
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Puts(e.Message);
            }
        }
        
    }
    
}