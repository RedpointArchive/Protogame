using System;

namespace ProtogameEditor
{
    public class IDEEditor
    {
        public void Init()
        {
            Console.WriteLine("This is from the editor assembly");
        }

        public string[] GetHandledFileExtensions()
        {
            return new[]
            {
                ".test"
            };
        }
    }
}

