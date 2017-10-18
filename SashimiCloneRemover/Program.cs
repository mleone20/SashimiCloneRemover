using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SashimiCloneRemover
{
    class Program
    {

        /// <summary>
        /// Calcola l'hash del file usando l'algoritmo scelto.
        /// </summary>
        /// <param name="hashAlgorithm"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        static string GetHashForFile(HashAlgorithm hashAlgorithm, string filename)
        {
            try
            {
                // Hash di output
                string outputHash = "";

                // Apri il file 
                using (var fStream = System.IO.File.Open(filename, System.IO.FileMode.Open))
                {
                    // Calcoal l'hash
                    var hash = hashAlgorithm.ComputeHash(fStream);

                    // Ritorniamolo come stringa
                    foreach (var b in hash)
                        outputHash += b.ToString("x");
                }

                // Ritorna l'hash calcolato
                return outputHash;
            }
            catch(Exception e)
            {
                // Si è verificato un errore grave
                Console.Error.WriteLine(e.Message);
                return null;
            }
        }

        static void Main(string[] args)
        {
            // Gli argomenti sono validi?s
            if (args.Length != 2)
            {
                // Sriviamo giusto qualcosa per l'errore ...
                Console.Error.WriteLine("Argomenti non validi");
                return;
            }

            // Dictionary di hash calcolati fino ad ora
            HashSet<string> hashes = new HashSet<string>();

            // Il percorso nel quale cercare i file e il pattern da cercare
            string path = args[0];
            string searchPattern = args[1];

            // Creiamo il servizio che calcola l'md5
            HashAlgorithm hashAlgorithm = System.Security.Cryptography.MD5.Create();

            // Lista tutti i file in questa cartella
            foreach (var filename in System.IO.Directory.EnumerateFiles(path, searchPattern))
            {
                // Esiste già?
                string calculatedHash = GetHashForFile(hashAlgorithm, filename);

                // Se si sono verificati errori ...
                if (string.IsNullOrEmpty(calculatedHash))
                {
                    // Skip di questo file
                    continue;
                }

                // Se abbiamo già trovato quel file ...
                if (hashes.Contains(calculatedHash))
                {
                    // Scriviamo in output questo file perchè è doppione
                    Console.WriteLine(filename);
                    System.IO.File.Delete(filename);
                    continue;
                }

                // Salviamo l'hash calcolato (è nuovo, appena trovato!)
                hashes.Add(calculatedHash);
            }

            // Qui abbiamo finito :)
        }
    }
}