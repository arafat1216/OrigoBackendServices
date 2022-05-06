using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public interface IFlatDictionaryProvider
    {
        Dictionary<string, string> Execute(object @object, string prefix = "");
    }
}
