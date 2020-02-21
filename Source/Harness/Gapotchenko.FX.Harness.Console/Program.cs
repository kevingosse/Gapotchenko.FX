﻿#region Usings
using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.ComponentModel;
using Gapotchenko.FX.Data.Encoding;
using Gapotchenko.FX.Diagnostics;
using Gapotchenko.FX.IO;
using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Linq.Expressions;
using Gapotchenko.FX.Math;
using Gapotchenko.FX.Text;
using Gapotchenko.FX.Threading;
using Gapotchenko.FX.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Gapotchenko.FX.Harness.Console
{
    using Console = System.Console;
    using Math = System.Math;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                _Run();
                TaskBridge.Execute(_RunAsync);

                Console.WriteLine();
                Console.WriteLine("----------------------");
                Console.WriteLine("DONE");
            }
            catch (Exception e)
            {
                Console.Write("Error: ");
                Console.WriteLine(e);
            }
        }

        static void _Run()
        {
            Console.WriteLine("Process: {0}", RuntimeInformation.ProcessArchitecture);
            Console.WriteLine("OS: {0}", RuntimeInformation.OSArchitecture);

            Console.WriteLine(BitOperations.Log2(32));

            IEnumerable<int> source = new[] { 1, 2, 3 };

            var h = source.ToHashSet();

            Console.WriteLine(h.IsNullOrEmpty());
        }

        static async Task _RunAsync(CancellationToken ct)
        {
            await Task.Yield();

            string s = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec suscipit, lectus et dapibus ultricies, sem nulla finibus dolor, vitae pharetra urna risus eget nunc. Nunc laoreet condimentum magna, a varius massa auctor in. Mauris cursus sodales justo eget faucibus. Nullam nec nisi eget lorem faucibus feugiat. Fusce sed iaculis turpis, ut vestibulum ipsum.";

            string filePath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Temp\base.txt");

            //var stream = Base64.Instance.CreateEncoder(
            //    File.CreateText(filePath),
            //    DataEncodingOptions.Indent);
            //try
            //{
            //    await stream.WriteAsync(Encoding.UTF8.GetBytes(s));
            //    await stream.WriteAsync(Encoding.UTF8.GetBytes(s));
            //    await stream.WriteAsync(Encoding.UTF8.GetBytes(s));
            //    await stream.WriteAsync(Encoding.UTF8.GetBytes(s));
            //    await stream.WriteAsync(Encoding.UTF8.GetBytes(s));
            //    await stream.WriteAsync(Encoding.UTF8.GetBytes(s));
            //    await stream.WriteAsync(Encoding.UTF8.GetBytes("h"));
            //}
            //finally
            //{
            //    await stream.DisposeAsync();
            //}

            using (var tr = new StreamReader(
                Base64.Instance.CreateDecoder(
                    File.OpenText(filePath),
                    DataEncodingOptions.Padding)))
            {
                Console.WriteLine(await tr.ReadLineAsync());
            }

            Console.WriteLine(Encoding.UTF8.GetString(Base64.GetBytes("SQ=Ж=QU0=VEpN", DataEncodingOptions.Padding | DataEncodingOptions.Relax)));

            //string e = Base64.GetString(Encoding.UTF8.GetBytes(s), DataEncodingOptions.Indent);

            //e = Convert.ToBase64String(Encoding.UTF8.GetBytes(s), Base64FormattingOptions.InsertLineBreaks);

            //Console.WriteLine(e);
        }
    }
}
