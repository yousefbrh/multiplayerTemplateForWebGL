using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Misc
{
    public class PasswordGenerator
    {
        private static readonly RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
    private static readonly char[] availableCharacters = 
    {
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
        'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
        'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '+'
    };

    public static string GenerateSecurePassword()
    {
        // Characters to ensure the password policy is met
        string passwordPolicyChars = GetPolicyCharacters();
        
        List<char> passwordChars = new List<char>();
        passwordChars.AddRange(passwordPolicyChars);

        // Fill the rest of the password length with random characters
        for (int i = passwordPolicyChars.Length; i < 16; i++)
        {
            passwordChars.Add(GetRandomCharacter(availableCharacters));
        }

        // Shuffle the constructed password to randomize character distribution
        passwordChars = Shuffle(passwordChars);

        return new string(passwordChars.ToArray());
    }

    private static string GetPolicyCharacters()
    {
        // Ensures the password contains at least one of each required type
        return $"{GetRandomCharacter("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray())}" +
               $"{GetRandomCharacter("abcdefghijklmnopqrstuvwxyz".ToCharArray())}" +
               $"{GetRandomCharacter("0123456789".ToCharArray())}" +
               $"{GetRandomCharacter("!@#$%^&*()-+".ToCharArray())}";
    }

    private static char GetRandomCharacter(char[] characterSet)
    {
        byte[] randomNumber = new byte[1];
        do
        {
            rngCsp.GetBytes(randomNumber);
        }
        while (!characterSet.Any(x => x == (char)randomNumber[0]));
        return (char)randomNumber[0];
    }

    private static List<char> Shuffle(List<char> list)
    {
        int n = list.Count;  
        while (n > 1) 
        {  
            n--;  
            int k = RandomInt(0, n + 1);  
            (list[k], list[n]) = (list[n], list[k]);  
        }

        return list;
    }

    private static int RandomInt(int min, int max)
    {
        uint scale = uint.MaxValue;
        while (scale == uint.MaxValue)
        {
            byte[] four_bytes = new byte[4];
            rngCsp.GetBytes(four_bytes);
            scale = BitConverter.ToUInt32(four_bytes, 0);
        }

        return (int)(min + (max - min) * (scale / (double)uint.MaxValue));
    }
    }
}