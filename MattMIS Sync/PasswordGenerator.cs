using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattMIS_Sync
{

    public static class PasswordGenerator
    {
        //string vowels = "aeiou";
        //string consonants = "bcdfghjklmnprstvwxyz";

        /*
           THIS CLASS IS NOT IN USE YET
        */

        static string vowels = "aaaaaeeeeeeeiiiiiooooouuu";
        static string consonants = "bbcccdddfffgghhhhjklllmmmnnnnnpprrrrrsssssttttttvwwxyyz";

        static string[] vowelafter = { "th", "ch", "sh", "qu" };
        static string[] consonantafter = { "oo", "ee" };
        static Random rnd = new Random();

        public static string GeneratePassword(int length)
        {
            string pass = "";
            bool isvowel = false;

            for (int i = 0; i < length; i++)
            {
                if (isvowel)
                {
                    if (rnd.Next(0, 5) == 0 && i < (length - 1))
                    {
                        if (rnd.Next(0, 1) == 0)
                        {
                            pass += consonantafter[rnd.Next(0, consonantafter.Length)].ToUpper(); ;
                        }
                        else
                        {
                            pass += consonantafter[rnd.Next(0, consonantafter.Length)].ToLower() ;
                        }

                    }
                    else
                    {
                        
                        if (rnd.Next(0, 1) == 0)
                        {
                            pass += vowels.Substring(rnd.Next(0, vowels.Length), 1).ToUpper(); 
                        }
                        else
                        {
                            pass += vowels.Substring(rnd.Next(0, vowels.Length), 1).ToLower();
                        }
                    }
                }
                else
                {
                    if (rnd.Next(0, 5) == 0 && i < (length - 1))
                    {
                       
                        if (rnd.Next(0, 1) == 0)
                        {
                            pass += vowelafter[rnd.Next(0, vowelafter.Length)].ToUpper();
                        }
                        else
                        {
                            pass += vowelafter[rnd.Next(0, vowelafter.Length)].ToLower();
                        }
                    }
                    else
                    {
                        if (rnd.Next(0, 1) == 0)
                        {
                            pass += consonants.Substring(rnd.Next(0, consonants.Length), 1).ToUpper();
                        }
                        else
                        {
                            pass += consonants.Substring(rnd.Next(0, consonants.Length), 1).ToLower();
                        }
                        
                    }
                }
                isvowel = !isvowel;
            }
            return pass;
        }
    }
}

