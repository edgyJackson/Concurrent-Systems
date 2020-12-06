//----------------------------------------------------------
// CS 474 Lab 2 
// Reggie Johnson 
//Data Parallel program (all processors working on removing the multiple of the same prime)
//that finds out the number of primes that are less than 1000, 1,000,000, and 2,000,000 respectively using the Sieve of Eratosthens method. 
//----------------------------------------------------------
using System;
using System.Threading;
using System.Threading.Tasks; // parallelism 
using System.Diagnostics; //for the stop watch to work... milliseconds a milli a milli a milli
namespace SieveOfEratosthenes
{
    class Program
    {

        static void sieveOfEratosthenes(int size){


            int count = 0; //for counting the number of 1’s still in the array.


            // 2. Declare an array of int/short/char/bit of SIZE representing integers from 1 to SIZE.
            int[] primeNumbersArray = new int[size];


            // 3. Initialize the array to all 1’s. This step can be done in parallel.
            Parallel.For(0, primeNumbersArray.Length, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
           {
               primeNumbersArray[i] = 1;

           });

            // 4. Looping through the array to remove the multiples of primes. 
            for (int i = 2; i * i < size; i++)
            {
                // See if i is marked as 1 indicating that it is still considered a prime number
                if (primeNumbersArray[i] == 1)
                {
                    // mark all multiples of i as 0 since we know it ***aint*** a prime number.
                    Parallel.For(i*2,  size, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, j => {

                        if(j % i == 0)
                        primeNumbersArray[j] = 0;
                        

                        
                        });
                }
            }

            //5. Count the number of 1’s still in the array.
            Parallel.For(2, size, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, ()=>0, (i,loop, subtotal) =>
           { subtotal += primeNumbersArray[i];
               return subtotal;
           },
              (x) => Interlocked.Add(ref count, x)        
           );

            //used environmental variable to find processor count my ryzen 7 1800x cpu has 8 cores so that 16 threads via hyperthreading
            Console.WriteLine("The number of processors on this computer is {0}.", Environment.ProcessorCount);
            Console.WriteLine("Number of primes that are less than or equal to {0} is {1}\n", size, count);

        } 


        static void Main(string[] args)
        {

            // 1. Again define a constant SIZE and make it 20 initially for easy debugging.           
            const int SIZE = 10000000;

            Stopwatch stopwatch = new Stopwatch();       
            stopwatch.Start();

            //run sieve algorithm on SIZE
            sieveOfEratosthenes(SIZE);

            stopwatch.Stop();
            
            //display stopwatch information for sequential array fill
            Console.WriteLine("duration in milliseconds: {0}\n", stopwatch.ElapsedMilliseconds);

            // Reset timer. 
            stopwatch.Reset();

        }
    }
}
