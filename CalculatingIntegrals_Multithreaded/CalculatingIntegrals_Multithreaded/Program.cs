using System.Diagnostics;
using System.Net.Sockets;
using System.Numerics;

Stopwatch sw = new Stopwatch();
double output = 0;
object locker = new object();
const int THREAD_NUMBER = 4;

Thread startThread(double a, double b, double dx, int startIndex, int endIndex)
{
    Thread t = new Thread(() => simpson(a, b, dx, startIndex, endIndex));
    t.Start();
    return t;
}

void simpson(double a, double b, double dx, int startIndex, int endIndex)
{
    int multiplier;
    double addition = 0;
    for (int i = startIndex; i <= endIndex; i++)
    {
        if (i % 2 == 0) { multiplier = 2; }
        else { multiplier = 4; }
        addition += multiplier * f(a + (double)i * dx);
    }
    lock (locker) { output += addition; }
}

double f(double x) { return ((3 * x) - 1) / ((3 * x) + 1); }

double F(double a, double b, int n)
{
    double dx = (b - a) / (double)(n - 1);
    output = f(a) + f(b);

    int fraction = n / THREAD_NUMBER;
    Thread[] Threads = new Thread[THREAD_NUMBER];
    for (int i = 0; i < THREAD_NUMBER; i++)
    {
        int start = i * fraction + 1;
        int end = (i + 1) * fraction;
        Threads[i] = startThread(a, b, dx, start, end);
    }
    for (int i = 0; i < THREAD_NUMBER; i++) { Threads[i].Join(); }
    output *= dx / 3.0;
    return output;
}

double Integral(double a, double b, double accuracy)
{
    // Start time counting here
    sw.Start();
    if (a == b) { return 0; }
    int n = (int)(Math.Abs(b - a) * 2);
    if (n <= 100) { n = 101; }
    else if (n % 2 == 0) { n++; }

    double integral = F(a, b, n);
    double nextIntegral = F(a, b, 2 * n);
    n *= 4;
    while (Math.Abs(integral - nextIntegral) > accuracy)
    {
        integral = nextIntegral;
        nextIntegral = F(a, b, n);
        Console.WriteLine("1st integral: " + integral + "\n2nd integral: " + nextIntegral);
        Console.WriteLine("Difference: " + Math.Abs(integral - nextIntegral) + "\n\n");
        n *= 2;
    }
    // Finish time counting here
    sw.Stop();
    return nextIntegral;
}

double precision = 0.00000001;
double Integ = Integral(2, 8, precision);
Console.WriteLine("Calculated Integral:\t\t" + Integ);
Console.WriteLine("Threads used: \t\t\t" + THREAD_NUMBER);
float time = (float)(sw.ElapsedMilliseconds) / 1000;
Console.WriteLine("Time elapsed: \t\t\t" + time + " s");