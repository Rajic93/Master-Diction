using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diction_Master___Library
{
    public interface IObserver
    {
        void Update(List<ContentVersionInfo> changes);
    }
}
