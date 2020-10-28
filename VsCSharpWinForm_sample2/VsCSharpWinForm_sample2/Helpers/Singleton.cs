using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace VsCSharpWinForm_sample2.Helpers
{
    public class Singleton
    {
        /// Updated date: 2020-09-04
        /// https://www.codeproject.com/Articles/430590/Design-Patterns-1-of-3-Creational-Design-Patterns#Singleton
        /// https://www.dofactory.com/net/singleton-design-pattern
        /// https://en.wikipedia.org/wiki/Singleton_pattern
        private static volatile Singleton _instance;
        private static readonly object _instanceLocker = new object();
        private Singleton() { }
        public static Singleton GetInstance()
        {
            if (_instance == null)
            {
                lock (_instanceLocker)
                {
                    if (_instance == null) { _instance = new Singleton(); }
                }
            }
            return _instance;
        }

        public int Value1 { get; set; }
    }
}
