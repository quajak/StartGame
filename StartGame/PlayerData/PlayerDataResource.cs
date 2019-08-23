using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.PlayerData
{
    static class PlayerDataResource
    {
        static List<string> MaleNames;

        public static string GetMaleName()
        {
            if(MaleNames is null)
            {
                MaleNames = File.ReadAllLines(@".\Resources\MaleNames.txt").ToList();
            }
            return MaleNames.GetRandom();
        }
    }
}
