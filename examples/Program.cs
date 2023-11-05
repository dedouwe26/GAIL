using dedouwesGAIL;

namespace GAILExamples
{
    class Program
    {
        public static void Main(string[] args) {
            GAIL g = new("hello");
            Console.WriteLine(g.s);
        }
    }
}