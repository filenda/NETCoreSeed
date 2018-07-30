using System;
using System.Collections.Generic;
using System.Text;

namespace NETCoreSeed.Shared.Helpers
{
    public class CommaSeparatedStringToIntListConverter
    {
        public IEnumerable<int> Convert(string str)
        {
            if (String.IsNullOrEmpty(str))
                yield break;

            foreach (var s in str.Split(','))
            {
                int num;
                if (int.TryParse(s, out num))
                    yield return num;
            }
        }
    }
}