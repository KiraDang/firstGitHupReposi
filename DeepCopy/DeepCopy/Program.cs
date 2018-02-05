using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DeepCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                Console.Write("Demo of shallow and deep copy, using classes and MemberwiseCopy:\n");
                var Bob = new Person(30, "Lamborghini");
                Console.Write("  Create Bob\n");
                Console.Write("    Bob.Age={0}, Bob.Purchase.Description={1}\n", Bob.Age, Bob.Purchase.Description);
                Console.Write("  Clone Bob >> BobsSon\n");
                var BobsSon = Bob.DeepCopy();
                Console.Write("  Adjust BobsSon details\n");
                BobsSon.Age = 2;
                BobsSon.Purchase.Description = "Toy car";
                Console.Write("    BobsSon.Age={0}, BobsSon.Purchase.Description={1}\n", BobsSon.Age, BobsSon.Purchase.Description);
                Console.Write("  Proof of deep copy: If BobsSon is a true clone, then adjusting BobsSon details will not affect Bob:\n");
                Console.Write("    Bob.Age={0}, Bob.Purchase.Description={1}\n", Bob.Age, Bob.Purchase.Description);
                Debug.Assert(Bob.Age == 30);
                Debug.Assert(Bob.Purchase.Description == "Lamborghini");
                var sw = new Stopwatch();
                sw.Start();
                int total = 0;
                for (int i = 0; i < 100000; i++)
                {
                    var n = Bob.DeepCopy();
                    total += n.Age;
                }
                Console.Write("  Elapsed time: {0},{1}\n", sw.Elapsed, total);
            }
            {
                Console.Write("Demo of shallow and deep copy, using structs:\n");
                var Bob = new PersonStruct(30, "Lamborghini");
                Console.Write("  Create Bob\n");
                Console.Write("    Bob.Age={0}, Bob.Purchase.Description={1}\n", Bob.Age, Bob.Purchase.Description);
                Console.Write("  Clone Bob >> BobsSon\n");
                var BobsSon = Bob.DeepCopy();
                Console.Write("  Adjust BobsSon details:\n");
                BobsSon.Age = 2;
                BobsSon.Purchase.Description = "Toy car";
                Console.Write("    BobsSon.Age={0}, BobsSon.Purchase.Description={1}\n", BobsSon.Age, BobsSon.Purchase.Description);
                Console.Write("  Proof of deep copy: If BobsSon is a true clone, then adjusting BobsSon details will not affect Bob:\n");
                Console.Write("    Bob.Age={0}, Bob.Purchase.Description={1}\n", Bob.Age, Bob.Purchase.Description);
                Debug.Assert(Bob.Age == 30);
                Debug.Assert(Bob.Purchase.Description == "Lamborghini");
                var sw = new Stopwatch();
                sw.Start();
                int total = 0;
                for (int i = 0; i < 100000; i++)
                {
                    var n = Bob.DeepCopy();
                    total += n.Age;
                }
                Console.Write("  Elapsed time: {0},{1}\n", sw.Elapsed, total);
            }
            {
                Console.Write("Demo of deep copy, using class and serialize/deserialize:\n");
                int total = 0;
                var sw = new Stopwatch();
                sw.Start();
                var Bob = new Person(30, "Lamborghini");
                for (int i = 0; i < 100000; i++)
                {
                    var BobsSon = MyDeepCopy.DeepCopy<Person>(Bob);
                    total += BobsSon.Age;
                }
                Console.Write("  Elapsed time: {0},{1}\n", sw.Elapsed, total);
            }
            Console.ReadKey();
        }

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }

    public class Person
    {
        public Person(int age, string description)
        {
            this.Age = age;
            this.Purchase.Description = description;
        }
        [Serializable] // Not required if using MemberwiseClone
        public class PurchaseType
        {
            public string Description;
            public PurchaseType ShallowCopy()
            {
                return (PurchaseType)this.MemberwiseClone();
            }
        }
        public PurchaseType Purchase = new PurchaseType();
        public int Age;
        // Add this if using nested MemberwiseClone.
        // This is a class, which is a reference type, so cloning is more difficult.
        public Person ShallowCopy()
        {
            return (Person)this.MemberwiseClone();
        }
        // Add this if using nested MemberwiseClone.
        // This is a class, which is a reference type, so cloning is more difficult.
        public Person DeepCopy()
        {
            // Clone the root ...
            Person other = (Person)this.MemberwiseClone();
            // ... then clone the nested class.
            other.Purchase = this.Purchase.ShallowCopy();
            return other;
        }
    }

    public struct PersonStruct
    {
        public PersonStruct(int age, string description)
        {
            this.Age = age;
            this.Purchase.Description = description;
        }
        public struct PurchaseType
        {
            public string Description;
        }
        public PurchaseType Purchase;
        public int Age;
        // This is a struct, which is a value type, so everything is a clone by default.
        public PersonStruct ShallowCopy()
        {
            return (PersonStruct)this;
        }
        // This is a struct, which is a value type, so everything is a clone by default.
        public PersonStruct DeepCopy()
        {
            return (PersonStruct)this;
        }
    }

    public class MyDeepCopy
    {
        public static T DeepCopy<T>(T obj)
        {
            object result = null;
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                result = (T)formatter.Deserialize(ms);
                ms.Close();
            }
            return (T)result;
        }
    }
}
