namespace Firefly.CrossPlatformZip.Console
{
    using System;

    class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Usage();
                return 1;
            }

            var zipFile = args[0];
            var path = args[1];

            try
            {
                if (args.Length >= 3)
                {
                    var target = (ZipPlatform)Enum.Parse(typeof(ZipPlatform), args[2], true);

                    Zipper.Zip(zipFile, path, 9, target);
                }
                else
                {
                    Zipper.Zip(zipFile, path, 9);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }

            return 0;
        }

        private static void Usage()
        {
            Console.WriteLine("Usage");
            Console.WriteLine("CrossPlatformZip zipFile fileOrDirectory [windows/unix]");
        }
    }
}