// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

Console.WriteLine("Hello, World!");

string filepath = @"C:\Users\Theo\Desktop\Repo\Pathtracer\Plots\Plots\Plots\Plots.py";
string arguments = "";

ProcessStartInfo start = new ProcessStartInfo();
start.FileName = "my/full/path/to/python.exe";
start.Arguments = string.Format("{0} {1}", filepath, arguments);
start.UseShellExecute = false;
start.RedirectStandardOutput = true;
using (Process? process = Process.Start(start)) {
    if (process == null) throw new InvalidOperationException();
    using StreamReader reader = process.StandardOutput;
    string result = reader.ReadToEnd();
    Console.Write(result);
}
