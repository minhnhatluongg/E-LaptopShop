using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Services.Implementations
{
    public class SlugGenerator
    {
        private static readonly Regex NonWord = new(@"[^a-z0-9\- ]", RegexOptions.Compiled);
        private static readonly Regex MultiDash = new(@"-+", RegexOptions.Compiled);

       
        public SlugGenerator()
        {

        }
    }
}
